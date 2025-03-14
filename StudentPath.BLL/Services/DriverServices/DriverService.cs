using AutoMapper;
using StudentPath.BLL.Dtoes;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.DriverServices
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DriverReadDTO>> GetAllDriversAsync()
        {
            var drivers = await _unitOfWork.Driver.GetAsync(d => !d.IsDeleted, null, null, 10, false, d => d.VehicleInfo, d => d.Locations);
            return _mapper.Map<IEnumerable<DriverReadDTO>>(drivers);
        }

        public async Task<DriverDetailsDTO?> GetDriverByIdAsync(string id)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == id, false, d => d.VehicleInfo, d => d.Locations);
            if (driver == null || driver.IsDeleted) return null;

            return _mapper.Map<DriverDetailsDTO>(driver);
        }

        public async Task<DriverReadDTO> CreateDriverAsync(DriverAddDTO driverDto)
        {
            var driver = _mapper.Map<Driver>(driverDto);
            driver.UserType = UserTypeEnum.Driver;
            driver.IsDeleted = false;

            foreach (var vehicleInfoDto in driverDto.VehicleInfo)
            {
                var vehicleInfo = _mapper.Map<VehicleInfo>(vehicleInfoDto);
                vehicleInfo.DriverId = driver.Id;
            }

            foreach (var locationDto in driverDto.Locations)
            {
                var location = _mapper.Map<Location>(locationDto);
                location.UserId = driver.Id;
            }

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();

            return _mapper.Map<DriverReadDTO>(driver);
        }

        public async Task<bool> UpdateDriverAsync(string id, DriverUpdateDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == id);
            if (driver == null || driver.IsDeleted) return false;

            _mapper.Map(driverDto, driver);

            // Update VehicleInfo
            var existingVehicleInfos = driver.VehicleInfo.ToList();
            foreach (var vehicleInfoDto in driverDto.VehicleInfo)
            {
                var vehicleInfo = existingVehicleInfos.FirstOrDefault(v => v.VehicleType == vehicleInfoDto.VehicleType && v.LicensePlate == vehicleInfoDto.LicensePlate);
                if (vehicleInfo == null)
                {
                    vehicleInfo = _mapper.Map<VehicleInfo>(vehicleInfoDto);
                    vehicleInfo.DriverId = driver.Id;
                    driver.VehicleInfo.Add(vehicleInfo);
                }
                else
                {
                    _mapper.Map(vehicleInfoDto, vehicleInfo);
                }
            }

            // Remove VehicleInfo that are not in the new list
            var vehicleInfoToRemove = existingVehicleInfos.Where(v => !driverDto.VehicleInfo.Any(dto => dto.VehicleType == v.VehicleType && dto.LicensePlate == v.LicensePlate)).ToList();
            foreach (var vehicleInfo in vehicleInfoToRemove)
            {
                driver.VehicleInfo.Remove(vehicleInfo);
            }

            // Update Locations
            var existingLocations = driver.Locations.ToList();
            foreach (var locationDto in driverDto.Locations)
            {
                var location = existingLocations.FirstOrDefault(l => l.City == locationDto.City && l.Country == locationDto.Country);
                if (location == null)
                {
                    location = _mapper.Map<Location>(locationDto);
                    location.UserId = driver.Id;
                    driver.Locations.Add(location);
                }
                else
                {
                    _mapper.Map(locationDto, location);
                }
            }

            // Remove Locations that are not in the new list
            var locationsToRemove = existingLocations.Where(l => !driverDto.Locations.Any(dto => dto.City == l.City && dto.Country == l.Country)).ToList();
            foreach (var location in locationsToRemove)
            {
                driver.Locations.Remove(location);
            }

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();
            return true;
        }

        public async Task<bool> SoftDeleteDriverAsync(string id)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == id);
            if (driver == null || driver.IsDeleted) return false;

            await _unitOfWork.Driver.SoftDeleteAsync(driver);
            await _unitOfWork.Save();
            return true;
        }
    }
}
