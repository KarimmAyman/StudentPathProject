using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.DriverRepository
{
    public class DriverRepo : GenericRepo<Driver>, IDriverRepo
    {
        private readonly StudentPathContext _db;

        public DriverRepo(StudentPathContext db) : base(db)
        {
            this._db = db;
        }

        public async Task SoftDeleteAsync(Driver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));

            // Mark the driver as deleted
            driver.IsDeleted = true;

            // Update the entity
            _db.Update(driver);
        }

    }
}
