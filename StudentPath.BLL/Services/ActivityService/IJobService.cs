using StudentPath.BLL.Dtoes.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.ActivityService
{
    public interface IJobService
    {
        Task<IEnumerable<JobResponseDto>> GetAllJobsAsync();
        Task<JobResponseDto?> GetJobByIdAsync(int id);
        Task<JobResponseDto> CreateJobAsync(JobCreateDto jobDto);
        Task<bool> UpdateJobAsync(JobUpdateDto jobDto);
        Task<bool> DeleteJobAsync(int id);
    }
}
