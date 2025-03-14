using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.Dtoes
{
    public class DriverReadDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public GenderType Gender { get; set; }
        public ApprovalStatus? Status { get; set; }
        public List<VehicleInfoDto> VehicleInfo { get; set; } = new List<VehicleInfoDto>();
        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
    }
}
