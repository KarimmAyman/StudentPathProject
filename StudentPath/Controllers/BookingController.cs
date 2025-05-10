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

            if (currentUserId == null)
            {
                return Unauthorized("You must be user to be logged in to book a trip.");
            }
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

            // 3. Calculate requested seats
            var seatsToBook = request.NumberOfSeats;

            // 4. Look for an existing booking by this user on this trip
            var booking = await context.Bookings
                .FirstOrDefaultAsync(b => b.TripId == request.TripId
                                       && b.UserId == currentUserId);

            decimal extraCost = 0;

            if (booking != null)
            {
                // 5a. Already booked some seats—compute new total and delta
                var newSeatTotal = booking.NumberOfSeats + seatsToBook;
                var delta = seatsToBook;


                // 5b. Check availability for the extra seats
                if (delta > trip.AvailableSeats)
                {
                    return BadRequest(new
                    {
                        error = $"Only {trip.AvailableSeats} additional seats available. You requested {delta} more."
                    });
                }
                extraCost = delta * trip.PricePerSeat;


                // 5c. Update the existing booking

                booking.NumberOfSeats += newSeatTotal;
                booking.TotalPrice += extraCost;
                booking.BookingDate = DateTime.UtcNow;

                booking.Note = request.Note ?? booking.Note;
                booking.MeetingPoint = request.MeetingPoint ?? booking.MeetingPoint;
                // (leave other fields unchanged)
                booking.PaymentStatus = PaymentStatus.Pending;
                booking.BookingStatus = BookingStatus.Pending;

                // 5d. Deduct from available seats
                trip.AvailableSeats -= delta;

            }
            else
            {
                // 6. No existing booking—create a new one
                if (seatsToBook > trip.AvailableSeats)
                {
                    return BadRequest(new
                    {
                        error = $"Only {trip.AvailableSeats} seats available. You requested {seatsToBook}."
                    });
                }
                decimal totalCost = seatsToBook * trip.PricePerSeat;
                booking = new Booking
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
            }

            // 7. Persist changes
            await context.SaveChangesAsync();

            // 8. Return the updated booking summary
            return Ok(new
            {
                message = "Booking processed successfully.",
                bookingStatus = booking.BookingStatus.ToString(), // include status
                bookingId = booking.BookingId,
                totalSeats = booking.NumberOfSeats,
                availableSeats = trip.AvailableSeats,
                totalPrice = booking.TotalPrice,
                extraCost = extraCost
            });
        }
    }
}


