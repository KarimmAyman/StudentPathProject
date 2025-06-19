using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Users
{
    public class UserAddDTO
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public UserTypeEnum UserType { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please Enter Your Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The Range of age between 18 and 100")]

        [Range(18, 100)]
        public int Age { get; set; }



        [Required(ErrorMessage = "Gender is required")]
        public GenderType Gender { get; set; }



        public string? ImgUrl { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]


        public IFormFile? ProfileImage { get; set; }
        public UserAddDTO()
        {
            Id = Guid.NewGuid().ToString();
            UserType = UserTypeEnum.User;
        }
    }
    public class AddCardDto
    {
        [Required]
        public string StripeCustomerId { get; set; } // Stripe Customer ID

        [Required]
        public string PaymentMethodId { get; set; } // Tokenized payment method

    }
    
    public class StripeUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
   
    public class CreatePaymentIntentDto
    {
        public string Email { get; set; } // Optional, for tracking users in your system
        public decimal Amount { get; set; } // Amount to charge
        public string? PaymentMethodId { get; set; }
        public string Currency { get; set; }

    }
    public class PaymentMethodRequest
    {
        public string CardType { get; set; } // Accepts "visa" or "mastercard"
    }


    public class PaymobOrderResponse
    {
        public int Id { get; set; }
    }

    public class PaymentTokenResponse
    {
        public string Token { get; set; }
    }

    public class RedirectUrlResponse
    {
        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }
        [JsonProperty("id")]

        public string TransactionId { get; set; }

    }
    public class AuthResponse
    {
        public string token { get; set; }
    }
    public class WalletPaymentRequest
    {
        public int BookingId { get; set; }
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
    }

    public class WalletPaymentResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }

        public string RedirectUrl { get; set; }
    }
    public class PaymobWebhookRequest
    {

        public string Type { get; set; }

        public PaymobWebhookObj Obj { get; set; }
    }

    public class PaymobWebhookObj
    {

        public long Id { get; set; }  // Transaction ID
                                        // Payment status (true for success, false for failure)

        public bool Success { get; set; }

        public long OrderId { get; set; }  // The order ID in Paymob's system

        public long AmountCents { get; set; }  // The amount in cents

        public string Currency { get; set; }  // Currency code (e.g., "EGP")
                                              // You can add more fields depending on what Paymob sends in the webhook
    }
    public class CreatePaymentRequest
    {
        public long Amount { get; set; }  // Amount in smallest currency unit (e.g., cents)
        public string Currency { get; set; } = "EGP";
        public string Email { get; set; }
        public int BookingId { get; set; }


    }


}

