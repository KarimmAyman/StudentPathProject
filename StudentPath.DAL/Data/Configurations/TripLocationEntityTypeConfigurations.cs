using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.Models;

public class TripLocationEntityTypeConfigurations : IEntityTypeConfiguration<TripLocation>
{
    public void Configure(EntityTypeBuilder<TripLocation> builder)
    {
        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.DisplayName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(tl => tl.FullAddress)
               .IsRequired()
               .HasMaxLength(250);

        builder.HasIndex(tl => new { tl.Latitude, tl.Longitude });
    }
}