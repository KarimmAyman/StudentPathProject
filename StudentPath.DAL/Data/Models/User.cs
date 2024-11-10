using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class User
    {
        public int UserId { get; set; }  // Primary key

        [Required]
        public string FullName { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }

        [Required]
        public string ID { get; set; }  // National ID 

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }

        public UserRole Role { get; set; }  // Enum for 'User' or 'Driver'

        // Driver Specific Fields
        public VehicleInfo VehicleInfo { get; set; }  // Optional for drivers
    }

    public enum UserRole
    {
        User,
        Driver
    }
}
