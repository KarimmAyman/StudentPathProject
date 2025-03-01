using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Foreign Key linking to User
        [ForeignKey("User")]
        public string UserId { get; set; }
       
        public virtual User User { get; set; }
    }

}
