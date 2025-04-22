using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class TripLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(250)]
        public string FullAddress { get; set; }

        [MaxLength(150)]
        public string AdditionalNotes { get; set; }

        public virtual ICollection<Trip> TripsAsFrom { get; set; }
        public virtual ICollection<Trip> TripsAsTo { get; set; }
    }

}
