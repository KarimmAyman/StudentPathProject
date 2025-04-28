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
    public class TripRequestEntityTypeConfigurations:IEntityTypeConfiguration<TripRequest>
    {
        public void Configure(EntityTypeBuilder<TripRequest> builder)
        {
           builder
          .HasOne(tr => tr.FromLocation)
          .WithMany(tl=>tl.TripsRequestAsFrom)
          .HasForeignKey(tr => tr.FromLocationId)
          .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(tr => tr.ToLocation)
                .WithMany(tl => tl.TripsRequestAsTo)
                .HasForeignKey(tr => tr.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

