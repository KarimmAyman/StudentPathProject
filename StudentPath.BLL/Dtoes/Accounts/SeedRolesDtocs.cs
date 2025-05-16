using Microsoft.AspNetCore.Identity;

using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtos.Accounts
{
    public class SeedRolesDtocs
    {
        public static async Task SeedRoles(RoleManager<CustomRole> roleManager)
        {
            var roles = new List<string> {Roles.Admin, Roles.Driver
                , Roles.Student,Roles.User};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new CustomRole { Name = role  });
                }
            }
        }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Driver = "Driver";
        public const string Student = "Student";
        public const string User = "User";

    }
}
