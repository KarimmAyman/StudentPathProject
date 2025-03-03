using AutoMapper;
using StudentPath.BLL.Dtoes.Students;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.AutoMappers.StudentMapper
{
    public class StudentProfile : Profile
    {

        public StudentProfile()
        {
            CreateMap<Student, StudentReadDTO>().ReverseMap();
            CreateMap<Student, StudentAddDTO>().ReverseMap();
            CreateMap<Student, StudentUpdatedDTO>().ReverseMap();
            CreateMap<Student, StudentDetailsDTO>().ReverseMap();
            CreateMap<Student, StudentDeleteDTO>().ReverseMap();

            


        }
    }
}
