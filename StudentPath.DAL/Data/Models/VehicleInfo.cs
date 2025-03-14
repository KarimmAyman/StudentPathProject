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

        [Required, MaxLength(50)]
        public string VehicleType { get; set; } = null!;

        [Required, MaxLength(20)]
        public string LicensePlate { get; set; } = null!;

        [Range(1, 50)]
        public int SeatingCapacity { get; set; } = 0;

        // Foreign key and navigation property specific to Driver
        [ForeignKey("Driver")]
        public string DriverId { get; set; } = null!;  // Foreign key to Driver
        public virtual Driver Driver { get; set; } = null!;  // Navigation property to Driver
    }

}
