using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.BLL.Services.PaymobService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly PaymobService _walletService;
        private readonly StudentPathContext context;

        public PaymobController(PaymobService walletService, StudentPathContext context)
        {
            _walletService = walletService;
            this.context = context;
        }

        [HttpPost("Wallet")]
        public async Task<IActionResult> Wallet([FromBody] WalletPaymentRequest request)
        {
            var result = await _walletService.InitiateWalletPaymentAsync(request);
            return Ok(result);
        }
        [HttpPost("paymob/webhook")]
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookRequest request)
        {
            // Step 1: Retrieve the payment record based on the Paymob TransactionId
            var payment = await context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == request.Obj.Id.ToString());

            if (payment == null)
                return NotFound(new { status = "failed", error = $"No payment found with TransactionId = {request.Obj.Id}" });
            if (string.IsNullOrWhiteSpace(request.Type))
                return BadRequest(new { status = "failed", error = "Missing 'type' field in payload." });
            if (request.Obj == null)
                return BadRequest(new { status = "failed", error = "Missing 'obj' field in payload." });

            if (payment.PaymentStatus == PaymentStatus.Paid)
            {
                return BadRequest("This payment has already been confirmed as paid. Please try a different transaction.");
            }
            // Step 2: Handle payment status based on the webhook response

            if (request.Obj.Success && request.Type == "TRANSACTION") // Successful payment
            {
                // Step 2a: Mark the payment as successful
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;

                // Step 2b: Update the wallet balance and log the transaction
                var wallet = await context.Wallets.FirstOrDefaultAsync(w => w.UserId == payment.UserId);
                if (wallet == null) // Create a wallet if it doesn't exist
                {
                    wallet = new Wallet
                    {
                        UserId = payment.UserId,
                        Balance = payment.Amount,  // Set the balance to the payment amount
                        LastTransactionId = payment.TransactionId
                    };

                    // Add the new wallet to the database
                    context.Wallets.Add(wallet);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Step 2b1: Add the payment amount to the user's wallet balance
                    wallet.Balance += payment.Amount;
                    wallet.LastTransactionId = payment.TransactionId;
                }
                var walletTransaction = new WalletTransaction
                    {
                        WalletId = wallet.WalletId,
                        Amount = payment.Amount,
                        TransactionDate = DateTime.UtcNow,
                        PaymobTransactionId = payment.TransactionId
                    };

                    context.WalletsTransactions.Add(walletTransaction);
                

                // Commit all changes to the database
                await context.SaveChangesAsync();
            }
            else if (!request.Obj.Success && request.Type == "TRANSACTION") // Failed payment
            {
                // Step 3: Mark the payment as failed
                payment.PaymentStatus = PaymentStatus.Cancelled;
                payment.PaymentDate = DateTime.UtcNow;

                // No wallet update since the payment is failed, just update the payment record
                await context.SaveChangesAsync();
            }

            // Step 4: Optionally, handle any pending payments or timeouts
            // For example, you could set a timeout period for pending payments.
            else if (payment.PaymentStatus == PaymentStatus.Pending)
            {
                // Set a timeout period for pending payments (e.g., 24 hours)
                if (DateTime.UtcNow - payment.PaymentDate > TimeSpan.FromHours(24))
                {
                    payment.PaymentStatus = PaymentStatus.Pending; // Timeout exceeded, mark as failed
                    await context.SaveChangesAsync();
                }
            }

            return Ok(new
            {
                status = "success",
                message = "Webhook processed successfully",
                transactionId = request.Obj.Id,
                newStatus = payment.PaymentStatus.ToString()
            });
        }



        [HttpGet("Paymob-status")]
        public async Task<IActionResult> GetPaymobStatus([FromQuery] string transactionId)
        {
            var payment = await context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
            if (payment == null) return NotFound("Payment not found");

            return Ok(new
            {
                Status = payment.PaymentStatus.ToString(),
                Amount = payment.Amount,
                Date = payment.PaymentDate
            });
        }




    }
}
