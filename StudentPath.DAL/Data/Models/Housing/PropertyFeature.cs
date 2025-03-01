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
        public Property Property { get; set; }

        [ForeignKey("Feature")]
        public int FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}
