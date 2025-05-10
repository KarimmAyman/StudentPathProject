using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class WalletTransaction
    {
        [Key]
        public int WalletTransactionId { get; set; }  // Unique identifier for the transaction

        [Required]
        [ForeignKey("Wallet")]
        public int WalletId { get; set; }  // Reference to the wallet from which the payment was made

        public virtual Wallet Wallet { get; set; }  // Navigation property to the wallet

        [Required]
        public decimal Amount { get; set; }  // Amount involved in the transaction (debit or credit)


        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;  // Date when the transaction occurred

        public string PaymobTransactionId { get; set; }  // Paymob Transaction ID (for tracking purposes)
        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }  // Always linked to Payment
        public virtual Payment Payment { get; set; }

    }
}
