using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.Models;

public class UserDriverEntityTypeConfiguration : IEntityTypeConfiguration<UserDriver>
{
    public void Configure(EntityTypeBuilder<UserDriver> builder)
    {
        // Composite Key
        builder.HasKey(ud => new { ud.UserId, ud.DriverId });

        // Configuring the relationship between User and UserDriver
        builder
            .HasOne(ud => ud.User)
            .WithMany(u => u.UserDrivers)
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Restrict delete to avoid cascading

        // If you want to configure Driver, do it separately outside the navigation
        builder
            .HasOne(ud => ud.Driver)
            .WithMany()
            .HasForeignKey(ud => ud.DriverId)
            .OnDelete(DeleteBehavior.Restrict); // Restrict delete to avoid cascading

        // Ensuring the length of UserId and DriverId is set correctly
        builder
            .Property(ud => ud.UserId)
            .HasMaxLength(450);  // This should match the length of IdentityUser.Id

        builder
            .Property(ud => ud.DriverId)
            .HasMaxLength(450);  // This should match the length of IdentityUser.Id
    }
}
