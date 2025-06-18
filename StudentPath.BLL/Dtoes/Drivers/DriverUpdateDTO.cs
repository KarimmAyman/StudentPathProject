using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes
{
    public class DriverUpdateDTO
    {

        // Basic Info Only (Name, Phone, Photo)
        public IFormFile? PersonalPhoto { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? UserName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
