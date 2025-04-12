using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    
        public class Wallet
        {
            [Key]
            public int WalletId { get; set; }

            [ForeignKey("User")]
            public string UserId { get; set; }
            public virtual User User { get; set; }

            [Range(0, double.MaxValue)]
            public decimal Balance { get; set; } = 0;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public string LastTransactionId { get; set; } // Stores lasttransactionid for wallet funding tracking
        }
    }

