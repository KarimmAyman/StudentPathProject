using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Activities;
using StudentPath.BLL.Services.ActivityService;
using System.Security.Claims;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // GET: api/jobs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(new { successed = true, data = jobs });
        }

        // GET: api/jobs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound(new { successed = false, errors = new[] { "Job not found." } });
            }
            return Ok(new { successed = true, data = job });
        }

        // POST: api/jobs
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] JobCreateDto createDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { successed = false, errors = new[] { "User ID not found in token." } });
            }

            Console.WriteLine($"✅ Extracted User ID: {userId}");  // Debugging

            createDto.CreatedByUserId = userId;  // Assign User ID

            var createdJob = await _jobService.CreateJobAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = createdJob.Id }, new { successed = true, data = createdJob });
        }

        // PUT: api/jobs/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] JobUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest(new { successed = false, errors = new[] { "Mismatched job ID." } });
            }

            var updatedJob = await _jobService.UpdateJobAsync(updateDto);
            if (updatedJob == null)
            {
                return NotFound(new { successed = false, errors = new[] { "Job not found." } });
            }
            return Ok(new { successed = true, data = updatedJob });
        }

        // DELETE: api/jobs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _jobService.DeleteJobAsync(id);
            if (!success)
            {
                return NotFound(new { successed = false, errors = new[] { "Job not found." } });
            }
            return Ok(new { successed = true, message = "Job deleted successfully." });
        }
    }
}
