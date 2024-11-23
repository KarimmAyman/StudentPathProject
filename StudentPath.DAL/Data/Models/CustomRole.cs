using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class CustomRole : IdentityRole
    {
        public bool IsDeleted { get; set; } = false;


    }
}
