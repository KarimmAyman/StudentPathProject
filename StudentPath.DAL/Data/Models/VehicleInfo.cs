using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class VehicleInfo
    {
        public int VehicleInfoId { get; set; }  // Primary key

        [Required]
        public string VehicleType { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Range(1, 50)]  // Assuming seating capacity is between 1 and 50
        public int SeatingCapacity { get; set; }

        public int UserId { get; set; }  // Foreign key to User
        public User User { get; set; }  // Navigation property
    }

}
