using Microsoft.AspNetCore.Identity;
using StudentPath.DAL.Data.Models.Activities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public enum UserTypeEnum
    {
         User,
        Student = 1,
        Driver = 2,
        Admin = 3 ,
        

    }
    public enum GenderType
    {
        Male = 1,
        Female = 2

    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Denied
    }

    public class User : IdentityUser
    {

        [Range(18, 100)]
        public int Age { get; set; }

        public GenderType Gender { get; set; }


        [DataType(DataType.ImageUrl)]
        public string? ImgUrl { get; set; } // Optional profile picture

        public UserTypeEnum UserType { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RegistrationDate { get; set; } 
        public string? PhoneNumber { get; set; }

   
        public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();  // Navigation Property
        public bool IsBanned { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public string? OtpCode { get; set; } // Stores the OTP
        public DateTime? OtpExpiry { get; set; } // Stores the OTP expiration time
        public bool IsOtpVerified { get; set; } = false; // ✅ New Property to track OTP verification

        public virtual ICollection<UserDriver> UserDrivers { get; set; } = new HashSet<UserDriver>();
        // Stripe-related fields
        public string? StripeCustomerId { get; set; } // Stores Stripe customer ID

        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual Wallet? Wallet { get; set; } // User's in-app wallet
        public string? DefaultPaymentMethodId { get; set; } // Store saved PaymentMethodId

        public virtual ICollection<Job> CreatedJobs { get; set; } = new HashSet<Job>();
        public bool? IsActive { get; set; } = true; // Accepts null, default to true
        public bool? CanReceiveTripRequests { get; set; } = false; // Accepts null, default to false

    }
    // Student subclass
    public class Student : User
    {
       
     

    }

    // Driver subclass
    public class Driver : User
    {
        public int Age { get; set; }
        public string IdNumber { get; set; }
        public string IdFrontPath { get; set; }
        public string IdBackPath { get; set; }
        public string CriminalRecordPath { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string LicenseFrontPath { get; set; }
        public string LicenseBackPath { get; set; }
        public string LicenseSelfiePath { get; set; }
        public string VehiclePicturePath { get; set; }
        public string VehicleRegistrationFrontPath { get; set; }
        public string VehicleRegistrationBackPath { get; set; }
        public ApprovalStatus? Status { get; set; } = ApprovalStatus.Pending;
        public string DrivingLicense { get; set; }

        public decimal? Balance { get; set; } = 0;
        public virtual ICollection<VehicleInfo> VehicleInfo { get; set; } = new HashSet<VehicleInfo>(); // Associated Vehicle
                                                                                                        // Relationship with students (if needed)
        public virtual ICollection<UserDriver> UserDrivers { get; set; } = new HashSet<UserDriver>();

        public virtual ICollection<DriverWalletTransaction> DriverWalletTransactions { get; set; } = new HashSet<DriverWalletTransaction>();

    }
    public class Admin : User
    {
    }
}
