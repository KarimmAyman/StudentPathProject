using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.Dtoes
{
    public class DriverDetailsDTO : DriverReadDTO
    {
       /* public string Id { get; set; } = string.Empty;*/ // IdentityUser ID
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public GenderType Gender { get; set; }
        public string? PersonalPhotoUrl { get; set; }

        // Personal Documents
        public string NationalIdFrontPath { get; set; } = string.Empty;
        public string NationalIdBackPath { get; set; } = string.Empty;
        public string CriminalStatusRecordPath { get; set; } = string.Empty;

        // Driver License Documents
        public string LicenseFrontPath { get; set; } = string.Empty;
        public string LicenseBackPath { get; set; } = string.Empty;
        public string SelfieWithLicensePath { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpirationDate { get; set; }

        public ApprovalStatus? Status { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<VehicleReadDTO> VehicleInfo { get; set; } = new List<VehicleReadDTO>();
        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
    }
}
