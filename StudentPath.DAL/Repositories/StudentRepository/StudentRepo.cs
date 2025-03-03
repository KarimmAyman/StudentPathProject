using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.StudentRepository
{
    public class StudentRepo : GenericRepo<Student>, IStudentRepo
    {
        private readonly StudentPathContext _db;

        public StudentRepo(StudentPathContext db) : base(db)
        {
            this._db = db;
        }

        public async Task SoftDeleteAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            // Mark the student as deleted
            student.IsDeleted = true;

            // Update the entity
            _db.Update(student);


        }
    }
}
