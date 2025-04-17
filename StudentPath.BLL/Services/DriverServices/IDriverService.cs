using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes;

namespace StudentPath.BLL.Services.DriverServices
{
    public interface IDriverService
    {
        Task<IEnumerable<DriverReadDTO>> GetAllDriversAsync();
        Task<DriverDetailsDTO?> GetDriverByIdAsync(string id);
        Task<DriverReadDTO> CreateDriverAsync(DriverAddDTO driverDto);
        Task<bool> UpdateDriverAsync(string id, DriverUpdateDTO driverDto);

        Task<bool> SoftDeleteDriverAsync(string id);
       public IFormFile GetFormFileByKey(string key);
    }
}
