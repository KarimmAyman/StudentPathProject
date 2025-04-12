using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.StripeService
{
    public class StripeService
    {

        public StripeService()
        {
        }

        public async Task<string> CreateStripeCustomerAsync(string email, string userName)
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = email,
                Name = userName
            });
            return customer.Id;
        }

        // **Step 2: Add Card to Customer**
        public async Task<string> AddCardToCustomerAsync(string customerId, string cardNumber, int expMonth, int expYear, string cvc)
        {
            var paymentMethodService = new PaymentMethodService();

            var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = cvc
                }
            });

            await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
            {
                Customer = customerId
            });

            return paymentMethod.Id;
        }

        // **Step 3: Create Payment Intent for Wallet Top-Up**
        public async Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, decimal amount)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" }
            });

            return paymentIntent;
        }
    }
}


