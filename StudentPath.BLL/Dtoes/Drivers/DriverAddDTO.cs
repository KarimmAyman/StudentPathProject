using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPath.BLL.Dtoes
{
    public class DriverAddDTO
    {
        // Personal Info
        public IFormFile? PersonalPhoto { get; set; }

        //public string? PersonalPhotoPath { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }

        // ID Documents
        public IFormFile IdFront { get; set; }
        //public string IdFront { get; set; }
        public IFormFile IdBack { get; set; }
        public IFormFile CriminalRecord { get; set; }
        public string IdNumber { get; set; }

        // License Documents
        public IFormFile LicenseFront { get; set; }
        public IFormFile LicenseBack { get; set; }
        public IFormFile LicenseSelfie { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }

        public List<VehicleAddDTO> VehicleAddDTOs { get; set; } = new List<VehicleAddDTO>();

        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();

        public string Id { get; internal set; }
    }

  

}
