using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPath.BLL.Dtoes
{
    public class DriverUpdateDTO
    {
        [NotMapped]
        [FromForm(Name = "ImgUrlFile")]
        public IFormFile? ImgUrlFile { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? UserName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
