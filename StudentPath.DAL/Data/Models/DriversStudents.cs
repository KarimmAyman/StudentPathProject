using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models
{
    public  class DriverStudent
    {
        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
        [ForeignKey("Driver")]
        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
