using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Configurations
{
    public class PaymentEntityTypeConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder
            .HasOne(p => p.Booking)
    .WithMany(b => b.Payments)
    .HasForeignKey(p => p.BookingId)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
