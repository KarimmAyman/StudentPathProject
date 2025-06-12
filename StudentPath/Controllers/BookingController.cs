using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes.Bookings;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Security.Claims;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly StudentPathContext context;

        public BookingController(StudentPathContext context)
        {
            this.context = context;
        }
        [HttpPost("book")]
        public async Task<IActionResult> BookTrip([FromBody] CreateBookingRequestDTO request)
        {

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUserId == null)
            {
                return Unauthorized("You must be user to be logged in to book a trip.");
            }
            if (user.UserType != UserTypeEnum.User)
                return Forbid("Only Users are allowed to book trips.");

            var trip = await context.Trips
        .FirstOrDefaultAsync(t => t.TripId == request.TripId);

            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.DepartureTime <= DateTime.UtcNow)
            {
                return BadRequest(new { error = "This trip has already departed. You cannot book a past trip." });
            }

            // B. Is it too close to the departure time? (must be > 1 hour before)
            if (trip.DepartureTime <= DateTime.UtcNow.AddHours(1))
            {
                return BadRequest(new { error = "Bookings must be made at least 1 hour before the trip starts." });
            }

                var seatsToBook = request.NumberOfSeats;
            if (seatsToBook <= 0)
            {
                return BadRequest(new { error = "You must book at least one seat." });
            }

           


            if (seatsToBook > trip.AvailableSeats)
                {
                    return BadRequest(new
                    {
                        error = $"Only {trip.AvailableSeats} seats available. You requested {seatsToBook}."
                    });
                }

            if (trip.PricePerSeat <= 0)
            {
                return BadRequest(new { error = "Invalid trip price per seat. Cannot proceed with booking." });
            }

            decimal totalCost = seatsToBook * trip.PricePerSeat;

            if (totalCost <= 0)
            {
                return BadRequest(new { error = "Invalid total cost. Cannot proceed with booking." });
            }

            var booking = new Booking
                {
                    UserId = currentUserId,
                    TripId = request.TripId,
                    BookingDate = DateTime.UtcNow,
                    NumberOfSeats = seatsToBook,
                    MeetingPoint = request.MeetingPoint,
                    Note = request.Note,
                    TotalPrice = totalCost,
                    IsCancelled = false,
                    PaymentStatus = PaymentStatus.Pending,
                    BookingStatus = BookingStatus.Pending
                };
                context.Bookings.Add(booking);

                // Deduct all requested seats
                trip.AvailableSeats -= seatsToBook;
            

            // 7. Persist changes
            await context.SaveChangesAsync();

            // 8. Return the updated booking summary
            return Ok(new
            {
                data = new
                {
                    bookingStatus = booking.BookingStatus.ToString(),
                    bookingId = booking.BookingId,
                    totalSeats = booking.NumberOfSeats,
                    availableSeats = trip.AvailableSeats,
                    totalPrice = booking.TotalPrice
                },
                message = "Booking processed successfully.",
                success = true,
                statusCode = 200
            });
        }
    }
}


