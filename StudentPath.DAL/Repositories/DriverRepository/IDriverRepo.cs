using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.DriverRepository
{
    public interface IDriverRepo : IGenericRepo<Driver>
    {
        Task SoftDeleteAsync(Driver driver);
    }
}
