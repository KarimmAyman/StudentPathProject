using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripLocationDto
    {
        [JsonIgnore]

        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; }
        public string FullAddress { get; set; }

        [JsonIgnore]
        public string? AdditionalNotes { get; set; }
    }
}