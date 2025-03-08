using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.HousingDtoes
{
    public class LocationDto
    {
        public int Id { get; set; } // ID of the location
        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
