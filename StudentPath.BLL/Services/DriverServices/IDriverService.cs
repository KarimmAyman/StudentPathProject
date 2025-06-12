using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Drivers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.DriverServices
{
    public interface IDriverService
    {
        Task<IEnumerable<DriverReadDTO>> GetAllDriversAsync();
        Task<DriverDetailsDTO?> GetDriverByIdAsync(string id);
        Task<DriverReadDTO> CreateDriverAsync(DriverAddDTO driverDto);
        Task<bool> UpdateDriverProfileAsync(string id, DriverUpdateDTO driverDto);
        Task<bool> UpdateDriverVehiclesAsync(string id, DriverVehicleUpdateDTO vehicleDto);
        Task<DashboardDto> GetDriverDashboardAsync(string driverId);
        Task<bool> SoftDeleteDriverAsync(string id);
    }
}