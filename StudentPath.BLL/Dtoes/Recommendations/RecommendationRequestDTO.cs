using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Recommendations
{
    public class RecommendationRequestDTO
    {
        public string UserId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class AiTrip
    {
        [JsonPropertyName("TripId")]
        public int TripId { get; set; }
        [JsonPropertyName("FromLocationId")]
        public int FromLocationId { get; set; }
        [JsonPropertyName("ToLocationId")]
        public int ToLocationId { get; set; }
        [JsonPropertyName("PricePerSeat")]
        public float PricePerSeat { get; set; }
        [JsonPropertyName("EstimatedDistance")]
        public float EstimatedDistance { get; set; }
        [JsonPropertyName("EstimatedDuration")]
        public float EstimatedDuration { get; set; }
        [JsonPropertyName("DepartureTime")]
        public string DepartureTime { get; set; }
        [JsonPropertyName("HasAirConditioning")]
        public int HasAirConditioning { get; set; }
        [JsonPropertyName("HasFreeWater")]
        public int HasFreeWater { get; set; }
        [JsonPropertyName("HasMusic")]
        public int HasMusic { get; set; }
        [JsonPropertyName("HasPhoneCharger")]
        public int HasPhoneCharger { get; set; }
        [JsonPropertyName("HasWiFi")]
        public int HasWiFi { get; set; }
        [JsonPropertyName("AvailableSeats")]
        public int AvailableSeats { get; set; }
    }

    public class AiLocation
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("Latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("Longitude")]
        public float Longitude { get; set; }
    }

    public class AiRecommendationRequest
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
        [JsonPropertyName("past_trips")]
        public List<AiTrip> PastTrips { get; set; }
        [JsonPropertyName("upcoming_trips")]
        public List<AiTrip> UpcomingTrips { get; set; }
        [JsonPropertyName("locations")]
        public List<AiLocation> Locations { get; set; }
    }

    public class AiRecommendationResponse
    {
        [JsonPropertyName("recommended_trips")]
        public List<int> RecommendedTrips { get; set; }
    }


    public class RecommendedTrip
    {
        public int Id { get; set; }
        public LocationInfo FromLocation { get; set; }
        public LocationInfo ToLocation { get; set; }
        public BasicInfo BasicInfo { get; set; }
        public DriverInfo DriverInfo { get; set; }
        public AdditionalInfoDTO AdditionalInfo { get; set; }
        public decimal PricePerSeat { get; set; }
        public DateTime CreatedAt { get; set; }
        public TripStatus Status { get; set; }
    }

    public class LocationInfo
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string DisplayName { get; set; }
        public string FullAddress { get; set; }
    }

    public class BasicInfo
    {
        public string FormattedDepartureTime { get; set; }
        public string FormattedDistance { get; set; }
        public string FormattedDuration { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class DriverInfo
    {
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public VehicleInfoDTO VehicleInfo { get; set; }
    }

    public class VehicleInfoDTO
    {
        public string VehicleModel { get; set; }
        public int SeatingCapacity { get; set; }
        public string PlateNumber { get; set; }
    }

    public class AdditionalInfoDTO
    {
        public string StartingPoint { get; set; }
        public string? Notes { get; set; }
        public List<string> Amenities { get; set; } 

      
      
    }

}