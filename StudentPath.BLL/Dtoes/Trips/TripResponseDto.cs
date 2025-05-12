using StudentPath.BLL.Dtoes.Accounts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripResponseDto
    {
        public int Id { get; set; }
        public TripLocationDto FromLocation { get; set; }
        public TripLocationDto ToLocation { get; set; }
        
        public BasicInfoDTO BasicInfo { get; set; }
        public DriverInfoDto DriverInfo { get; set; }


        public AdditionalInfoDTO AdditionalInfo { get; set; }
        public decimal PricePerSeat { get; set; }
        public DateTime CreatedAt { get; set; }





        public class DriverInfoDto
        {

            [JsonIgnore]
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string DriverPhone { get; set; }

            // If the driver has multiple vehicles
            public VehicleInfoDto? VehicleInfo { get; set; }
        }
        public class VehicleInfoDto
        {
          
            public string VehicleModel { get; set; }
            public int SeatingCapacity { get; set; }
            public string PlateNumber { get; set; }
        }

        public class BasicInfoDTO
        {


            [JsonIgnore]
            public DateTime DepartureTime { get; set; }
            public string FormattedDepartureTime
            {
                get
                {
                    // Convert UTC to Egypt time (handles daylight saving automatically)
                    var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                    var localEgyptTime = TimeZoneInfo.ConvertTimeFromUtc(DepartureTime, egyptTimeZone);

                    if (localEgyptTime.Date == DateTime.UtcNow.Date)
                    {
                        return $"Today / {localEgyptTime.ToString("HH:mm")}";
                    }
                    else if (localEgyptTime.Date == DateTime.UtcNow.Date.AddDays(1))
                    {
                        return $"Tomorrow / {localEgyptTime:HH:mm}";
                    }
                    return localEgyptTime.ToString("yyyy-MM-dd HH:mm");
                }
            }
            [JsonIgnore]
          
            public double? EstimatedDistance { get; set; }
            public string FormattedDistance => EstimatedDistance.HasValue
                         ? $"{EstimatedDistance.Value} KM"
                         : "N/A";
            [JsonIgnore]
            public TimeSpan? EstimatedDuration { get; set; }
            public string FormattedDuration
            {
                get
                {
                    if (!EstimatedDuration.HasValue) return "N/A";

                    var duration = EstimatedDuration.Value;
                    if (duration.TotalHours < 1)
                        return $"{duration.Minutes}min";
                    else if (duration.TotalHours == 1)
                        return $"1h {duration.Minutes}min";
                    else
                        return $"{(int)duration.TotalHours}h {duration.Minutes}min";
                }
            }
            public int AvailableSeats { get; set; }
           
               

         


        }
        public class AdditionalInfoDTO
        {
            public string StartingPoint { get; set; }
            public string? Notes { get; set; }
            public List<string> Amenities { get; set; } = new();

            [JsonIgnore]
            public bool? HasWiFi { get; set; }

            [JsonIgnore]
            public bool? HasPhoneCharger { get; set; }

            [JsonIgnore]
            public bool? HasAirConditioning { get; set; }

            [JsonIgnore]
            public bool? HasFreeWater { get; set; }

            [JsonIgnore]
            public bool? HasMusic { get; set; }

            public void PopulateAmenities()
            {
                Amenities = new List<string>();

                if (HasWiFi == true)
                    Amenities.Add("WiFi");
                if (HasMusic == true)
                    Amenities.Add("Music");
                if (HasPhoneCharger == true)
                    Amenities.Add("Phone Charger");
                if (HasAirConditioning == true)
                    Amenities.Add("Air Conditioning");
                if (HasFreeWater == true)
                    Amenities.Add("Free Water");
            }


        }
    }
}