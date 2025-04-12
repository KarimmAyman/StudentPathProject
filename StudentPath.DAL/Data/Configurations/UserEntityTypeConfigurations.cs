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

    public class UserEntityTypeConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
                builder
               .HasIndex(u => u.PhoneNumber)
              .IsUnique();

       
                builder.Property(u => u.PhoneNumber)
                .HasMaxLength(14) 
                .IsRequired();
            //Configure primary key
            //builder.HasKey(u => u.Id);

            //Configure one-to - one relationship with Location
            //builder.HasOne(u => u.Location)
            //    .WithOne(l => l.User)
            //    .HasForeignKey<Location>(l => l.UserId)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent circular dependency

            //Table name configuration(optional)
            //builder.ToTable("Users");
        }
    }
}
