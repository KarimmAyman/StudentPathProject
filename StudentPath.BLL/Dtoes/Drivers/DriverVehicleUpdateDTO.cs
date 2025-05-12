using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class DriverVehicleUpdateDTO
    {
        public List<VehicleUpdateDTO>? Vehicles { get; set; } = new List<VehicleUpdateDTO>();
    }
}
