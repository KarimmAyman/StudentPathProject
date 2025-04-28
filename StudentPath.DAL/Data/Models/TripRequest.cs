using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class TripRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [ForeignKey("FromLocation")]

        public int FromLocationId { get; set; }
        public virtual TripLocation FromLocation { get; set; }

        [Required]
        [ForeignKey("ToLocation")]

        public int ToLocationId { get; set; }
        public virtual TripLocation ToLocation { get; set; }

        public bool IsLookingForTrip { get; set; } = true;

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    }
}
