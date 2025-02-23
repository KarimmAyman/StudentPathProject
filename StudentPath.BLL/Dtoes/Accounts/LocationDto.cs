using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Accounts
{
    public class LocationDto
    {
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = "city";

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }="city";

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; } = 90;

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; } = 90;
    }

}
