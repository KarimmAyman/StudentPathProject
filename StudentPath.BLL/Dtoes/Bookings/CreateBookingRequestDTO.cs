using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Bookings
{
    public class CreateBookingRequestDTO
    {
        [Required]
        public int TripId { get; set; }  // The Trip the user wants to book

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must book at least one seat.")]
        public int NumberOfSeats { get; set; }  // Number of seats the user wants to book

        public CoordinateDto MeetingPoint { get; set; }

        public string Note { get; set; }  // Optional note for the booking
    }

    
    public class CoordinateDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
