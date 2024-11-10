using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }  // Primary key for the payment

        public int BookingId { get; set; }  
        public Booking Booking { get; set; }  

        [Required]
        public decimal Amount { get; set; }  

        public PaymentStatus PaymentStatus { get; set; }  

        public DateTime PaymentDate { get; set; } 

        public string TransactionId { get; set; }  // Unique transaction ID for tracking

        public string PaymentMethod { get; set; }  // Method of payment (e.g., Credit Card, PayPal, etc.)
   
        // link Payment with an EscrowAccount
        public int? EscrowAccountId { get; set; }  // Foreign Key to EscrowAccount
        public EscrowAccount EscrowAccount { get; set; }  // Navigation property to EscrowAccount
    }

}
