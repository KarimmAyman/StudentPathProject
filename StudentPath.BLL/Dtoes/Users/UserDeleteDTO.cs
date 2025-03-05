using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Users
{
    public class UserDeleteDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public bool IsDeleted { get; set; } = false;


        public GenderType Gender { get; set; }
    }
}
