using System.ComponentModel.DataAnnotations;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripLocationDto
    {
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; }
        public string FullAddress { get; set; }
        public string? AdditionalNotes { get; set; }
    }
}