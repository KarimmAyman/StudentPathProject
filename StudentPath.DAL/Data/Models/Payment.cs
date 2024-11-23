using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{



    public enum PaymentMethodEnum
    {
        CreditCard,
        PayPal,
        BankTransfer,
        Cash
    }

    public class Payment
    {
        public int PaymentId { get; set; }  // Primary key for the payment

        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }  // Navigation property to Booking

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }  // Payment amount

        public PaymentStatus PaymentStatus { get; set; }  // Status of the payment

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;  // Default to current time

        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; }  // Unique transaction ID for tracking

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }  // Method of payment (e.g., Credit Card, PayPal, etc.)

        // Optional: link Payment with an EscrowAccount
        [ForeignKey("EscrowAccount")]
        public int? EscrowAccountId { get; set; }  // Foreign Key to EscrowAccount
        public virtual EscrowAccount EscrowAccount { get; set; }  // Navigation property to EscrowAccount

        // Optional: Timestamp for record creation and updates
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }


}
