using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Services.AdminServices;
using StudentPath.BLL.Services.FaceVerificationService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Security.Claims;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly StudentPathContext context;
        private readonly IFaceVerificationService faceVerificationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AdminController(IAdminService adminService,StudentPathContext context,IFaceVerificationService faceVerificationService,IHttpContextAccessor httpContextAccessor)
        {
            _adminService = adminService;
            this.context = context;
            this.faceVerificationService = faceVerificationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        // 1. List all pending drivers
        [HttpGet("drivers/pending")]
        public async Task<IActionResult> GetPendingDrivers()
        {
            //var drivers = await _adminService.GetPendingDriversAsync();

            //return Ok(new
            //{
            //    successed = true,
            //    message = "Pending drivers retrieved successfully.",
            //    data = drivers  // List<DriverReadDTO>
            //});

            var baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";

            var pendingDrivers = await context.Drivers
             .Where(d => d.Status == ApprovalStatus.Pending)
             .Select(d => new DriverFaceVerficiationDTO
             {

                 Id=d.Id,
                 IdFrontPhotoUrl = $"{baseUrl}/{d.IdFrontPath.Replace("\\", "/").TrimStart('/')}",
                 PersonalPhotoUrl = $"{baseUrl}/{d.ImgUrl.Replace("\\", "/").TrimStart('/')}",
                 Status = (ApprovalStatus)d.Status
             })
             .ToListAsync();

            var filteredDrivers = new List<DriverReadDTO>();

            foreach (var driver in pendingDrivers)
            {
                // Validate photo URLs
                if (string.IsNullOrEmpty(driver.IdFrontPhotoUrl) || string.IsNullOrEmpty(driver.PersonalPhotoUrl))
                {
                    var dbDriver = await context.Drivers.FindAsync(driver.Id);
                    if (dbDriver != null)
                    {
                        dbDriver.Status = ApprovalStatus.Denied;
                    }
                    continue;
                }

                // Call AI face verification API

                bool isSamePerson = await faceVerificationService.VerifyFacesAsync(
                    driver.IdFrontPhotoUrl,
                    driver.PersonalPhotoUrl
                );

                var driverToUpdate = await context.Drivers.FindAsync(driver.Id);
                if (driverToUpdate != null)
                {
                    if (isSamePerson)
                    {
                        // Move to next stage if faces match
                        driverToUpdate.Status = ApprovalStatus.NextStage;
                    }
                    else
                    {
                        // Deny driver if faces don't match
                        driverToUpdate.Status = ApprovalStatus.Denied;
                    }
                }
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                succeeded = true,
                message = "Pending drivers processed successfully.",
                data = filteredDrivers // Return only drivers who passed verification
            });
        }
   

        // 2. Approve a driver
        [HttpPut("drivers/{id}/approve")]
        public async Task<IActionResult> ApproveDriver(string id)
        {
            var ok = await _adminService.ApproveDriverAsync(id);
            if (!ok)
                return NotFound(new
                {
                    successed = false,
                    errors = new[] { "Driver not found or could not be approved." }
                });

            return Ok(new
            {
                successed = true,
                message = "Driver approved successfully."
            });
        }

        // 3. Deny a driver
        [HttpPut("drivers/{id}/deny")]
        public async Task<IActionResult> DenyDriver(string id)
        {
            var ok = await _adminService.DenyDriverAsync(id);
            if (!ok)
                return NotFound(new
                {
                    successed = false,
                    errors = new[] { "Driver not found or could not be denied." }
                });

            return Ok(new
            {
                successed = true,
                message = "Driver denied successfully."
            });
        }

        // 4. Ban a user
        [HttpPut("users/{id}/ban")]
        public async Task<IActionResult> BanUser(string id)
        {
            var ok = await _adminService.BanUserAsync(id);
            if (!ok)
                return NotFound(new
                {
                    successed = false,
                    errors = new[] { "User not found or could not be banned." }
                });

            return Ok(new
            {
                successed = true,
                message = "User banned successfully."
            });
        }

        // 5. Unban a user
        [HttpPut("users/{id}/unban")]
        public async Task<IActionResult> UnbanUser(string id)
        {
            var ok = await _adminService.UnbanUserAsync(id);
            if (!ok)
                return NotFound(new
                {
                    successed = false,
                    errors = new[] { "User not found or could not be unbanned." }
                });

            return Ok(new
            {
                successed = true,
                message = "User unbanned successfully."
            });
        }
    }
}
