using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.DriverServices
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public DriverService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<DriverReadDTO>> GetAllDriversAsync()
        {
            var drivers = await _unitOfWork.Driver.GetAsync(
                d => !d.IsDeleted,
                null, null, 10, false,
                d => d.VehicleInfo,
                d => d.Locations
            );

            return drivers.Select(driver => new DriverReadDTO
            {
                Id = driver.Id,
                UserName = driver.UserName,
                Email = driver.Email,
                PhoneNumber = driver.PhoneNumber,
                Gender = driver.Gender,
                NationalIdFrontPath = driver.IdFrontPath,
                NationalIdBackPath = driver.IdBackPath,
                CriminalStatusRecordPath = driver.CriminalRecordPath,
                LicenseFrontPath = driver.LicenseFrontPath,
                LicenseBackPath = driver.LicenseBackPath,
                SelfieWithLicensePath = driver.LicenseSelfiePath,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpiryDate,
                Status = driver.Status,
                VehicleInfo = driver.VehicleInfo?.Select(v => new VehicleReadDTO
                {
                    Id = v.VehicleInfoId,
                    VehicleBrand = v.VehicleBrand,
                    VehicleModel = v.VehicleModel,
                    VehicleColor = v.VehicleColor,
                    ProductionYear = v.ProductionYear,
                    PlateNumber = v.PlateNumber,
                    SeatingCapacity = v.SeatingCapacity,
                    VehiclePicture = v.VehiclePicturePath,
                    VehicleRegistrationFront = v.VehicleRegistrationFrontPath,
                    VehicleRegistrationBack = v.VehicleRegistrationBackPath
                }).ToList() ?? new List<VehicleReadDTO>(),
                Locations = driver.Locations?.Select(l => new LocationDto
                {
                    City = l.City,
                    Country = l.Country,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList() ?? new List<LocationDto>()
            });
        }

        public async Task<DriverDetailsDTO?> GetDriverByIdAsync(string id)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                false,
                d => d.VehicleInfo,
                d => d.Locations);

            if (driver == null || driver.IsDeleted) return null;

            return new DriverDetailsDTO
            {
                UserName = driver.UserName,
                Email = driver.Email,
                Age = CalculateAge(driver.DateOfBirth),
                Gender = driver.Gender,
                NationalIdFrontPath = driver.IdFrontPath,
                NationalIdBackPath = driver.IdBackPath,
                CriminalStatusRecordPath = driver.CriminalRecordPath,
                LicenseFrontPath = driver.LicenseFrontPath,
                LicenseBackPath = driver.LicenseBackPath,
                SelfieWithLicensePath = driver.LicenseSelfiePath,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpiryDate,
                Status = driver.Status,
                IsBanned = driver.IsBanned,
                IsDeleted = driver.IsDeleted,
                RegistrationDate = driver.RegistrationDate,
                VehicleInfo = driver.VehicleInfo?.Select(v => new VehicleReadDTO
                {
                    Id = v.VehicleInfoId,
                    VehicleBrand = v.VehicleBrand,
                    VehicleModel = v.VehicleModel,
                    VehicleColor = v.VehicleColor,
                    ProductionYear = v.ProductionYear,
                    PlateNumber = v.PlateNumber,
                    SeatingCapacity = v.SeatingCapacity,
                    VehiclePicture = v.VehiclePicturePath,
                    VehicleRegistrationFront = v.VehicleRegistrationFrontPath,
                    VehicleRegistrationBack = v.VehicleRegistrationBackPath
                }).ToList() ?? new List<VehicleReadDTO>(),
                Locations = driver.Locations?.Select(l => new LocationDto
                {
                    City = l.City,
                    Country = l.Country,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList() ?? new List<LocationDto>()
            };
        }

        public async Task<DriverReadDTO> CreateDriverAsync(DriverAddDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == driverDto.Id);
            if (driver == null)
            {
                throw new Exception($"Driver with ID {driverDto.Id} not found in Identity system");
            }

            // Update driver properties
            driver.DateOfBirth = driverDto.DateOfBirth;
            driver.Gender = driverDto.Gender;
            driver.IdNumber = driverDto.IdNumber;
            driver.LicenseNumber = driverDto.LicenseNumber;
            driver.LicenseExpiryDate = driverDto.LicenseExpiryDate;
            driver.Status = ApprovalStatus.Pending;
            driver.IsDeleted = false;
            driver.RegistrationDate = DateTime.UtcNow;
            driver.VehicleInfo = new List<VehicleInfo>();
            driver.Locations = new List<Location>();

            // Handle file uploads
            driver.IdFrontPath = await _fileService.SaveFileAsync(driverDto.IdFront, "Drivers");
            driver.IdBackPath = await _fileService.SaveFileAsync(driverDto.IdBack, "Drivers");
            driver.CriminalRecordPath = await _fileService.SaveFileAsync(driverDto.CriminalRecord, "Drivers");
            driver.LicenseFrontPath = await _fileService.SaveFileAsync(driverDto.LicenseFront, "Drivers");
            driver.LicenseBackPath = await _fileService.SaveFileAsync(driverDto.LicenseBack, "Drivers");
            driver.LicenseSelfiePath = await _fileService.SaveFileAsync(driverDto.LicenseSelfie, "Drivers");

            // Process vehicles
            if (driverDto.VehicleAddDTOs != null && driverDto.VehicleAddDTOs.Any())
            {
                foreach (var vehicleDto in driverDto.VehicleAddDTOs)
                {
                    var vehicle = new VehicleInfo
                    {
                        VehicleBrand = vehicleDto.VehicleBrand,
                        VehicleModel = vehicleDto.VehicleModel,
                        VehicleColor = vehicleDto.VehicleColor,
                        ProductionYear = vehicleDto.ProductionYear,
                        PlateNumber = vehicleDto.PlateNumber,
                        SeatingCapacity = vehicleDto.SeatingCapacity,
                        DriverId = driver.Id,
                        VehiclePicturePath = await _fileService.SaveFileAsync(vehicleDto.VehiclePicture, "Vehicles"),
                        VehicleRegistrationFrontPath = await _fileService.SaveFileAsync(vehicleDto.VehicleRegistrationFront, "Vehicles"),
                        VehicleRegistrationBackPath = await _fileService.SaveFileAsync(vehicleDto.VehicleRegistrationBack, "Vehicles")
                    };
                    await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(vehicle);
                    driver.VehicleInfo.Add(vehicle);
                }
            }

            // Process locations
            if (driverDto.Locations != null && driverDto.Locations.Any())
            {
                foreach (var locationDto in driverDto.Locations)
                {
                    var location = new Location
                    {
                        City = locationDto.City,
                        Country = locationDto.Country,
                        Latitude = locationDto.Latitude,
                        Longitude = locationDto.Longitude,
                        UserId = driver.Id
                    };
                    await _unitOfWork.Locations.CreateOrUpdateAsync(location);
                    driver.Locations.Add(location);
                }
            }

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();

            var driverWithVehicles = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == driver.Id,
                includeProperties: d => d.VehicleInfo);

            return new DriverReadDTO
            {
                Id = driverWithVehicles.Id,
                UserName = driverWithVehicles.UserName,
                Email = driverWithVehicles.Email,
                PhoneNumber = driverWithVehicles.PhoneNumber,
                Gender = driverWithVehicles.Gender,
                NationalIdFrontPath = driverWithVehicles.IdFrontPath,
                NationalIdBackPath = driverWithVehicles.IdBackPath,
                CriminalStatusRecordPath = driverWithVehicles.CriminalRecordPath,
                LicenseFrontPath = driverWithVehicles.LicenseFrontPath,
                LicenseBackPath = driverWithVehicles.LicenseBackPath,
                SelfieWithLicensePath = driverWithVehicles.LicenseSelfiePath,
                LicenseNumber = driverWithVehicles.LicenseNumber,
                LicenseExpirationDate = driverWithVehicles.LicenseExpiryDate,
                Status = driverWithVehicles.Status,
                VehicleInfo = driverWithVehicles.VehicleInfo?.Select(v => new VehicleReadDTO
                {
                    Id = v.VehicleInfoId,
                    VehicleBrand = v.VehicleBrand,
                    VehicleModel = v.VehicleModel,
                    VehicleColor = v.VehicleColor,
                    ProductionYear = v.ProductionYear,
                    PlateNumber = v.PlateNumber,
                    SeatingCapacity = v.SeatingCapacity,
                    VehiclePicture = v.VehiclePicturePath,
                    VehicleRegistrationFront = v.VehicleRegistrationFrontPath,
                    VehicleRegistrationBack = v.VehicleRegistrationBackPath
                }).ToList() ?? new List<VehicleReadDTO>(),
                Locations = driverWithVehicles.Locations?.Select(l => new LocationDto
                {
                    City = l.City,
                    Country = l.Country,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList() ?? new List<LocationDto>()
            };
        }

        public async Task<bool> UpdateDriverProfileAsync(string id, DriverUpdateDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: d => d.Locations);

            if (driver == null || driver.IsDeleted) return false;

            // Update basic driver properties
            driver.UserName = driverDto.UserName;
            driver.Email = driverDto.Email;
            driver.PhoneNumber = driverDto.PhoneNumber;
            driver.DateOfBirth = driverDto.DateOfBirth;
            driver.IdNumber = driverDto.IdNumber;
            driver.LicenseNumber = driverDto.LicenseNumber;
            driver.LicenseExpiryDate = driverDto.LicenseExpiryDate;

            // Handle document updates
            if (driverDto.IdFront != null)
                driver.IdFrontPath = await _fileService.SaveFileAsync(driverDto.IdFront, "Drivers");
            if (driverDto.IdBack != null)
                driver.IdBackPath = await _fileService.SaveFileAsync(driverDto.IdBack, "Drivers");
            if (driverDto.CriminalRecord != null)
                driver.CriminalRecordPath = await _fileService.SaveFileAsync(driverDto.CriminalRecord, "Drivers");
            if (driverDto.LicenseFront != null)
                driver.LicenseFrontPath = await _fileService.SaveFileAsync(driverDto.LicenseFront, "Drivers");
            if (driverDto.LicenseBack != null)
                driver.LicenseBackPath = await _fileService.SaveFileAsync(driverDto.LicenseBack, "Drivers");
            if (driverDto.LicenseSelfie != null)
                driver.LicenseSelfiePath = await _fileService.SaveFileAsync(driverDto.LicenseSelfie, "Drivers");

            // Update locations
            await UpdateDriverLocations(driver, driverDto.Locations);

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();
            return true;
        }


        public async Task<bool> UpdateDriverVehiclesAsync(string id, DriverVehicleUpdateDTO vehicleDto)
        {
            // Get driver with their vehicles
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: d => d.VehicleInfo);

            if (driver == null || driver.IsDeleted) return false;

            foreach (var vehicleUpdateDto in vehicleDto.Vehicles)
            {
                // Validate required fields for composite key
                if (string.IsNullOrWhiteSpace(vehicleUpdateDto.PlateNumber))
                {
                    throw new ArgumentException("PlateNumber is required for vehicle updates");
                }

                // Find existing vehicle by composite key (DriverId + PlateNumber)
                var existingVehicle = driver.VehicleInfo.FirstOrDefault(v =>
                    v.DriverId == id &&
                    v.PlateNumber == vehicleUpdateDto.PlateNumber);

                if (existingVehicle == null)
                {
                    // Create new vehicle if not found
                    var newVehicle = new VehicleInfo
                    {
                        DriverId = id, // Part of composite key
                        PlateNumber = vehicleUpdateDto.PlateNumber, // Part of composite key
                        VehicleBrand = vehicleUpdateDto.VehicleBrand,
                        VehicleModel = vehicleUpdateDto.VehicleModel,
                        VehicleColor = vehicleUpdateDto.VehicleColor,
                        ProductionYear = vehicleUpdateDto.ProductionYear,
                        SeatingCapacity = vehicleUpdateDto.SeatingCapacity
                    };

                    // Handle file uploads for new vehicle
                    if (vehicleUpdateDto.VehiclePicture != null)
                    {
                        newVehicle.VehiclePicturePath = await _fileService.SaveFileAsync(
                            vehicleUpdateDto.VehiclePicture, "Vehicles");
                    }

                    if (vehicleUpdateDto.VehicleRegistrationFront != null)
                    {
                        newVehicle.VehicleRegistrationFrontPath = await _fileService.SaveFileAsync(
                            vehicleUpdateDto.VehicleRegistrationFront, "Vehicles");
                    }

                    if (vehicleUpdateDto.VehicleRegistrationBack != null)
                    {
                        newVehicle.VehicleRegistrationBackPath = await _fileService.SaveFileAsync(
                            vehicleUpdateDto.VehicleRegistrationBack, "Vehicles");
                    }

                    await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(newVehicle);
                    driver.VehicleInfo.Add(newVehicle);
                    continue;
                }

                // Update existing vehicle (maintain composite key values)
                existingVehicle.VehicleBrand = vehicleUpdateDto.VehicleBrand;
                existingVehicle.VehicleModel = vehicleUpdateDto.VehicleModel;
                existingVehicle.VehicleColor = vehicleUpdateDto.VehicleColor;
                existingVehicle.ProductionYear = vehicleUpdateDto.ProductionYear;
                existingVehicle.SeatingCapacity = vehicleUpdateDto.SeatingCapacity;

                // Only update files if new ones are provided
                if (vehicleUpdateDto.VehiclePicture != null)
                {
                    if (!string.IsNullOrEmpty(existingVehicle.VehiclePicturePath))
                    {
                        _fileService.DeleteFile(existingVehicle.VehiclePicturePath);
                    }
                    existingVehicle.VehiclePicturePath = await _fileService.SaveFileAsync(
                        vehicleUpdateDto.VehiclePicture, "Vehicles");
                }

                if (vehicleUpdateDto.VehicleRegistrationFront != null)
                {
                    if (!string.IsNullOrEmpty(existingVehicle.VehicleRegistrationFrontPath))
                    {
                        _fileService.DeleteFile(existingVehicle.VehicleRegistrationFrontPath);
                    }
                    existingVehicle.VehicleRegistrationFrontPath = await _fileService.SaveFileAsync(
                        vehicleUpdateDto.VehicleRegistrationFront, "Vehicles");
                }

                if (vehicleUpdateDto.VehicleRegistrationBack != null)
                {
                    if (!string.IsNullOrEmpty(existingVehicle.VehicleRegistrationBackPath))
                    {
                        _fileService.DeleteFile(existingVehicle.VehicleRegistrationBackPath);
                    }
                    existingVehicle.VehicleRegistrationBackPath = await _fileService.SaveFileAsync(
                        vehicleUpdateDto.VehicleRegistrationBack, "Vehicles");
                }

                await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(existingVehicle);
            }

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

        private async Task UpdateDriverLocations(Driver driver, List<LocationDto> locationDtos)
        {
            var existingLocations = driver.Locations.ToList();

            foreach (var locationDto in locationDtos)
            {
                var location = existingLocations.FirstOrDefault(l =>
                    l.Longitude == locationDto.Longitude &&
                    l.Latitude == locationDto.Latitude);

                if (location == null)
                {
                    location = new Location
                    {
                        City = locationDto.City,
                        Country = locationDto.Country,
                        Latitude = locationDto.Latitude,
                        Longitude = locationDto.Longitude,
                        UserId = driver.Id
                    };
                    driver.Locations.Add(location);
                }
                else
                {
                    location.City = locationDto.City;
                    location.Country = locationDto.Country;
                    location.Latitude = locationDto.Latitude;
                    location.Longitude = locationDto.Longitude;
                }
            }

            // Remove locations not in DTO
            var locationsToRemove = existingLocations
                .Where(l => !locationDtos.Any(dto =>
                    dto.Latitude == l.Latitude &&
                    dto.Longitude == l.Longitude))
                .ToList();

            foreach (var location in locationsToRemove)
            {
                driver.Locations.Remove(location);
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}