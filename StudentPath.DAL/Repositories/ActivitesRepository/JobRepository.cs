using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.ActivitesRepository
{
    public class JobRepository :IJobRepository
    {
        private readonly StudentPathContext _context;

        public JobRepository(StudentPathContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .Where(j => !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
        }

        public async Task AddAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
        }

        public void Update(Job job)
        {
            _context.Jobs.Update(job);
        }

        public void SoftDelete(Job job)
        {
            job.IsDeleted = true;
            _context.Jobs.Update(job);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
