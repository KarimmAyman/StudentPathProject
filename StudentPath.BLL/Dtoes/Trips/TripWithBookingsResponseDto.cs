using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;
using System.Text.Json.Serialization;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripWithBookingsResponseDto : TripResponseDto
    {
        public List<BookingInfoDto> Bookings { get; set; } = new();

        public class BookingInfoDto
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