using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.Dtoes
{
    public class DriverDetailsDTO
    {
        public string Id { get; set; } = string.Empty; // IdentityUser ID
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public GenderType Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public string DrivingLicense { get; set; } = string.Empty;
        public ApprovalStatus? Status { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<VehicleInfoDto> VehicleInfo { get; set; } = new List<VehicleInfoDto>();
        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
    }
}
