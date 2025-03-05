using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.UserRepository
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly StudentPathContext _db;

        public UserRepo(StudentPathContext db) : base(db)
        {
            this._db = db;
        }

        public async Task SoftDeleteAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Mark the student as deleted
            user.IsDeleted = true;

            // Update the entity
            _db.Update(user);


        }
    }
}
