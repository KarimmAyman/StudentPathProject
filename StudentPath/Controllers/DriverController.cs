using Azure;
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
using StudentPath.DAL.Data.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using static StudentPath.DAL.Data.Models.DriverWalletTransaction;

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

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDriverDashboard()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("User not authenticated");

            var dashboard = await _driverService.GetDriverDashboardAsync(driverId);
            return Ok(dashboard);

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


        [HttpPost("withdraw-from-wallet")]
        public async Task<IActionResult> WithdrawDriverWalletTransaction(WithdrawWalletDto withdrawWalletDto)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("Driver ID not found in token.");

            using var transactionScope = await context.Database.BeginTransactionAsync();

            try { 
            var driver = await context.Drivers.FindAsync(driverId);
            if (driver == null) return NotFound("Driver not found.");

            
                decimal currentBalance = driver.Balance ?? 0;
                if (currentBalance < withdrawWalletDto.Amount)
                    return BadRequest("Insufficient balance for withdrawal.");

                // Deduct amount
                decimal newBalance = currentBalance - withdrawWalletDto.Amount;


            // Create the transaction with the new balance
            var transaction = new DriverWalletTransaction
            {
                DriverId = driverId,
                Amount = withdrawWalletDto.Amount,
                TransactionDate = DateTime.UtcNow,
                Operation = WalletTransactionOperation.Withdrawal,
                BalanceAfterTransaction = newBalance // sets the balance after this transaction
            };

            // Update the driver's balance in AspNetUsers table
            driver.Balance = newBalance;

            // Save the transaction and balance update
            context.DriverWalletsTransactions.Add(transaction);
            context.Drivers.Update(driver); // EF will track this automatically, but safe to mark explicitly

             await context.SaveChangesAsync(); // Save everything atomically
              await transactionScope.CommitAsync();

            return Ok(new
            {
                message = "Withdrawal processed successfully.",
                success = true,
                statusCode = 200,
                data = new
                {
                    balance = driver.Balance
                },
               

            });
        }
            catch (Exception ex)
            {
                await transactionScope.RollbackAsync();
                return StatusCode(500, new { message = ex.Message, errorCode = 500 });
            }
        }



        [HttpGet("wallet-transactions")]
        public async Task<IActionResult> GetDriverWalletTransactions()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("Driver ID not found in token.");

            var driver = await context.Drivers.FindAsync(driverId);
            if (driver == null) return NotFound("Driver not found.");


            var transactions = await context.DriverWalletsTransactions
                .Where(t => t.DriverId == driverId)
                .OrderByDescending(t => t.TransactionDate) // recent first
                .Select(t => new DriverWalletTransactionsDTO
                {
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Operation = t.Operation.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                data = transactions,
                currentBalance = driver.Balance ?? 0,
                message = "Driver wallet transactions retrieved successfully.",
                success = true,
                statusCode = 200
            });
        }





    }
}