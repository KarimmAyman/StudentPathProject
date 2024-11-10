using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class EscrowAccount
    {
        public int EscrowAccountId { get; set; }  // Primary Key

        public int BookingId { get; set; }  // Foreign key to Booking
        public Booking Booking { get; set; }  // Navigation property

        public decimal Amount { get; set; }  // Amount held in escrow
        public decimal Fee { get; set; }  // A fee is a service charge or commission deducted from the total payment,
                                          // typically taken by the platform to cover operational and transaction costs.

        public EscrowStatus Status { get; set; }  // Status of escrow (Pending, Completed, Cancelled)

        public DateTime CreatedAt { get; set; }  // Date when escrow account was created
        public DateTime? ReleasedAt { get; set; }  // Date when funds were released to the driver
    }

    // Enum for Escrow Status
    public enum EscrowStatus
    {
        Pending,       // Funds are still held in escrow
        Completed,     // Funds have been released to the driver
        Canceled       // Funds were returned to the student (canceled trip)
    }


}
