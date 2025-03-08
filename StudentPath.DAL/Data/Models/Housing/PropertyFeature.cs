using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class PropertyFeature
    {
        [Key]
        public int PropertyFeatureId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }  // Marked as virtual for lazy loading

        [ForeignKey("Feature")]
        public int FeatureId { get; set; }
        public virtual Feature Feature { get; set; }   // Marked as virtual for lazy loading
    }
}
