using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class DriverFaceVerficiationDTO
    {
        public string Id { get; set; }
        public string IdFrontPhotoUrl { get; set; }
        public string PersonalPhotoUrl { get; set; }
        public ApprovalStatus Status { get; set; } // Use the enum here
        public string StatusReason { get; set; }
    }
}
