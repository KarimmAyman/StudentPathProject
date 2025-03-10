using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.HousingDtoes
{
    public class FeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FeatureCategory Category { get; set; }
    }
}
