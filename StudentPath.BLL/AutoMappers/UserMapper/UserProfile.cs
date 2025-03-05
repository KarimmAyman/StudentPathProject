using AutoMapper;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.AutoMappers.UserMapper
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            CreateMap<User, UserReadDTO>().ReverseMap();
            CreateMap<User, UserAddDTO>().ReverseMap();
            CreateMap<User, UserUpdatedDTO>().ReverseMap();
            CreateMap<User, UserDetailsDTO>().ReverseMap();
            CreateMap<User, UserDeleteDTO>().ReverseMap();

            


        }
    }
}
