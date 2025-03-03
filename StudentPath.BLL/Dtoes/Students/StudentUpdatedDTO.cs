using Microsoft.AspNetCore.Http;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Students
{
    public class StudentUpdatedDTO
    {

        public string Id { get; set; }
        [Required]
        [StringLength(100)]

        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter your phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid phone number format.")]

        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "The Range of age between 18 and 100")]

        [Range(18, 100)]
        public int Age { get; set; }


        
        [Required(ErrorMessage = "Gender is required")]
        public GenderType Gender { get; set; }



        public string? ImgUrl { get; set; }
        [JsonIgnore]

        public IFormFile? ProfileImage { get; set; }

    }
}
