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
    public class TripEntityTypeConfigurations : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.TripId);

            builder.Property(t => t.TripId)
                .UseIdentityColumn();

            builder.Property(t => t.DepartureTime)
                .IsRequired();

            builder.Property(t => t.AvailableSeats)
                .IsRequired();

            builder.Property(t => t.PricePerSeat)
                .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Driver)
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Amenities defaults
            builder.Property(t => t.HasWiFi).HasDefaultValue(false);
            builder.Property(t => t.HasPhoneCharger).HasDefaultValue(false);
            builder.Property(t => t.HasAirConditioning).HasDefaultValue(false);
            builder.Property(t => t.HasFreeWater).HasDefaultValue(false);
            builder.Property(t => t.HasMusic).HasDefaultValue(false);
        }
    }
}
