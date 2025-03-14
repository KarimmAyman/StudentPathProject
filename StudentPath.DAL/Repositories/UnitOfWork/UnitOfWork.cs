using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Repositories.DriverRepository;
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
        public IDriverRepo Driver { get; private set; }  // Add Driver repository



        public UnitOfWork(StudentPathContext _db)
        {
            this.db = _db;

            User = new UserRepo(db);
            Driver = new DriverRepo(db);  // Add Driver repository

        }


        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
