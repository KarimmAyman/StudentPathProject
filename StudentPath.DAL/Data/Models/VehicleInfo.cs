using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class VehicleInfo
    {
        public int VehicleInfoId { get; set; }  // Primary key
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public int ProductionYear { get; set; }
        public string PlateNumber { get; set; }
        public int SeatingCapacity { get; set; }

        // Vehicle Documents
        public string? VehiclePicturePath { get; set; }
        public string? VehicleRegistrationFrontPath { get; set; }
        public string? VehicleRegistrationBackPath { get; set; }

        [ForeignKey("Driver")]
        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }
    }

}
