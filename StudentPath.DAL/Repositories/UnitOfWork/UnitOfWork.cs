using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Repositories.StudentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentPathContext db;
        public IStudentRepo Student { get; private set; }


        public UnitOfWork(StudentPathContext _db)
        {
            this.db = _db;

            Student = new StudentRepo(db);

        }


        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
