using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Accounts
{
    public class VehicleInfoDto
    {
        [Required, MaxLength(50)]
        public string VehicleType { get; set; }

        [Required, MaxLength(20)]
        public string LicensePlate { get; set; }

        [Range(1, 50)]
        public int SeatingCapacity { get; set; }
    }
}
