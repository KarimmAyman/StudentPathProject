using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Identity;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Services.DriverServices;
using StudentPath.BLL.Services.TripServices;
using StudentPath.DAL.Data.DBHelpers;
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
        private readonly StudentPathContext context;
        private readonly ITripService _tripService;

        public DriverController(IDriverService driverService,StudentPathContext context, ITripService tripService)
        {
            _driverService = driverService;
            this.context = context;
            _tripService = tripService;
        }

        [HttpGet("GetAllDrivers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DriverReadDTO>>>> GetAllDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAllDriversAsync();
                return ApiResponse<IEnumerable<DriverReadDTO>>.SuccessResponse(
                    "Drivers retrieved successfully",
                    200,
                    drivers);
            }
            catch (Exception)
            {
                return ApiResponse<IEnumerable<DriverReadDTO>>.ErrorResponse(
                    "An error occurred while retrieving drivers",
                    500);
            }
        }

        [HttpGet("ById/{id}")]
        public async Task<ActionResult<ApiResponse<DriverDetailsDTO>>> GetDriverById(string id)
        {
            try
            {
                var driver = await _driverService.GetDriverByIdAsync(id);
                if (driver == null)
                {
                    return ApiResponse<DriverDetailsDTO>.ErrorResponse(
                        "Driver not found",
                        404);
                }
                return ApiResponse<DriverDetailsDTO>.SuccessResponse(
                    "Driver retrieved successfully",
                    200,
                    driver);
            }
            catch (Exception)
            {
                return ApiResponse<DriverDetailsDTO>.ErrorResponse(
                    "An error occurred while retrieving the driver",
                    500);
            }
        }

        [HttpGet("{driverId}/trip")]
        public async Task<IActionResult> GetDriverTrip(string driverId)
        {
            var response = await _tripService.GetDriverTripDetailsAsync(driverId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddDriver")]
        public async Task<ActionResult<ApiResponse<DriverReadDTO>>> AddDriver(
            [FromForm] DriverAddDTO driverDto,
            [FromForm] string vehicleInfoJson,
            [FromForm] string locationsJson)
        {
            try
            {
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

                return CreatedAtAction(
                    nameof(GetDriverById),
                    new { id = createdDriver.Id },
                    ApiResponse<DriverReadDTO>.SuccessResponse(
                        "Driver created successfully",
                        201,
                        createdDriver));
            }
            catch (Exception)
            {
                return ApiResponse<DriverReadDTO>.ErrorResponse(
                    "An error occurred while creating the driver",
                    500);
            }
        }

        [HttpPut("EditDriver/{id}")]
        public async Task<ActionResult<ApiResponse<DriverReadDTO>>> EditDriver(
            string id,
            [FromForm] DriverUpdateDTO driverDto)
        {
            try
            {
                var updatedDriver = await _driverService.UpdateDriverProfileAsync(id, driverDto);
                if (updatedDriver == null)
                {
                    return ApiResponse<DriverReadDTO>.ErrorResponse(
                        "Driver not found",
                        404);
                }

                return ApiResponse<DriverReadDTO>.SuccessResponse(
                    "Driver profile updated successfully",
                    200
                    /*updatedDriver*/);
            }
            catch (Exception)
            {
                return ApiResponse<DriverReadDTO>.ErrorResponse(
                    "An error occurred while updating the driver",
                    500);
            }
        }

        [HttpPut("EditDriverVehicles/{id}")]
        public async Task<ActionResult<ApiResponse<List<VehicleReadDTO>>>> EditDriverVehicles(
            string id,
            [FromForm] string vehiclesJson)
        {
            try
            {
                var vehicleDto = JsonSerializer.Deserialize<DriverVehicleUpdateDTO>(vehiclesJson);

                // Validate all vehicles have plate numbers
                if (vehicleDto.Vehicles.Any(v => string.IsNullOrWhiteSpace(v.PlateNumber)))
                {
                    return ApiResponse<List<VehicleReadDTO>>.ErrorResponse(
                        "All vehicles must have a PlateNumber",
                        400);
                }

                // Process files
                for (int i = 0; i < vehicleDto.Vehicles.Count; i++)
                {
                    vehicleDto.Vehicles[i].VehiclePicture = Request.Form.Files[$"Vehicles[{i}].VehiclePicture"];
                    vehicleDto.Vehicles[i].VehicleRegistrationFront = Request.Form.Files[$"Vehicles[{i}].VehicleRegistrationFront"];
                    vehicleDto.Vehicles[i].VehicleRegistrationBack = Request.Form.Files[$"Vehicles[{i}].VehicleRegistrationBack"];
                }

                var updatedVehicles = await _driverService.UpdateDriverVehiclesAsync(id, vehicleDto);

                if (updatedVehicles == null)
                {
                    return ApiResponse<List<VehicleReadDTO>>.ErrorResponse(
                        "Driver not found",
                        404);
                }

                return ApiResponse<List<VehicleReadDTO>>.SuccessResponse(
                    "Vehicles updated successfully",
                    200
                    /*updatedVehicles*/);
            }
            catch (Exception)
            {
                return ApiResponse<List<VehicleReadDTO>>.ErrorResponse(
                    "An error occurred while updating vehicles",
                    500);
            }
        }


        [HttpDelete("DeleteDriver/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteDriver(string id)
        {
            try
            {
                bool deleted = await _driverService.SoftDeleteDriverAsync(id);
                if (!deleted)
                {
                    return ApiResponse<string>.ErrorResponse(
                        "Driver not found",
                        404);
                }

                return ApiResponse<string>.SuccessResponse(
                    "Driver deleted successfully",
                    200);
            }
            catch (Exception)
            {
                return ApiResponse<string>.ErrorResponse(
                    "An error occurred while deleting the driver",
                    500);
            }
        }




       
    }
}