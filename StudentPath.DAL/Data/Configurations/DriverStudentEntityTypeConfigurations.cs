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
    internal class DriverStudentEntityTypeConfigurations : IEntityTypeConfiguration<DriverStudent>
    {
        public void Configure(EntityTypeBuilder<DriverStudent> builder)
        {
            builder
            .HasKey(sd => new
            {
                sd.StudentId,
                sd.DriverId
            });

            builder
       .HasOne(sd => sd.Student)
      .WithMany(s => s.DriverStudents)
      .HasForeignKey(sd => sd.StudentId)
      .OnDelete(DeleteBehavior.Restrict); // No cascade delete for Student

            builder
                         .HasOne(sd => sd.Driver)
                        .WithMany(d => d.DriverStudents)
                        .HasForeignKey(sd => sd.DriverId)
                        .OnDelete(DeleteBehavior.Restrict); // No cascade delete for Driver


            builder
                        .Property(sd => sd.StudentId)
                        .HasMaxLength(450);  // Ensuring the length matches Users.Id

            builder
                        .Property(sd => sd.DriverId)
                        .HasMaxLength(450);  // Ensuring the length matches Users.Id
        }
    }
}
