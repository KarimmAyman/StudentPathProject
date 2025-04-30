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
        [Key]
        public int TripId { get; set; }

        [Required]
        [ForeignKey("FromLocation")]
        public int FromLocationId { get; set; }
        public virtual TripLocation FromLocation { get; set; }

        [Required]
        [ForeignKey("ToLocation")]
        public int ToLocationId { get; set; }
        public virtual TripLocation ToLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Range(1, 50)]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0.01, 1000.00)]
        public decimal PricePerSeat { get; set; }

        [MaxLength(500)]
        public string? DriverNotes { get; set; }
        public double? EstimatedDistance { get; set; } // in kilometers
        public TimeSpan? EstimatedDuration { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; }

        // Amenities
        public bool HasWiFi { get; set; }
        public bool HasPhoneCharger { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasFreeWater { get; set; }
        public bool HasMusic { get; set; }

        [ForeignKey("Driver")]
        public string DriverId { get; set; }
        public virtual User Driver { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }

}
