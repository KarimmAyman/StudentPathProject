using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Configurations
{
    public class WalletTransactionsEntityTypeConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder
                .HasOne(wt => wt.Payment)
                .WithMany(p => p.WalletTransactions)
                .HasForeignKey(wt => wt.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
