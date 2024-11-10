using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class Trip
    {
        public int TripId { get; set; }  // Primary key

        [Required]
        public string FromLocation { get; set; }

        [Required]
        public string ToLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }// This marks the beginning of the journey from the departure location.

        [Range(1, 50)]  // Assuming max 50 seats
        public int AvailableSeats { get; set; }

        [Required]
        public decimal PricePerSeat { get; set; }

        public string Description { get; set; }

        public int DriverId { get; set; }  // Foreign key to Driver (User)
        public User Driver { get; set; }  // Navigation property

        // Relationship with bookings
        public ICollection<Booking> Bookings { get; set; }
    }

}
