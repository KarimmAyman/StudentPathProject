using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.BLL.Services.TripServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPath.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
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
    }
}