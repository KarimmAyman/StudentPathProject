using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using System.Text.Json.Serialization;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripWithBookingsDto
    {
        public int TripId { get; set; }
        public TripLocationDto FromLocation { get; set; }
        public TripLocationDto ToLocation { get; set; }
        public TripBasicInfoDto BasicInfo { get; set; }
        public DriverDetailedInfoDto DriverInfo { get; set; }
        public TripAdditionalInfoDto AdditionalInfo { get; set; }
        public List<BookingDetailsDto> Bookings { get; set; } = new();
        public TripStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public class TripBasicInfoDto
        {
            public DateTime DepartureTime { get; set; }
            public string FormattedDepartureTime { get; set; }
            public double? EstimatedDistance { get; set; }
            public string FormattedDistance { get; set; }
            public TimeSpan? EstimatedDuration { get; set; }
            public string FormattedDuration { get; set; }
            public int AvailableSeats { get; set; }
            public int TotalBookedSeats { get; set; }
            public decimal PricePerSeat { get; set; }
        }

        public class DriverDetailedInfoDto
        {
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string DriverPhone { get; set; }
            public string DriverPhotoUrl { get; set; }
            public decimal? DriverRating { get; set; }
            public VehicleInfoDto Vehicle { get; set; }
        }

        public class TripAdditionalInfoDto
        {
            public string StartingPoint { get; set; }
            public string Notes { get; set; }
            public List<string> Amenities { get; set; } = new();
        }

        public class BookingDetailsDto
        {
            public int BookingId { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserPhone { get; set; }
            public string UserPhotoUrl { get; set; }
            public int NumberOfSeats { get; set; }
            public string BookingStatus { get; set; }
            public DateTime BookingDate { get; set; }
            public decimal TotalAmount { get; set; }
            public MeetingPointDto MeetingPoint { get; set; }
            public PaymentInfoDto Payment { get; set; }
        }

        public class MeetingPointDto
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string DisplayName { get; set; }
            public string FullAddress { get; set; }
            public DateTime EstimatedArrivalTime { get; set; }
        }

        public class PaymentInfoDto
        {
            public string PaymentMethod { get; set; }
            public string TransactionId { get; set; }
            public string Status { get; set; }
            public DateTime PaymentDate { get; set; }
        }
    }
}