using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public enum UserTypeEnum
    {
        Student = 1,
        Driver = 2,
        Admin = 3

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

        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ImgUrl { get; set; } // Optional profile picture
        public string SSN { get; set; }

        public UserTypeEnum UserType { get; set; }

        public bool IsBanned { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }
    // Student subclass
    public class Student : User
    {
        public string Grade { get; set; }
        // Relationship with a driver (if needed, e.g., for student transportation)
        public virtual ICollection<DriverStudent> DriverStudents { get; set; } = new HashSet<DriverStudent>();

    }

    // Driver subclass
    public class Driver : User
    {
        public ApprovalStatus? Status { get; set; } = ApprovalStatus.Pending;
        public string DrivingLicense { get; set; }
        public virtual ICollection<VehicleInfo> VehicleInfo { get; set; } = new HashSet<VehicleInfo>(); // Associated Vehicle
                                                                                                   // Relationship with students (if needed)
        public virtual ICollection<DriverStudent> DriverStudents { get; set; } = new HashSet<DriverStudent>();
    }
    public class Admin : User
    {
    }
}
