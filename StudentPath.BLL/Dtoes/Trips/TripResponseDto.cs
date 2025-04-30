namespace StudentPath.BLL.Dtoes.Trips
{
    public class TripResponseDto
    {
        public int Id { get; set; }
        public TripLocationDto FromLocation { get; set; }
        public TripLocationDto ToLocation { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string StartingPoint { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public double? EstimatedDistance { get; set; }
        public TimeSpan? EstimatedDuration { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}