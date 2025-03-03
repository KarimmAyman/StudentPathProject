using StudentPath.DAL.Repositories.StudentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {

        public IStudentRepo Student { get; }
        public Task Save();

    }
}
