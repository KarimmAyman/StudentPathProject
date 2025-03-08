using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public enum FeatureCategory
    {
        Interior,
        Exterior
    }
    public class Feature
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // e.g., "Jacuzzi", "Security", etc.

        [Required]
        public FeatureCategory Category { get; set; } // Interior or Exterior

        public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; } = new List<PropertyFeature>();
    }
}
