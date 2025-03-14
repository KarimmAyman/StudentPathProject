using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes
{
    public class DriverUpdateDTO
    {

        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(18, 100)]
        public int? Age { get; set; }

        public GenderType? Gender { get; set; }

        public string? Address { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "SSN must be 14 digits.")]
        public string? SSN { get; set; }

        public string? DrivingLicense { get; set; }

        public ApprovalStatus? Status { get; set; }

        public List<VehicleInfoDto> VehicleInfo { get; set; } = new List<VehicleInfoDto>();

        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
    }
}
