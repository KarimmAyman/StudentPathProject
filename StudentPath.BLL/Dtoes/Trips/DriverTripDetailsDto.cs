using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class DriverTripDetailsDto
    {
        public int Id { get; set; }
        public string DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public int RemainingSeats { get; set; }
        public string FromLocationDisplayName { get; set; }
        public string ToLocationDisplayName { get; set; }
    }
}
