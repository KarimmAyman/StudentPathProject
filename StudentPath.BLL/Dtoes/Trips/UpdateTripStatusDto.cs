using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Trips
{
    public class UpdateTripStatusDto
    {
        public TripStatus NewStatus { get; set; }
    }
}
