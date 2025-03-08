using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class LocationProperty
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Foreign Key linking to User
        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        public virtual Property Property { get; set; }

    }
}
