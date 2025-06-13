using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }  // Primary key for booking

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }  // Foreign key to User
        public virtual User User { get; set; }  // Navigation property

        [Required]
        [ForeignKey("Trip")]
        public int TripId { get; set; }  // Foreign key to Trip
        public virtual Trip Trip { get; set; }  // Navigation property

        [Required]
        public DateTime BookingDate { get; set; }  // Date of booking

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }  // Total price for the booking

        public bool IsCancelled { get; set; } = false;  // Default value is false
        [Required]
        public int NumberOfSeats { get; set; }

        public Coordinate MeetingPoint { get; set; }  // Replaced string with Location object

        [MaxLength(500)]
        public string? Note { get; set; }
        [Required]
        public BookingStatus BookingStatus { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }  // Status of the payment (Pending, Paid, Cancelled)


        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();


        // Relationship to EscrowAccount (optional)
        public int? EscrowAccountId { get; set; }  // Foreign Key to EscrowAccount
        public virtual EscrowAccount EscrowAccount { get; set; }  // Navigation property to EscrowAccount
    }

    [Owned] // EF Core Owned Entity Type
    public class Coordinate
    {
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,  // Payment is pending or in escrow
        Paid,     // Payment has been completed successfully
        Cancelled // Payment was cancelled or refunded
    }

    public enum BookingStatus
    {
        Pending,   // Booking is confirmed but payment is pending
        Confirmed, // Booking is confirmed and payment is completed
        Cancelled  // Booking has been cancelled by the user
    }
}




