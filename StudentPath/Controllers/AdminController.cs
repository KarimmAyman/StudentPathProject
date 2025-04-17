using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Services.AdminServices;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // 1. List all pending drivers
        [HttpGet("drivers/pending")]
        public async Task<IActionResult> GetPendingDrivers()
        {
            var drivers = await _adminService.GetPendingDriversAsync();

            return Ok(new
            {
                successed = true,
                message = "Pending drivers retrieved successfully.",
                data = drivers  // List<DriverReadDTO>
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
