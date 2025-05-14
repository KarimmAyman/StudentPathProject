using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Identity;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Services.DriverServices;
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

        public DriverController(IDriverService driverService,StudentPathContext context)
        {
            _driverService = driverService;
            this.context = context;
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




        #region stripedriver


        //[HttpPost("driver/stripe/create-account")]
        //public async Task<IActionResult> CreateStripeAccount([FromBody] StripeAccountRequest dto)
        //{
        //    var accountService = new AccountService();

        //    var accountOptions = new AccountCreateOptions
        //    {
        //        Type = "express",
        //        Country = "EG", // or your driver's country
        //        Email = dto.Email, // use actual driver email
        //        Capabilities = new AccountCapabilitiesOptions
        //        {
        //            Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
        //        },
        //        TosAcceptance = new AccountTosAcceptanceOptions { ServiceAgreement = "recipient" },

        //    };

        //    var account = await accountService.CreateAsync(accountOptions);

        //    var domain = "https://localhost:7092"; // replace with your real domain or use config

        //    var accountLinkService = new AccountLinkService();
        //    var link = await accountLinkService.CreateAsync(new AccountLinkCreateOptions
        //    {
        //        Account = account.Id,
        //        RefreshUrl = $"{domain}/api/Driver/driver/stripe/refresh?account={account.Id}",
        //        ReturnUrl = $"{domain}/api/Driver/driver/stripe/return?account={account.Id}",
        //        Type = "account_onboarding"
        //    });
        ////    var identityService = new Stripe.Identity.VerificationSessionService();


        ////    var identitySession = await identityService.CreateAsync(new VerificationSessionCreateOptions
        ////    {
        ////        Type = "document", // your flow ID
        ////        Metadata = new Dictionary<string, string>
        ////{
        ////    { "email", dto.Email },
        ////    { "phone", dto.PhoneNumber },
        ////    { "stripe_account_id", dto.StripeAccountId }
        ////}
        ////    });
        ////    var returnUrl = $"{domain}/api/Driver/driver/identity/complete?session_id={identitySession.Id}";



        //    // Save `account.Id` in your DB linked to the driver

        //    return Ok(new
        //    {
        //    //    SessionId = identitySession.Id,
        //        OnboardingUrl = link.Url
        //        //IdentityVerificationUrl = identitySession.Url,
        //        //ReturnUrl=returnUrl
        //    });
        //}





        //public async Task<IActionResult> CreateOrVerifyStripeAccount([FromBody] StripeAccountRequest dto)
        //{
        //    var domain = "https://localhost:7092"; // replace with your real domain or use config

        //    // Check if the user already has a Stripe account
        //    var existingAccountId = await context.Users
        //        .Where(u => u.Email == dto.Email)
        //        .Select(u => u.StripeAccountId) // Assuming you save the Stripe Account ID in your user table
        //        .FirstOrDefaultAsync();

        //    if (!string.IsNullOrEmpty(existingAccountId))
        //    {
        //        // User already has a Stripe account, proceed to verification
        //        return await VerifyStripeAccount(existingAccountId, dto.Email, dto.PhoneNumber, domain);
        //    }

        //    // If user doesn't have an account, create a new Stripe account
        //    return await CreateStripeAccount(dto, domain);
        //}

        //private async Task<IActionResult> CreateStripeAccount(StripeAccountRequest dto, string domain)
        //{
        //    var accountService = new AccountService();

        //    // Create a new Stripe account
        //    var accountOptions = new AccountCreateOptions
        //    {
        //        Type = "express",
        //        Country = "EG", // or your driver's country
        //        Email = dto.Email, // use actual driver email
        //        Capabilities = new AccountCapabilitiesOptions
        //        {
        //            Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
        //        },
        //        TosAcceptance = new AccountTosAcceptanceOptions { ServiceAgreement = "recipient" },
        //    };

        //    var account = await accountService.CreateAsync(accountOptions);

        //    // Save the account ID in the user record in your database
        //    var user = await context.Users
        //        .Where(u => u.Email == dto.Email)
        //        .FirstOrDefaultAsync();

        //    if (user != null)
        //    {
        //        user.StripeAccountId = account.Id;  // Store the Stripe account ID
        //        await context.SaveChangesAsync();
        //    }

        //    // Generate the account link for onboarding
        //    var accountLinkService = new AccountLinkService();
        //    var link = await accountLinkService.CreateAsync(new AccountLinkCreateOptions
        //    {
        //        Account = account.Id,
        //        RefreshUrl = $"{domain}/api/Driver/driver/stripe/refresh?account={account.Id}",
        //        ReturnUrl = $"{domain}/api/Driver/driver/stripe/return?account={account.Id}",
        //        Type = "account_onboarding"
        //    });

        //    // Create the verification session for the new account
        //    var identityService = new Stripe.Identity.VerificationSessionService();
        //    var identitySession = await identityService.CreateAsync(new VerificationSessionCreateOptions
        //    {
        //        VerificationFlow = "vf_1RNeMYG7zadl0u1PqbPV2QL5", // your flow ID
        //        Metadata = new Dictionary<string, string>
        //{
        //    { "email", dto.Email },
        //    { "phone", dto.PhoneNumber },
        //    { "stripe_account_id", account.Id }
        //},
        //        ReturnUrl = $"{domain}/api/Driver/driver/identity/complete"
        //    });

        //    return Ok(new
        //    {
        //        OnboardingUrl = link.Url,
        //        IdentityVerificationUrl = identitySession.Url
        //    });
        //}

        //private async Task<IActionResult> VerifyStripeAccount(string accountId, string email, string phoneNumber, string domain)
        //{
        //    var accountService = new AccountService();

        //    // Retrieve the existing Stripe account
        //    var account = await accountService.GetAsync(accountId);

        //    // Check if the account is already verified
        //    if (account.Verification.Status == "verified")
        //    {
        //        return Ok("Account is already verified.");
        //    }

        //    // Create a verification session for the existing account
        //    var identityService = new Stripe.Identity.VerificationSessionService();
        //    var identitySession = await identityService.CreateAsync(new VerificationSessionCreateOptions
        //    {
        //        VerificationFlow = "vf_1RNeMYG7zadl0u1PqbPV2QL5", // your flow ID
        //        Metadata = new Dictionary<string, string>
        //{
        //    { "email", email },
        //    { "phone", phoneNumber },
        //    { "stripe_account_id", accountId }
        //},
        //        ReturnUrl = $"{domain}/api/Driver/driver/identity/complete"
        //    });

        //    return Ok(new
        //    {
        //        IdentityVerificationUrl = identitySession.Url
        //    });
        //}


        //[HttpGet("driver/stripe/refresh")]
        //public IActionResult StripeRefresh([FromQuery] string account)
        //{
        //    // Optionally log or alert that onboarding was cancelled or expired
        //    return Content("Stripe onboarding was cancelled or expired. Please try again.");
        //}

        //[HttpGet("driver/stripe/return")]
        //public async Task<IActionResult> StripeReturn([FromQuery] string account)
        //{
        //    var accountService = new AccountService();
        //    var stripeAccount = await accountService.GetAsync(account);

        //    if (stripeAccount.ChargesEnabled && stripeAccount.PayoutsEnabled)
        //    {
        //        // ✅ Update your DB: mark driver as onboarded
        //        return Content("Stripe onboarding completed successfully.");
        //    }

        //    return Content("Stripe onboarding not yet complete. Please finish the process.");
        //}
        //[HttpGet("driver/identity/complete")]
        //public async Task<IActionResult> IdentityComplete([FromQuery] string session_id)
        //{
        //    if (string.IsNullOrEmpty(session_id))
        //        return BadRequest("Missing session_id");

        //    var service = new Stripe.Identity.VerificationSessionService();
        //    var session = await service.GetAsync(session_id);

        //    var email = session.Metadata.ContainsKey("email") ? session.Metadata["email"] : "unknown";

        //    switch (session.Status)
        //    {
        //        case "verified":
        //            // 🔒 Update the database to mark the driver as verified using email or driver_id from metadata
        //            // Example: await _driverService.MarkAsVerified(email);
        //            return Ok($"✅ Identity verification completed for {email}.");

        //        case "requires_input":
        //            return Ok("⚠️ Verification not completed. Please go back and finish the steps.");

        //        case "canceled":
        //            return Ok("❌ Verification was canceled.");

        //        case "processing":
        //            return Ok("⏳ Verification is still being processed. Please check back later.");

        //        default:
        //            return Ok($"ℹ️ Current verification status: {session.Status}");
        //    }
        //}
        #endregion

    }
}