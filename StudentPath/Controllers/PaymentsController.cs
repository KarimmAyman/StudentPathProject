using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Identity;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.BLL.Services.StripeService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly StripeService _stripeService;
        private readonly IConfiguration configuration;
        private readonly StudentPathContext _context;


        public PaymentsController(StudentPathContext context, StripeService stripeService,IConfiguration configuration)
        {
            _context = context;
            _stripeService = stripeService;
            this.configuration = configuration;
        }
        #region RegisterUser
        // **Register User and Create Stripe Customer**
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] StripeUserDTO userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check if user is already registered in Stripe
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                return BadRequest(new { message = "User is already registered with Stripe" });
            }

            // Register user in Stripe
            user.StripeCustomerId = await _stripeService.CreateStripeCustomerAsync(user.Email, user.UserName);

            // Save changes to database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered in Stripe successfully", StripeCustomerId = user.StripeCustomerId });

        }
        #endregion


        #region AddCard

        // **Add Card to Stripe and Save it in DB**
        [HttpPost("add-card")]
        public async Task<IActionResult> AddCard([FromBody] AddCardDto request)
        {

            try
            {
                var customerService = new CustomerService();
                var paymentMethodService = new PaymentMethodService();

                // Attach PaymentMethod to Customer
                await paymentMethodService.AttachAsync(request.PaymentMethodId, new PaymentMethodAttachOptions
                {
                    Customer = request.StripeCustomerId
                });

                // Set the PaymentMethod as default
                await customerService.UpdateAsync(request.StripeCustomerId, new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = request.PaymentMethodId
                    }
                });

                // (Optional) Save PaymentMethodId in the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == request.StripeCustomerId);
                if (user != null)
                {
                    user.DefaultPaymentMethodId = request.PaymentMethodId;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = "Card added successfully!", PaymentMethodId = request.PaymentMethodId });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        #endregion



        #region GetStripeCustomer


        [HttpGet("stripe-customer-id/{email}")]
        public async Task<IActionResult> GetStripeCustomerId(string email)
        {
            try
            {
                // ✅ Step 1: Fetch User by Email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                // ✅ Step 2: Check if User has a Stripe Customer ID
                if (string.IsNullOrEmpty(user.StripeCustomerId))
                    return BadRequest(new { message = "User does not have a Stripe Customer ID" });

                // ✅ Step 3: Return the Stripe Customer ID
                return Ok(new
                {
                    email = user.Email,
                    stripeCustomerId = user.StripeCustomerId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion



        #region CreatePaymentIntent Using PMI

        [HttpPost("create-payment-intent-withPMI")]
        public async Task<IActionResult> CreatePaymentIntentWithPMI([FromBody] CreatePaymentIntentDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Start DB Transaction


            try
            {
                // Get the user from the database using the provided email
                var user = await _context.Users
                    .Where(u => u.Email == dto.Email)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                string currency = string.IsNullOrEmpty(dto.Currency) ? "usd" : dto.Currency.ToLower();

                decimal amountInDollars = dto.Amount;
                long amountInCents = (long)(amountInDollars * 100); // Convert to cents

                // ✅ Ensure payment method is provided
                if (string.IsNullOrEmpty(dto.PaymentMethodId))
                    return BadRequest(new { message = "Payment method is required" });

                // ✅ Check if the user has a Stripe customer ID
                if (string.IsNullOrEmpty(user.StripeCustomerId))
                {
                    // If the user doesn't have a Stripe customer ID, create one
                    var customerOptions = new CustomerCreateOptions
                    {
                        Email = user.Email,
                        Name=user.UserName
                    };
                    var customerService = new CustomerService();
                    var stripeCustomer = await customerService.CreateAsync(customerOptions);

                    user.StripeCustomerId = stripeCustomer.Id;
                    await _context.SaveChangesAsync(); // Save the new StripeCustomerId to your user record
                }

                // Create Stripe PaymentIntent for one-time payment
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = currency,
                    PaymentMethod = dto.PaymentMethodId,
                    Customer = user.StripeCustomerId, // Attach the Stripe CustomerId
                    PaymentMethodTypes = new List<string> { "card" },
                    Confirm = true // Auto-confirm payment
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // ✅ Save the transaction in your local database
                var newPayment = new Payment
                {
                    UserId = user.Id, // Use the UserId for the payment
                    Amount = amountInDollars,
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentMethod = PaymentMethodEnum.CreditCard,
                    TransactionId = paymentIntent.Id, // Stripe transaction ID
                    PaymentIntentId = paymentIntent.Id // Store PaymentIntentId for tracking
                  
                };

                _context.Payments.Add(newPayment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Commit transaction in DB

                return Ok(new
                {
                    status = "Success",
                    message = "Payment successful via Card",
                    paymentIntentId = paymentIntent.Id,
                    clientSecret = paymentIntent.ClientSecret,
                    stripeCustomerId=paymentIntent.CustomerId
                   // Return the UserId in the payment transaction response
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback in case of failure
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion


        #region CreatePyamentMethod

        [HttpPost("create-payment-method")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethodRequest request)
        {
            try
            {
                // Choose the correct token based on card type
                string token = request.CardType.ToLower() switch
                {
                    "visa" => "tok_visa",
                    "mastercard" => "tok_mastercard",
                    _ => throw new ArgumentException("Unsupported card type. Use 'visa' or 'mastercard'.")
                };

                var service = new PaymentMethodService();
                var paymentMethod = await service.CreateAsync(new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Token = token // Use the selected test token
                    }
                });

                return Ok(new { PaymentMethodId = paymentMethod.Id });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        #endregion


        #region Create Payment Intent


        [HttpPost("Create-Payment-Intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Start DB Transaction

            try
            {

                var user = await _context.Users
                   .Where(u => u.Email ==request.Email)
                   .FirstOrDefaultAsync();
                // 1. Get or create Stripe customer
                var customerService = new CustomerService();
                var customers = await customerService.ListAsync(new CustomerListOptions
                {
                    Email = request.Email
                });

                var customer = customers.FirstOrDefault();

                if (customer == null)
                {
                    customer = await customerService.CreateAsync(new CustomerCreateOptions
                    {
                        Email = user.Email,
                        Name=user.UserName
                    });
                }

                // 2. Create Ephemeral Key (requires Stripe version override)
                var ephemeralKeyOptions = new EphemeralKeyCreateOptions
                {
                    Customer = customer.Id,
                    StripeVersion = "2025-02-24.acacia", // Example version
                };
                var ephemeralKeyService = new EphemeralKeyService();
                var ephemeralKey = ephemeralKeyService.Create(ephemeralKeyOptions);

                // 3. Create PaymentIntent with automatic payment methods and status as pending
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = request.Amount * 100,  // Stripe requires the amount in cents
                    Currency = request.Currency,
                    Customer = customer.Id,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                        AllowRedirects="never"
                    }
                   
                });

                // 4. Save payment record in the database (status = pending)
                var newPayment = new Payment
                {
                    UserId = user.Id, // Ensure you are saving the User ID in the payment
                    Amount = request.Amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Pending,  // Set initial status to Pending
                    PaymentMethod = PaymentMethodEnum.CreditCard,  // Assuming CreditCard as the payment method
                    TransactionId = paymentIntent.Id,  // Save the Stripe PaymentIntent ID
                    PaymentIntentId = paymentIntent.Id, // Track PaymentIntent ID
                };

                _context.Payments.Add(newPayment);
                await _context.SaveChangesAsync();

                // 5. Commit transaction to ensure DB consistency
                await transaction.CommitAsync();

                // 6. Return the required data to the frontend
                return Ok(new
                {
                    CustomerId = customer.Id,
                    EphemeralKeySecret = ephemeralKey.Secret,
                    PaymentIntentClientSecret = paymentIntent.ClientSecret,
                    PublishableKey = configuration["Stripe:PublishableKey"],  // Your Stripe Publishable Key
                    PaymentIntentId = paymentIntent.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback DB changes in case of error
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion


        #region Stripe Webhook
        [HttpPost("Stripe-Webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            // You can verify the webhook signature to ensure it comes from Stripe
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var secret = configuration["Stripe:WebhookSecret"];  // Your Stripe webhook secret
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    secret,
                    throwOnApiVersionMismatch: false

                );
            }
            catch (StripeException e)
            {
                // Log the error and return bad request
                return BadRequest(new { message = "Invalid webhook signature." });
            }

            // Handle the event (you can add more events as needed)
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentIntentSucceeded(paymentIntent);
                    break;

                case "payment_intent.payment_failed":
                    var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentIntentFailed(failedPaymentIntent);
                    break;


                // Handle other events here (optional)
                default:
                    break;
            }

            return Ok();
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            // Find the corresponding payment record in your database
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentIntent.Id);

            if (payment != null)
            {
                // Update the payment status to "Succeeded"
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private async Task HandlePaymentIntentFailed(PaymentIntent paymentIntent)
        {
            // Find the corresponding payment record in your database
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentIntent.Id);

            if (payment != null)
            {
                // Update the payment status to "Failed"
                payment.PaymentStatus = PaymentStatus.Cancelled;
                payment.PaymentDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        #endregion

    }

}


















