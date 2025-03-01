using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.Configurations;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.DBHelpers
{
    public class StudentPathContext : IdentityDbContext<User>
    {
        public StudentPathContext(DbContextOptions<StudentPathContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies(true);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfigurations());
            modelBuilder.ApplyConfiguration(new BookingEntityTypeConfigurations());
            modelBuilder.ApplyConfiguration(new DriverStudentEntityTypeConfigurations());




        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<DriverStudent> DriverStudents { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<VehicleInfo> vehicleInfos { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<EscrowAccount> EscrowAccounts { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<CustomRole> CustomRoles { get; set; }

    }
}
