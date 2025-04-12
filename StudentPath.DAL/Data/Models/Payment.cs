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
        Wallet
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; }

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }

        public string? PaymentIntentId { get; set; } // Securely reference Stripe PaymentIntent    }
      }
       


    }

