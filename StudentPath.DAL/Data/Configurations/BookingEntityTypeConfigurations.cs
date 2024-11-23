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
    public class BookingEntityTypeConfigurations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder
        .HasOne(b => b.User)  // Navigating to the User (Student/Driver/Admin)
        .WithMany()  // The User can have many Bookings
        .HasForeignKey(b => b.UserId)
        .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete, restrict it
        } }
}
