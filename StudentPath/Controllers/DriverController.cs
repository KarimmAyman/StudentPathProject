using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Services.DriverServices;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet("GetAllDrivers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DriverReadDTO>>>> GetAllDrivers()
        {
            var drivers = await _driverService.GetAllDriversAsync();
            return Ok(ApiResponse<IEnumerable<DriverReadDTO>>.SuccessResponse("Drivers retrieved successfully", 200, drivers));
        }

        [HttpGet("ById/{id}")]
        public async Task<ActionResult<ApiResponse<DriverDetailsDTO>>> GetDriverById(string id)
        {
            var driver = await _driverService.GetDriverByIdAsync(id);
            if (driver == null)
            {
                return NotFound(ApiResponse<DriverDetailsDTO>.ErrorResponse("Driver not found", 404));
            }
            return Ok(ApiResponse<DriverDetailsDTO>.SuccessResponse("Driver retrieved successfully", 200, driver));
        }

        [HttpPost("AddDriver")]
        public async Task<ActionResult<ApiResponse<DriverReadDTO>>> AddDriver([FromForm] DriverAddDTO driverDto, [FromForm] string vehicleInfoJson, [FromForm] string locationsJson)
        {
            Console.WriteLine($"vehicleInfoJson: {vehicleInfoJson}");

            // Deserialize the JSON arrays
            if (!string.IsNullOrEmpty(vehicleInfoJson))
            {
                driverDto.VehicleAddDTOs = JsonSerializer.Deserialize<List<VehicleAddDTO>>(vehicleInfoJson);
            }

            if (!string.IsNullOrEmpty(locationsJson))
            {
                driverDto.Locations = JsonSerializer.Deserialize<List<LocationDto>>(locationsJson);
            }
            var createdDriver = await _driverService.CreateDriverAsync(driverDto);
            return CreatedAtAction(nameof(GetDriverById), new { id = createdDriver.Id },
                ApiResponse<DriverReadDTO>.SuccessResponse("Driver created successfully", 201, createdDriver));
        }

        [HttpPut("EditDriver/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> EditDriver(string id, [FromForm] DriverUpdateDTO driverDto)
        {
            bool updated = await _driverService.UpdateDriverAsync(id, driverDto);
            if (!updated)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Driver not found", 404));
            }
            return Ok(ApiResponse<string>.SuccessResponse("Driver updated successfully", 200));
        }

        [HttpDelete("DeleteDriver/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteDriver(string id)
        {
            bool deleted = await _driverService.SoftDeleteDriverAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Driver not found", 404));
            }
            return Ok(ApiResponse<string>.SuccessResponse("Driver deleted successfully", 200));
        }
    }
}
