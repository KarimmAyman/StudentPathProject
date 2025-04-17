using StudentPath.BLL.Dtoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.AdminServices
{
    public interface IAdminService
    {
        Task<IEnumerable<DriverReadDTO>> GetPendingDriversAsync();
        Task<bool> ApproveDriverAsync(string driverId);
        Task<bool> DenyDriverAsync(string driverId);
        Task<bool> BanUserAsync(string userId);
        Task<bool> UnbanUserAsync(string userId);
    }
}
