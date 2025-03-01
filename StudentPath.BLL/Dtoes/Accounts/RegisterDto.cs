
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtos.Accounts
{
    public class RegisterDto
    {
       
            [Required(ErrorMessage = "Username is required")]
            [StringLength(80, ErrorMessage = "Username must be between 5 and 80 characters", MinimumLength = 5)]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "Invalid Email Format")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 6)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Password confirmation is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
            public string ConfirmedPassword { get; set; }


        [DataType(DataType.ImageUrl)]
        public string? ImgUrl { get; set; }


        [Required(ErrorMessage = "UserType is required")]
            public UserTypeEnum UserType { get; set; } = UserTypeEnum.User;

            [Required(ErrorMessage = "Age is required")]
            [Range(5, 120)]
            public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public GenderType Gender { get; set; } 

     

        public string? PhoneNumber { get; set; }


        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        // Driver-Specific Properties
        public string? DrivingLicense { get; set; }

        // Vehicle Information (Only for Drivers)
        public List<VehicleInfoDto>? Vehicleinfo{ get; set; }
      

        
        public List<LocationDto>? locations { get; set; }

    }



}