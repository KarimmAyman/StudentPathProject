using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class Trip
    {
        public int TripId { get; set; }  // Primary key

        [Required, MaxLength(100)]
        public string FromLocation { get; set; }

        [Required, MaxLength(100)]
        public string ToLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }  // Start of the trip

        [Range(1, 50)]
        public int AvailableSeats { get; set; }

        [Required, Range(0.01, 1000.00)]
        public decimal PricePerSeat { get; set; }  // Cost of each seat

        [MaxLength(500)]
        public string Description { get; set; }  // Optional trip details

        // Foreign key to Driver
        [ForeignKey("Driver")]
        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }  // Specific navigation to Driver

        // Relationship with Bookings
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }

}
