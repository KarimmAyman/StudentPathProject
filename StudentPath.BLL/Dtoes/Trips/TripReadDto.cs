using System;
using System.Collections.Generic;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripReadDto
    {
        public int TripId { get; set; }
        public int StartLocationId { get; set; }
        public int EndLocationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
        public string DriverId { get; set; }
    }
}