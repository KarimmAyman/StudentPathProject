using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.DBHelpers
{
    public class StudentPathContext : DbContext
    {
        public StudentPathContext(DbContextOptions<StudentPathContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<VehicleInfo> vehicleInfos { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<EscrowAccount> EscrowAccounts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
