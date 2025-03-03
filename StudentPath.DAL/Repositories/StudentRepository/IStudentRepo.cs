using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.StudentRepository
{
    public interface IStudentRepo : IGenericRepo<Student>
    {
        public Task SoftDeleteAsync(Student student);


    }
}
