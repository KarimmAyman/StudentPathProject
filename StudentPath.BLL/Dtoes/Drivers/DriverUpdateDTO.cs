using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes
{
    public class DriverUpdateDTO
    {
        // Basic Info
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // ID Documents (as IFormFile - null means don't update)
        public IFormFile? IdFront { get; set; }
        public IFormFile? IdBack { get; set; }
        public IFormFile? CriminalRecord { get; set; }
        public string? IdNumber { get; set; }

        // License Documents
        public IFormFile? LicenseFront { get; set; }
        public IFormFile? LicenseBack { get; set; }
        public IFormFile? LicenseSelfie { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }

        //// Locations
        public List<LocationDto>? Locations { get; set; } = new List<LocationDto>();
    }

}
