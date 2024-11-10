using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{

    public class Booking
    {
        public int BookingId { get; set; }  // Primary key for booking

        public int UserId { get; set; }  
        public User User { get; set; }

        public int TripId { get; set; }  
        public Trip Trip { get; set; } 

        public DateTime BookingDate { get; set; }  

        public decimal TotalPrice { get; set; } 

        public bool IsCancelled { get; set; }  

        public PaymentStatus PaymentStatus { get; set; }  // Status of the payment (Pending, Paid, Cancelled)
        //link Booking with an EscrowAccount
        public int? EscrowAccountId { get; set; }  // Foreign Key to EscrowAccount
        public EscrowAccount EscrowAccount { get; set; }  // Navigation property to EscrowAccount

    }
    public enum PaymentStatus
    {
        Pending,  // Payment is pending or in escrow
        Paid,     // Payment has been completed successfully
        Cancelled // Payment was cancelled or refunded
    }

    public enum BookingStatus
    {
        Pending,  // Booking is confirmed but payment is pending
        Confirmed, // Booking is confirmed and payment is completed
        Cancelled  // Booking has been cancelled by the user
    }

}


