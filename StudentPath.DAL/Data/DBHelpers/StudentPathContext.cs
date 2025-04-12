using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.Configurations;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Data.Models.Housing;
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
       
            modelBuilder.ApplyConfiguration(new UserDriverEntityTypeConfiguration());

            // Ignore the conflicting navigation property in UserDriver
            modelBuilder.Entity<UserDriver>()
                .Ignore(ud => ud.Driver);  // Ignore the Driver navigation property in UserDriver

            modelBuilder.Entity<Feature>().HasData(
                // Interior Features
                new Feature { Id = 1, Name = "ADSL", Category = FeatureCategory.Interior },
                new Feature { Id = 2, Name = "Alarm", Category = FeatureCategory.Interior },
                new Feature { Id = 3, Name = "Balcony", Category = FeatureCategory.Interior },
                new Feature { Id = 4, Name = "Built-in Kitchen", Category = FeatureCategory.Interior },
                new Feature { Id = 5, Name = "Barbecue", Category = FeatureCategory.Interior },
                new Feature { Id = 6, Name = "Furnished", Category = FeatureCategory.Interior },
                new Feature { Id = 7, Name = "Laundry Room", Category = FeatureCategory.Interior },
                new Feature { Id = 8, Name = "Air Conditioning", Category = FeatureCategory.Interior },
                new Feature { Id = 9, Name = "Wallpaper", Category = FeatureCategory.Interior },
                new Feature { Id = 10, Name = "Dressing Room", Category = FeatureCategory.Interior },
                new Feature { Id = 11, Name = "Video Intercom", Category = FeatureCategory.Interior },
                new Feature { Id = 12, Name = "Jacuzzi", Category = FeatureCategory.Interior },
                new Feature { Id = 13, Name = "Shower", Category = FeatureCategory.Interior },
                new Feature { Id = 14, Name = "TV Satellite", Category = FeatureCategory.Interior },
                new Feature { Id = 15, Name = "Laminate", Category = FeatureCategory.Interior },
                new Feature { Id = 16, Name = "Panel Door", Category = FeatureCategory.Interior },
                new Feature { Id = 17, Name = "Marble Floor", Category = FeatureCategory.Interior },
                new Feature { Id = 18, Name = "Blinds", Category = FeatureCategory.Interior },
                new Feature { Id = 19, Name = "Sauna", Category = FeatureCategory.Interior },
                new Feature { Id = 20, Name = "Parent Bathroom", Category = FeatureCategory.Interior },
                new Feature { Id = 21, Name = "Parquet", Category = FeatureCategory.Interior },
                new Feature { Id = 22, Name = "Satin Plaster", Category = FeatureCategory.Interior },
                new Feature { Id = 23, Name = "Satin Color", Category = FeatureCategory.Interior },
                new Feature { Id = 24, Name = "Ceramic Floor", Category = FeatureCategory.Interior },
                new Feature { Id = 25, Name = "Spotlight", Category = FeatureCategory.Interior },
                new Feature { Id = 26, Name = "Fireplace", Category = FeatureCategory.Interior },
                new Feature { Id = 27, Name = "Terrace", Category = FeatureCategory.Interior },
                new Feature { Id = 28, Name = "Cloakroom", Category = FeatureCategory.Interior },
                new Feature { Id = 29, Name = "Underfloor Heating", Category = FeatureCategory.Interior },
                new Feature { Id = 30, Name = "Double Glazing", Category = FeatureCategory.Interior },

                // Exterior Features
                new Feature { Id = 31, Name = "Elevator", Category = FeatureCategory.Exterior },
                new Feature { Id = 32, Name = "Gardened", Category = FeatureCategory.Exterior },
                new Feature { Id = 33, Name = "Fitness", Category = FeatureCategory.Exterior },
                new Feature { Id = 34, Name = "Security", Category = FeatureCategory.Exterior },
                new Feature { Id = 35, Name = "Thermal Insulation", Category = FeatureCategory.Exterior },
                new Feature { Id = 36, Name = "Generator", Category = FeatureCategory.Exterior },
                new Feature { Id = 37, Name = "Doorman", Category = FeatureCategory.Exterior },
                new Feature { Id = 38, Name = "Car Park", Category = FeatureCategory.Exterior },
                new Feature { Id = 39, Name = "Playground", Category = FeatureCategory.Exterior },
                new Feature { Id = 40, Name = "PVC", Category = FeatureCategory.Exterior },
                new Feature { Id = 41, Name = "Siding", Category = FeatureCategory.Exterior },
                new Feature { Id = 42, Name = "Water Tank", Category = FeatureCategory.Exterior },
                new Feature { Id = 43, Name = "Tennis Court", Category = FeatureCategory.Exterior },
                new Feature { Id = 44, Name = "Fire Escape", Category = FeatureCategory.Exterior },
                new Feature { Id = 45, Name = "Swimming Pool", Category = FeatureCategory.Exterior },
                new Feature { Id = 46, Name = "Football Field", Category = FeatureCategory.Exterior },
                new Feature { Id = 47, Name = "Basketball Field", Category = FeatureCategory.Exterior },
                new Feature { Id = 48, Name = "Market", Category = FeatureCategory.Exterior }
            );


        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        //public virtual DbSet<DriverStudent> DriverStudents { get; set; }
        public DbSet<UserDriver> UserDrivers { get; set; }

        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<VehicleInfo> vehicleInfos { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<EscrowAccount> EscrowAccounts { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<CustomRole> CustomRoles { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; } // Add Wallet table
        public virtual DbSet<WalletTransaction> WalletsTransactions { get; set; }


        //Housing
        public DbSet<Feature> Features { get; set; }
        public DbSet<LocationProperty> LocationProperties { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyFeature> PropertyFeatures { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }

    }
}
