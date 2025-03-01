using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        public string FeatureName { get; set; } // e.g., Security, Parking, Shower Cabin
    }
}
