using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class PropertyImage
    {
        [Key]
        public int PropertyImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
