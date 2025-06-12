using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.BLL.Services.TripServices;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using static StudentPath.BLL.Dtoes.Trips.TripResponseDto;

namespace StudentPath.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly StudentPathContext context;

        public TripsController(ITripService tripService, StudentPathContext context)
        {
            _tripService = tripService;
            this.context = context;
        }

        /// <summary>
        /// Create a new trip
        /// </summary>
        /// <param name="dto">Trip creation data</param>
        /// <returns>Created trip details</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTrip([FromBody] TripCreateDto dto)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("User not authenticated");

            // Validate required location data
            if (dto.FromLocation == null || dto.ToLocation == null)
                return BadRequest("From and To locations are required");

            var result = await _tripService.CreateTripAsync(dto, driverId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing trip
        /// </summary>
        /// <param name="dto">Trip update data</param>
        /// <returns>Updated trip details</returns>
        //[HttpPut]
        //public async Task<IActionResult> UpdateTrip([FromBody] TripUpdateDto dto)
        //{
        //    var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(driverId))
        //        return Unauthorized("User not authenticated");

        //    var result = await _tripService.UpdateTripAsync(dto, driverId);
        //    return StatusCode(result.StatusCode, result);
        //}

        /// <summary>
        /// Delete a trip
        /// </summary>
        /// <param name="tripId">ID of the trip to delete</param>
        /// <returns>Operation result</returns>
        [HttpDelete("{tripId}")]
        public async Task<IActionResult> DeleteTrip(int tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("User not authenticated");

            var result = await _tripService.DeleteTripAsync(tripId, driverId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get trip by ID
        /// </summary>
        /// <param name="tripId">ID of the trip to retrieve</param>
        /// <returns>Trip details</returns>
        [AllowAnonymous]
        [HttpGet("{tripId}")]
        public async Task<IActionResult> GetTripById(int tripId)
        {
            var result = await _tripService.GetTripByIdAsync(tripId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get all upcoming trips
        /// </summary>
        /// <returns>List of upcoming trips</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTrips([FromQuery] bool includePast = false)
        {
            var result = await _tripService.GetAllTripsAsync(includePast);
            
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Get trips created by the current driver
        /// </summary>
        /// <returns>List of driver's trips</returns>
        [HttpGet("driver")]
        public async Task<IActionResult> GetDriverTrips()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("User not authenticated");

            var result = await _tripService.GetDriverTripsAsync(driverId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Search for trips based on criteria
        /// </summary>
        /// <param name="fromCity">Departure city</param>
        /// <param name="toCity">Destination city</param>
        /// <param name="date">Optional departure date</param>
        /// <returns>List of matching trips</returns>
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips(
            [FromQuery] string fromAddress,
            [FromQuery] string toAddress)
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
                return BadRequest("From address is required");

            if (string.IsNullOrWhiteSpace(toAddress))
                return BadRequest("To address is required");

            var result = await _tripService.SearchTripsAsync(fromAddress, toAddress);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get trips with specific amenities
        /// </summary>
        /// <param name="amenities">Amenities filter</param>
        /// <returns>List of matching trips</returns>
        //[AllowAnonymous]
        //[HttpGet("amenities")]
        //public async Task<IActionResult> GetTripsByAmenities([FromQuery] TripAmenitiesDto amenities)
        //{
        //    var result = await _tripService.GetTripsByAmenitiesAsync(amenities);
        //    return StatusCode(result.StatusCode, result);
        //}
            [HttpPost("Create-or-request-trip")]

        public async Task<IActionResult> CreateOrRequestTrip([FromBody] TripSearchOrRequestDto request)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                // Check if the user exists and is a regular user
                if (user == null)
                {
               
                    return Unauthorized("You must be user to be logged in to create a trip request.");
                
                }

                if (user.UserType != UserTypeEnum.User)
                {
                    return Forbid("Only regular users are allowed to request or search for trips.");
                }

            // Check if any trip exists between the two locations
            var existingTrips = await context.Trips
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.Driver)
                .Where(t =>
                    t.FromLocation.Latitude == request.FromLocation.Latitude &&
                    t.FromLocation.Longitude == request.FromLocation.Longitude &&
                    t.ToLocation.Latitude == request.ToLocation.Latitude &&
                    t.ToLocation.Longitude == request.ToLocation.Longitude)
                    .ToListAsync();

            var tripDtos = existingTrips.Select(t =>

                     {

                    var additionalInfo = new AdditionalInfoDTO
                    {
                        StartingPoint = t.FromLocation.DisplayName,
                        Notes = t.DriverNotes,
                        HasWiFi = t.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                        HasMusic = t.HasMusic, // Assuming `HasMusic` exists in the trip model
                        HasPhoneCharger = t.HasPhoneCharger,
                        HasAirConditioning = t.HasAirConditioning,
                        HasFreeWater = t.HasFreeWater

                    };
                    additionalInfo.PopulateAmenities();



                   return new TripResponseDto
                    {
                        Id = t.TripId,
                        FromLocation = new TripLocationDto
                        {
                            Id = t.FromLocation.Id,
                            DisplayName = t.FromLocation.DisplayName,
                            FullAddress = t.FromLocation.FullAddress,
                            AdditionalNotes = t.FromLocation.AdditionalNotes,
                            Latitude = t.FromLocation.Latitude,
                            Longitude = t.FromLocation.Longitude
                        },
                        ToLocation = new TripLocationDto
                        {
                            Id = t.ToLocation.Id,
                            DisplayName = t.ToLocation.DisplayName,
                            FullAddress = t.ToLocation.FullAddress,
                            AdditionalNotes = t.ToLocation.AdditionalNotes,
                            Latitude = t.ToLocation.Latitude,
                            Longitude = t.ToLocation.Longitude
                        },
                        BasicInfo = new BasicInfoDTO
                        {
                            DepartureTime = t.DepartureTime,
                            EstimatedDistance = t.EstimatedDistance,
                            EstimatedDuration = t.EstimatedDuration,
                            AvailableSeats = t.AvailableSeats,

                        },
                        DriverInfo = new DriverInfoDto
                        {
                            DriverId = t.DriverId,
                            DriverName = t.Driver?.UserName,
                            DriverPhone = t.Driver?.PhoneNumber,
                            VehicleInfo = (t.Driver as Driver)?.VehicleInfo?
                          .Where(v => v.DriverId == t.DriverId).
                     Select(v => new VehicleInfoDto
                     {
                         VehicleModel = v.VehicleModel,
                         SeatingCapacity = v.SeatingCapacity,

                         PlateNumber = v.PlateNumber

                     })
                    .FirstOrDefault()
                        },
                        AdditionalInfo = additionalInfo,
                        PricePerSeat = t.PricePerSeat,
                        Status = t.Status,
                       CreatedAt = t.CreatedAt
                    };
                });

                // Return trips if any exist
                if (existingTrips.Any())
                {
                    return Ok(new { message = "Trips found.", trips = tripDtos });
                }


            if (user.CanReceiveTripRequests == null || !user.CanReceiveTripRequests.Value)
            {
                user.CanReceiveTripRequests = true;  // Set to true
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }


            // If no trips and the user accepts trip requests, enable trip request functionality
            if (user.CanReceiveTripRequests.HasValue && user.CanReceiveTripRequests.Value)
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Update the user to accept trip requests
                            user.IsActive = true;
                            context.Users.Update(user);

                        var fromLocation = await context.TripLocations.FirstOrDefaultAsync(l =>
                             l.Latitude == request.FromLocation.Latitude &&
                             l.Longitude == request.FromLocation.Longitude);

                        if (fromLocation == null)
                        {
                            fromLocation = new TripLocation
                            {
                                Latitude = request.FromLocation.Latitude,
                                Longitude = request.FromLocation.Longitude,
                                DisplayName = request.FromLocation.DisplayName,
                                FullAddress = request.FromLocation.FullAddress,
                            };
                            context.TripLocations.Add(fromLocation);
                            await context.SaveChangesAsync();
                        }

                        // Try to find existing ToLocation
                        var toLocation = await context.TripLocations.FirstOrDefaultAsync(l =>
                            l.Latitude == request.ToLocation.Latitude &&
                            l.Longitude == request.ToLocation.Longitude);

                        if (toLocation == null)
                        {
                            toLocation = new TripLocation
                            {
                                Latitude = request.ToLocation.Latitude,
                                Longitude = request.ToLocation.Longitude,
                                DisplayName = request.ToLocation.DisplayName,
                                FullAddress = request.ToLocation.FullAddress,
                            };
                            context.TripLocations.Add(toLocation);
                            await context.SaveChangesAsync();
                        }
                        var existingTripRequest = await context.TripRequests.FirstOrDefaultAsync(tr =>
                               tr.UserId == user.Id &&
                               tr.FromLocationId == fromLocation.Id &&
                               tr.ToLocationId == toLocation.Id &&
                               tr.IsLookingForTrip == true);

                        if (existingTripRequest != null)
                        {
                            return BadRequest(new { message = "You have already requested a trip for these locations." });
                        }

                        var existingRequests = await context.TripRequests
                 .Where(tr => tr.UserId == user.Id)
                 .ToListAsync();

                        if (existingRequests.Any())
                        {
                            context.TripRequests.RemoveRange(existingRequests);
                            await context.SaveChangesAsync();
                        }
                        // Create the TripRequest
                        var tripRequest = new TripRequest
                        {
                            UserId = user.Id,
                            FromLocationId = fromLocation.Id,
                            ToLocationId = toLocation.Id,
                            IsLookingForTrip = true,
                            RequestDate = DateTime.UtcNow
                        };
                        context.TripRequests.Add(tripRequest);
                        await context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return Ok(new { message = "No trips found. Your trip request has been saved." });
                    }
                    catch (Exception ex)
                        {
                            // Rollback the transaction in case of error
                            await transaction.RollbackAsync();
                            return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
                        }
                    }
                }

                return NotFound(new { message = "No trips found and the user does not accept trip requests." });
            }

              [HttpGet("active-trip-requests")]
              public async Task<IActionResult> GetActiveTripRequests()
               {
                var activeTripRequests = await context.TripRequests
                    .Where(tr => tr.IsLookingForTrip == true)
                    .Include(tr => tr.FromLocation)
                    .Include(tr => tr.ToLocation)
                    .GroupBy(tr => new { tr.FromLocationId, tr.ToLocationId })
                    .Select(group => new
                    {
                        FromLocation = new
                        {
                            group.First().FromLocation.Latitude,
                            group.First().FromLocation.Longitude,
                            group.First().FromLocation.DisplayName,
                            group.First().FromLocation.FullAddress
                        },
                        ToLocation = new
                        {
                            group.First().ToLocation.Latitude,
                            group.First().ToLocation.Longitude,
                            group.First().ToLocation.DisplayName,
                            group.First().ToLocation.FullAddress
                        },
                        ActivePassengers = group.Count()
                    })
        .ToListAsync();

            return Ok(new
            {
                data = activeTripRequests,
                message = "Trip requests retrieved successfully.",
                success = true,
                statusCode = 200
            });
        }
        [HttpPatch("{tripId}/status")]
        public async Task<IActionResult> UpdateTripStatus(int tripId, [FromBody] UpdateTripStatusDto dto)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
                return Unauthorized("User not authenticated");

            var result = await _tripService.UpdateTripStatusAsync(tripId, dto.NewStatus, driverId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTripsByStatus(TripStatus status)
        {
            var result = await _tripService.GetTripsByStatusAsync(status);
            return StatusCode(result.StatusCode, result);
        }
    }
}