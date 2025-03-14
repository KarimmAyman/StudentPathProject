using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes
{
    public class DriverAddDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public UserTypeEnum UserType { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public string DrivingLicense { get; set; } = string.Empty;

        public List<VehicleInfoDto> VehicleInfo { get; set; } = new List<VehicleInfoDto>();

        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
    }
}
