using System;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripCreateDto
    {
        [Required]
        public TripLocationDto FromLocation { get; set; }

        [Required]
        public TripLocationDto ToLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        [Range(1, 10)]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0.01, 1000.00)]
        public decimal PricePerSeat { get; set; }

        [MaxLength(500)]
        public string? DriverNotes { get; set; }
        public double? EstimatedDistance { get; set; }
        public TimeSpan? EstimatedDuration { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; }

        // Amenities
        public bool HasWiFi { get; set; }
        public bool HasPhoneCharger { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasChildSeat { get; set; }
        public bool HasFreeWater { get; set; }
        public bool HasMusic { get; set; }
    }
}