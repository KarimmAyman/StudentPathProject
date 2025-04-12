using StudentPath.DAL.Data.Models.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.ActivitesRepository
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllAsync();
        Task<Job?> GetByIdAsync(int id);
        Task AddAsync(Job job);
        void Update(Job job);
        void SoftDelete(Job job);
        Task<bool> SaveChangesAsync();
    }
}
