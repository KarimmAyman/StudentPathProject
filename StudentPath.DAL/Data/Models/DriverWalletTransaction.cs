using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class DriverWalletTransaction
    {

        [Key]
        public int TransactionId { get; set; } // PK

        [ForeignKey("Driver")]
        public string DriverId { get; set; } // FK to Driver (AspNetUsers)

        public decimal Amount { get; set; } // Positive: earnings, Negative: withdrawals
        public DateTime TransactionDate { get; set; }
        public decimal BalanceAfterTransaction { get; set; } // Must match Driver.Balance after transaction applied

        public WalletTransactionOperation Operation { get; set; } // Enum: TripEarnings, Withdrawal, etc.

        public virtual Driver Driver { get; set; } // Navigation property



        public enum WalletTransactionOperation
        {
            TripEarnings = 1,
            Withdrawal = 2
        }
    }
}
