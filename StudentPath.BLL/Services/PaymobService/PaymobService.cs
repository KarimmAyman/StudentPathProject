using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.BLL.Services.AccountService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.PaymobService
{
    public class PaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly StudentPathContext _context;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PaymobService(HttpClient httpClient, IConfiguration config,StudentPathContext _context,IEmailService emailService,IHttpContextAccessor httpContextAccessor)
        {
           this._httpClient = httpClient;
            this._config = config;
            this._context = _context;
            this.emailService = emailService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<WalletPaymentResponse> InitiateWalletPaymentAsync(WalletPaymentRequest request)
        {
            string baseUrl2 = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
            string logoUrl = $"{baseUrl2}/Uploads/Aoun-logo.svg";
            var apiKey = _config["Paymob:ApiKey"];
            var integrationId = int.Parse(_config["Paymob:IntegrationId"]);
            var baseUrl = _config["Paymob:BaseUrl"];


            var booking = await _context.Bookings
           .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);
            if (booking == null)
                throw new Exception($"Booking #{request.BookingId} not found.");
            if (booking.TotalPrice <= 0)
                throw new InvalidOperationException("Invalid booking total price. Cannot proceed with payment.");

            if (request.Amount <= 0 || request.Amount != booking.TotalPrice)
                throw new InvalidOperationException("Invalid payment amount.");
            var existingPayment = await _context.Payments
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync(p => p.BookingId == booking.BookingId && p.PaymentMethod == PaymentMethodEnum.Wallet);

            if (existingPayment != null)
            {
                if (existingPayment.PaymentStatus == PaymentStatus.Paid)
                    throw new InvalidOperationException("This booking has already been paid.");

                if (existingPayment.PaymentStatus == PaymentStatus.Pending)
                    throw new InvalidOperationException("A wallet payment is already pending for this booking. Please wait for confirmation.");

            }                


                // 1. Authenticate
                var authResp = await _httpClient.PostAsJsonAsync($"{baseUrl}/auth/tokens", new { api_key = apiKey });
            if (!authResp.IsSuccessStatusCode) throw new Exception("Authentication with Paymob failed.");

            var authResult = JsonConvert.DeserializeObject<AuthResponse>(await authResp.Content.ReadAsStringAsync());
            var authToken = authResult?.token;
            if (string.IsNullOrEmpty(authToken)) throw new Exception("Auth token is null or empty.");

            // 2. Create Order
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(request.Amount * 100),
                currency = "EGP",
                items = Array.Empty<object>()
            };

            var orderResp = await _httpClient.PostAsJsonAsync($"{baseUrl}/ecommerce/orders", orderRequest);
            if (!orderResp.IsSuccessStatusCode) throw new Exception("Order creation failed.");

            var orderResult = JsonConvert.DeserializeObject<PaymobOrderResponse>(await orderResp.Content.ReadAsStringAsync());

            // 3. Create Payment Key
            var tokenRequest = new
            {
                auth_token = authToken,
                amount_cents = (int)(booking.TotalPrice * 100),
                expiration = 3600,
                order_id = orderResult.Id,
                billing_data = new
                {
                    apartment = "NA",
                    email = request.Email,
                    floor = "NA",
                    first_name = "Test",
                    street = "NA",
                    building = "NA",
                    phone_number = request.WalletId,
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "User",
                    state = "Cairo"
                },
                currency = "EGP",
                integration_id = integrationId
            };

            var tokenResp = await _httpClient.PostAsJsonAsync($"{baseUrl}/acceptance/payment_keys", tokenRequest);
            if (!tokenResp.IsSuccessStatusCode) throw new Exception("Payment token request failed.");

            var tokenResult = JsonConvert.DeserializeObject<PaymentTokenResponse>(await tokenResp.Content.ReadAsStringAsync());

            // 4. Trigger Wallet Payment
            var paymentRequest = new
            {
                source = new
                {
                    identifier = request.WalletId,
                    subtype = "WALLET"
                },
                payment_token = tokenResult.Token
            };

            var paymentResp = await _httpClient.PostAsJsonAsync($"{baseUrl}/acceptance/payments/pay", paymentRequest);
            var paymentResult = JsonConvert.DeserializeObject<RedirectUrlResponse>(await paymentResp.Content.ReadAsStringAsync());

            // Get Paymob TransactionId
            var transactionId = paymentResult?.TransactionId ?? "No Transaction ID";

            // 5. Lookup user and record in DB
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new Exception("User not found.");

            // Record payment
            var payment = new Payment
            {
                UserId = user.Id,
                BookingId = request.BookingId,              // ← link payment to booking
                Amount = booking.TotalPrice,
                PaymentStatus = PaymentStatus.Pending, // Or Pending if you want to wait for webhook
                PaymentDate = DateTime.UtcNow,
                TransactionId = transactionId,
                PaymentMethod = PaymentMethodEnum.Wallet
            };
            _context.Payments.Add(payment);

            //// Update wallet balance
            //var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            //if (wallet == null)
            //{
            //    wallet = new Wallet
            //    {
            //        UserId = user.Id,
            //        Balance = 0
            //    };
            //    _context.Wallets.Add(wallet);
            //}
            //wallet.Balance += request.Amount;

            //wallet.LastTransactionId = paymentResult.TransactionId;



            //// Record wallet transaction
            //var walletTransaction = new WalletTransaction
            //{
            //    Wallet = wallet,
            //    Amount = request.Amount,
            //    TransactionDate = DateTime.UtcNow,
            //    PaymobTransactionId = transactionId
            //};
            //_context.WalletsTransactions.Add(walletTransaction);

            await _context.SaveChangesAsync();
            if (!string.IsNullOrWhiteSpace(paymentResult?.RedirectUrl))
            {

                string confirmationLink = paymentResult.RedirectUrl;

                string confirmationEmailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #f6f9fc;
            padding: 20px;
            font-family: Arial, sans-serif;
            border-radius: 8px;
            text-align: left;
        }}
        .card {{
            background-color: white;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.1);
        }}
        .button {{
            background-color: #83cd20;
            color: white;
            padding: 12px 24px;
            border-radius: 6px;
            text-decoration: none;
            font-weight: bold;
            display: inline-block;
            margin-top: 20px;
            text-align: center;
        }}
        .footer {{
            font-size: 12px;
            color: #666;
            margin-top: 20px;
        }}
        .logo {{
            display: inline-block;
            width: 150px;
            vertical-align: middle;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='card'>
            <div>
                <img src='{logoUrl}' class='logo' alt='Student Path Logo' />
            </div>
            <p>Thanks for initiating a wallet payment of <strong>{request.Amount} EGP</strong>.</p>
            <p>Please confirm your payment by clicking the button below:</p>
            <a href='{confirmationLink}' class='button'>Confirm Wallet Payment</a>
            <p>This link will expire soon. Do not share it.</p>
        </div>
        <div class='footer'>
            <p>Student Path, Kafr El-Sheikh, Egypt</p>
        </div>
    </div>
</body>
</html>";

                await emailService.SendEmailAsync(request.Email, "Confirm Your Wallet Payment", confirmationEmailBody);
            }
            return new WalletPaymentResponse
            {
                Status = string.IsNullOrWhiteSpace(paymentResult?.RedirectUrl) ? "failed" : "success",
                Message = string.IsNullOrWhiteSpace(paymentResult?.RedirectUrl)
         ? "Wallet payment failed or is pending confirmation."
         : "Wallet payment initiated successfully. Please check your email to confirm the payment.",
                TransactionId = transactionId,
                RedirectUrl = paymentResult?.RedirectUrl
            };
        }

    }
}
