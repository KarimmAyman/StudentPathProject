
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [SwaggerIgnore]
        
        public string? ImgUrl { get; set; }
        [NotMapped]
        [FromForm(Name = "ImgUrlFile")]
        public IFormFile? ImgUrlFile { get; set; }



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
        // Add these in RegisterDto (under Driver-specific section)
        public IFormFile? IdFront { get; set; }
        public IFormFile? IdBack { get; set; }
        public IFormFile? CriminalRecord { get; set; }

        public IFormFile? LicenseFront { get; set; }
        public IFormFile? LicenseBack { get; set; }
        public IFormFile? LicenseSelfie { get; set; }

        public string? IdNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }

        // Vehicle Information (Only for Drivers)
        [SwaggerIgnore]
        public List<VehicleInfoDto>? Vehicleinfo{ get; set; }


        [SwaggerIgnore]
        public List<LocationDto>? locations { get; set; }

    }



}