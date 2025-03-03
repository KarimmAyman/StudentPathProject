using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Students
{
    public class StudentDetailsDTO
    {
        public string UserName { get; set; }
        [JsonIgnore]


        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        [JsonIgnore]

        public string ImgUrl { get; set; }
        public GenderType Gender { get; set; }

        public int Age { get; set; }
    }
}
