using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Repositories.UserRepository;
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
        public IUserRepo User { get; private set; }


        public UnitOfWork(StudentPathContext _db)
        {
            this.db = _db;

            User = new UserRepo(db);

        }


        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
