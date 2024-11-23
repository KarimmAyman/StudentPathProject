using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    // Enum for Escrow Status
    public enum EscrowStatus
    {
        Pending,       // Funds are still held in escrow
        Completed,     // Funds have been released to the driver
        Canceled       // Funds were returned to the student (canceled trip)
    }
    public class EscrowAccount
    {
        [Key]
        public int EscrowAccountId { get; set; }  
       
        public virtual ICollection<Booking> Bookings { get; set; }

        [Range(0, double.MaxValue)]  // Ensure non-negative values for the amount
        public decimal Amount { get; set; }  // Amount held in escrow

        [Range(0, double.MaxValue)]  // Ensure non-negative values for the fee
        public decimal Fee { get; set; }  // A fee is a service charge or commission deducted from the total payment

        public EscrowStatus Status { get; set; } = EscrowStatus.Pending;  // Default to Pending

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current UTC time
        public DateTime? ReleasedAt { get; set; }  // Date when funds were released to the driver

        // Optional: Soft deletion flag
        public bool IsDeleted { get; set; } = false;
    }




}
