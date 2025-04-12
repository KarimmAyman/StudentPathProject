using StudentPath.BLL.Dtoes.Activities;
using StudentPath.DAL.Data.Models.Activities;
using StudentPath.DAL.Repositories.ActivitesRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.ActivityService
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<JobResponseDto>> GetAllJobsAsync()
        {
            var jobs = await _jobRepository.GetAllAsync();
            return jobs.Select(j => new JobResponseDto
            {
                Id = j.Id,
                Title = j.Title,
                ContractType = j.ContractType,
                CompanyName = j.CompanyName,
                CompanyWebsite = j.CompanyWebsite,
                CompanyPhone = j.CompanyPhone,
                CompanyEmail = j.CompanyEmail,
                Location = j.Location,
                MinSalary = j.MinSalary,
                MaxSalary = j.MaxSalary,
                SalaryPeriod = j.SalaryPeriod,
                Description = j.Description,
                Responsibilities = j.Responsibilities,
                PostedDate = j.PostedDate,
                ExpiryDate = j.ExpiryDate,
                Experience = j.Experience,
                Category = j.Category,
                JobType = j.JobType,
                IsActive = j.IsActive,
                DaysRemaining = j.DaysRemaining
            });
        }

        public async Task<JobResponseDto?> GetJobByIdAsync(int id)
        {
            var j = await _jobRepository.GetByIdAsync(id);
            if (j == null) return null;

            return new JobResponseDto
            {
                Id = j.Id,
                Title = j.Title,
                ContractType = j.ContractType,
                CompanyName = j.CompanyName,
                CompanyWebsite = j.CompanyWebsite,
                CompanyPhone = j.CompanyPhone,
                CompanyEmail = j.CompanyEmail,
                Location = j.Location,
                MinSalary = j.MinSalary,
                MaxSalary = j.MaxSalary,
                SalaryPeriod = j.SalaryPeriod,
                Description = j.Description,
                Responsibilities = j.Responsibilities,
                PostedDate = j.PostedDate,
                ExpiryDate = j.ExpiryDate,
                Experience = j.Experience,
                Category = j.Category,
                JobType = j.JobType,
                IsActive = j.IsActive,
                DaysRemaining = j.DaysRemaining
            };
        }

        public async Task<JobResponseDto> CreateJobAsync(JobCreateDto jobDto)
        {
            // Map DTO to entity
            var job = new Job
            {
                Title = jobDto.Title,
                ContractType = jobDto.ContractType,
                CompanyName = jobDto.CompanyName,
                CompanyWebsite = jobDto.CompanyWebsite,
                CompanyPhone = jobDto.CompanyPhone,
                CompanyEmail = jobDto.CompanyEmail,
                Location = jobDto.Location,
                MinSalary = jobDto.MinSalary,
                MaxSalary = jobDto.MaxSalary,
                SalaryPeriod = jobDto.SalaryPeriod,
                Description = jobDto.Description,
                Responsibilities = jobDto.Responsibilities,
                PostedDate = DateTime.UtcNow,
                ExpiryDate = jobDto.ExpiryDate,
                Experience = jobDto.Experience,
                Category = jobDto.Category,
                JobType = jobDto.JobType,
                IsActive = true
            };

            // Optionally, set CreatedByUserId from DTO or via current context
            job.CreatedByUserId = jobDto.CreatedByUserId;

            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            // Map entity to response DTO
            return new JobResponseDto
            {
                Id = job.Id,
                Title = job.Title,
                ContractType = job.ContractType,
                CompanyName = job.CompanyName,
                CompanyWebsite = job.CompanyWebsite,
                CompanyPhone = job.CompanyPhone,
                CompanyEmail = job.CompanyEmail,
                Location = job.Location,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SalaryPeriod = job.SalaryPeriod,
                Description = job.Description,
                Responsibilities = job.Responsibilities,
                PostedDate = job.PostedDate,
                ExpiryDate = job.ExpiryDate,
                Experience = job.Experience,
                Category = job.Category,
                JobType = job.JobType,
                IsActive = job.IsActive,
                DaysRemaining = job.DaysRemaining
            };
        }

        public async Task<bool> UpdateJobAsync(JobUpdateDto jobDto)
        {
            var job = await _jobRepository.GetByIdAsync(jobDto.Id);
            if (job == null) return false;

            // Map the updates from the DTO
            job.Title = jobDto.Title;
            job.ContractType = jobDto.ContractType;
            job.CompanyName = jobDto.CompanyName;
            job.CompanyWebsite = jobDto.CompanyWebsite;
            job.CompanyPhone = jobDto.CompanyPhone;
            job.CompanyEmail = jobDto.CompanyEmail;
            job.Location = jobDto.Location;
            job.MinSalary = jobDto.MinSalary;
            job.MaxSalary = jobDto.MaxSalary;
            job.SalaryPeriod = jobDto.SalaryPeriod;
            job.Description = jobDto.Description;
            job.Responsibilities = jobDto.Responsibilities;
            job.ExpiryDate = jobDto.ExpiryDate;
            job.Experience = jobDto.Experience;
            job.Category = jobDto.Category;
            job.JobType = jobDto.JobType;

            _jobRepository.Update(job);
            return await _jobRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null) return false;

            _jobRepository.SoftDelete(job);
            return await _jobRepository.SaveChangesAsync();
        }
    }
}
