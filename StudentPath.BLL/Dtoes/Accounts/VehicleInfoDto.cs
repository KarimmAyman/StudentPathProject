using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Accounts
{
    public class VehicleInfoDto
    {
        // Vehicle Details
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public int ProductionYear { get; set; }
        public string PlateNumber { get; set; }
        public int SeatingCapacity { get; set; }

        // Vehicle Documents (as IFormFile)
        public IFormFile? VehiclePicture { get; set; }
        public IFormFile? VehicleRegistrationFront { get; set; }
        public IFormFile? VehicleRegistrationBack { get; set; }
    }
}
