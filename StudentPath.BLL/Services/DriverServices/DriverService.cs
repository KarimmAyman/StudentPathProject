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
using System.Linq.Expressions;
using System.Net.NetworkInformation;
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
                filter: d => !d.IsDeleted,
                orderBy: null,
                page: null,
                pageSize: 10,
                noTrack: false,
                includeProperties: [d => d.VehicleInfo, d => d.Locations]
            );

            return drivers.Select(driver => MapToDriverReadDTO(driver));
        }

        public async Task<DriverDetailsDTO?> GetDriverByIdAsync(string id)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                filter: d => d.Id == id && !d.IsDeleted,
                noTrack: false,
                includeProperties: [d => d.VehicleInfo, d => d.Locations]
            );

            if (driver == null) return null;

            return MapToDriverDetailsDTO(driver);
        }

        public async Task<DriverReadDTO> CreateDriverAsync(DriverAddDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == driverDto.Id);
            if (driver == null)
            {
                throw new Exception($"Driver with ID {driverDto.Id} not found in Identity system");
            }

            // Update driver properties
            driver.Age = driverDto.Age;
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

            var createdDriver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == driver.Id,
                includeProperties: [d => d.VehicleInfo, d => d.Locations]);
            return MapToDriverReadDTO(createdDriver);
        }

        public async Task<bool> UpdateDriverProfileAsync(string id, DriverUpdateDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: d => d.Locations);

            if (driver == null || driver.IsDeleted) return false;

            // Update properties using a dictionary-based approach
            var propertyUpdates = new Dictionary<string, Action>
            {
                { nameof(driverDto.UserName), () => driver.UserName = driverDto.UserName ?? driver.UserName },
                { nameof(driverDto.Email), () => driver.Email = driverDto.Email ?? driver.Email },
                { nameof(driverDto.PhoneNumber), () => driver.PhoneNumber = driverDto.PhoneNumber ?? driver.PhoneNumber },
                { nameof(driverDto.Age), () => driver.Age = driverDto.Age != 0 ? driverDto.Age : driver.Age }, // Fixed here
                { nameof(driverDto.IdNumber), () => driver.IdNumber = driverDto.IdNumber ?? driver.IdNumber },
                { nameof(driverDto.LicenseNumber), () => driver.LicenseNumber = driverDto.LicenseNumber ?? driver.LicenseNumber },
                { nameof(driverDto.LicenseExpiryDate), () => driver.LicenseExpiryDate = driverDto.LicenseExpiryDate ?? driver.LicenseExpiryDate }
            };

            foreach (var update in propertyUpdates)
            {
                if (driverDto.GetType().GetProperty(update.Key)?.GetValue(driverDto) != null)
                    update.Value();
            }

            // Handle document updates using a dictionary
            var documentUpdates = new Dictionary<string, Func<Task<string>>>
            {
                { nameof(driverDto.IdFront), () => _fileService.SaveFileAsync(driverDto.IdFront, "Drivers") },
                { nameof(driverDto.IdBack), () => _fileService.SaveFileAsync(driverDto.IdBack, "Drivers") },
                { nameof(driverDto.CriminalRecord), () => _fileService.SaveFileAsync(driverDto.CriminalRecord, "Drivers") },
                { nameof(driverDto.LicenseFront), () => _fileService.SaveFileAsync(driverDto.LicenseFront, "Drivers") },
                { nameof(driverDto.LicenseBack), () => _fileService.SaveFileAsync(driverDto.LicenseBack, "Drivers") },
                { nameof(driverDto.LicenseSelfie), () => _fileService.SaveFileAsync(driverDto.LicenseSelfie, "Drivers") },
                { nameof(driverDto.PersonalPhoto), () => _fileService.SaveFileAsync(driverDto.PersonalPhoto, "Drivers/ProfilePhotos") },
            };

            foreach (var doc in documentUpdates)
            {
                if (driverDto.GetType().GetProperty(doc.Key)?.GetValue(driverDto) != null)
                {
                    var path = await doc.Value();
                    driver.GetType().GetProperty($"{doc.Key}Path")?.SetValue(driver, path);
                }
            }

            // Update locations if provided
            if (driverDto.Locations?.Any() == true)
            {
                await UpdateDriverLocations(driver, driverDto.Locations);
            }

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();
            return true;
        }

        public async Task<bool> UpdateDriverVehiclesAsync(string id, DriverVehicleUpdateDTO vehicleDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: d => d.VehicleInfo);

            if (driver == null || driver.IsDeleted) return false;
            if (vehicleDto.Vehicles == null || !vehicleDto.Vehicles.Any()) return true;

            foreach (var vehicleUpdateDto in vehicleDto.Vehicles)
            {
                if (string.IsNullOrWhiteSpace(vehicleUpdateDto.PlateNumber))
                    throw new ArgumentException("PlateNumber is required for vehicle updates");

                var existingVehicle = driver.VehicleInfo.FirstOrDefault(v =>
                    v.DriverId == id && v.PlateNumber == vehicleUpdateDto.PlateNumber);

                var vehicle = existingVehicle ?? new VehicleInfo { DriverId = id, PlateNumber = vehicleUpdateDto.PlateNumber };

                // Update vehicle properties using a dictionary
                var vehicleUpdates = new Dictionary<string, Action>
                {
                    { nameof(vehicleUpdateDto.VehicleBrand), () => vehicle.VehicleBrand = vehicleUpdateDto.VehicleBrand ?? vehicle.VehicleBrand },
                    { nameof(vehicleUpdateDto.VehicleModel), () => vehicle.VehicleModel = vehicleUpdateDto.VehicleModel ?? vehicle.VehicleModel },
                    { nameof(vehicleUpdateDto.VehicleColor), () => vehicle.VehicleColor = vehicleUpdateDto.VehicleColor ?? vehicle.VehicleColor },
                    { nameof(vehicleUpdateDto.ProductionYear), () => vehicle.ProductionYear = vehicleUpdateDto.ProductionYear ?? vehicle.ProductionYear },
                    { nameof(vehicleUpdateDto.SeatingCapacity), () => vehicle.SeatingCapacity = vehicleUpdateDto.SeatingCapacity ?? vehicle.SeatingCapacity }
                };

                foreach (var update in vehicleUpdates)
                {
                    if (vehicleUpdateDto.GetType().GetProperty(update.Key)?.GetValue(vehicleUpdateDto) != null)
                        update.Value();
                }

                // Handle file updates
                var fileUpdates = new Dictionary<string, (Func<Task<string>> Save, string PathProperty)>
                {
                    { nameof(vehicleUpdateDto.VehiclePicture), (() => _fileService.SaveFileAsync(vehicleUpdateDto.VehiclePicture, "Vehicles"), nameof(vehicle.VehiclePicturePath)) },
                    { nameof(vehicleUpdateDto.VehicleRegistrationFront), (() => _fileService.SaveFileAsync(vehicleUpdateDto.VehicleRegistrationFront, "Vehicles"), nameof(vehicle.VehicleRegistrationFrontPath)) },
                    { nameof(vehicleUpdateDto.VehicleRegistrationBack), (() => _fileService.SaveFileAsync(vehicleUpdateDto.VehicleRegistrationBack, "Vehicles"), nameof(vehicle.VehicleRegistrationBackPath)) }
                };

                foreach (var file in fileUpdates)
                {
                    if (vehicleUpdateDto.GetType().GetProperty(file.Key)?.GetValue(vehicleUpdateDto) != null)
                    {
                        if (!string.IsNullOrEmpty(vehicle.GetType().GetProperty(file.Value.PathProperty)?.GetValue(vehicle)?.ToString()))
                            _fileService.DeleteFile(vehicle.GetType().GetProperty(file.Value.PathProperty)?.GetValue(vehicle)?.ToString());

                        var path = await file.Value.Save();
                        vehicle.GetType().GetProperty(file.Value.PathProperty)?.SetValue(vehicle, path);
                    }
                }

                if (existingVehicle == null)
                    driver.VehicleInfo.Add(vehicle);

                await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(vehicle);
            }

            await _unitOfWork.Save();
            return true;
        }

        public async Task<DashboardDto> GetDriverDashboardAsync(string driverId)
        {
            var dashboard = new DashboardDto();

            //// Get driver's wallet balance
            //var wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.DriverId == driverId);
            //dashboard.Balance = wallet?.Balance ?? 0;

            //// Calculate earnings summary (total earnings)
            //var payments = await _unitOfWork.Payments.GetAsync(
            //    p => p.Trip.DriverId == driverId && p.Status == PaymentStatus.Completed);
            //dashboard.EarningsSummary = payments.Sum(p => p.Amount);

            // Get driver entity to access Balance
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == driverId);
            dashboard.Balance = driver?.Balance ?? 0;

            // static earnings summary (gemy will replace with actual calculation from his work later)
            dashboard.EarningsSummary = 1234.56m;

            // Get completed trips count
            var completedTrips = await _unitOfWork.Trips.GetAsync(
                t => t.DriverId == driverId && t.Status == TripStatus.Completed);
            dashboard.CompletedTripsCount = completedTrips.Count();

            // Get weekly stats
            var startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endDate = startDate.AddDays(7);

            var weeklyTrips = await _unitOfWork.Trips.GetAsync(
                t => t.DriverId == driverId &&
                     t.Status == TripStatus.Completed &&
                     t.DepartureTime >= startDate &&
                     t.DepartureTime < endDate);

            dashboard.WeeklyStats = new WeeklyTripStatsDto
            {
                Sunday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Sunday),
                Monday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Monday),
                Tuesday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Tuesday),
                Wednesday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Wednesday),
                Thursday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Thursday),
                Friday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Friday),
                Saturday = weeklyTrips.Count(t => t.DepartureTime.DayOfWeek == DayOfWeek.Saturday)
            };

            return dashboard;
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
            var locationMap = locationDtos.ToDictionary(
                dto => (dto.Latitude, dto.Longitude),
                dto => dto);

            // Update or add locations
            driver.Locations = locationDtos.Select(dto =>
            {
                var location = existingLocations.FirstOrDefault(l =>
                    l.Latitude == dto.Latitude && l.Longitude == dto.Longitude)
                    ?? new Location { UserId = driver.Id };

                location.City = dto.City;
                location.Country = dto.Country;
                location.Latitude = dto.Latitude;
                location.Longitude = dto.Longitude;
                return location;
            }).ToList();

            // Remove locations not in DTO
            var locationsToRemove = existingLocations
                .Where(l => !locationMap.ContainsKey((l.Latitude, l.Longitude)))
                .ToList();

            foreach (var location in locationsToRemove)
            {
                driver.Locations.Remove(location); // Use ICollection.Remove instead of RemoveAll
            }
        }
        
        private DriverReadDTO MapToDriverReadDTO(Driver driver)
        {
            return new DriverReadDTO
            {
                Id = driver.Id,
                UserName = driver.UserName,
                Email = driver.Email,
                PhoneNumber = driver.PhoneNumber,
                Age = driver.Age,
                Gender = driver.Gender,
                ImgUrl = driver.ImgUrl,
                UserType = driver.UserType,
                IdNumber = driver.IdNumber,
                NationalIdFrontPath = driver.IdFrontPath,
                NationalIdBackPath = driver.IdBackPath,
                CriminalStatusRecordPath = driver.CriminalRecordPath,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpiryDate,
                LicenseFrontPath = driver.LicenseFrontPath,
                LicenseBackPath = driver.LicenseBackPath,
                SelfieWithLicensePath = driver.LicenseSelfiePath,
                Status = driver.Status,
                IsBanned = driver.IsBanned,
                IsDeleted = driver.IsDeleted,
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
                }).ToList() ?? []
            };
        }

        private DriverDetailsDTO MapToDriverDetailsDTO(Driver driver)
        {
            var baseDto = MapToDriverReadDTO(driver);
            return new DriverDetailsDTO
            {
                // Copy all base properties
                Id = baseDto.Id,
                UserName = baseDto.UserName,
                Email = baseDto.Email,
                PhoneNumber = baseDto.PhoneNumber,
                UserType = baseDto.UserType,
                Age = baseDto.Age,
                Gender = baseDto.Gender,
                ImgUrl = baseDto.ImgUrl,
                IdNumber = baseDto.IdNumber,
                NationalIdFrontPath = baseDto.NationalIdFrontPath,
                NationalIdBackPath = baseDto.NationalIdBackPath,
                CriminalStatusRecordPath = baseDto.CriminalStatusRecordPath,
                LicenseNumber = baseDto.LicenseNumber,
                LicenseExpirationDate = baseDto.LicenseExpirationDate,
                LicenseFrontPath = baseDto.LicenseFrontPath,
                LicenseBackPath = baseDto.LicenseBackPath,
                SelfieWithLicensePath = baseDto.SelfieWithLicensePath,
                Status = baseDto.Status,
                IsBanned = baseDto.IsBanned,
                IsDeleted = baseDto.IsDeleted,
                VehicleInfo = baseDto.VehicleInfo,
                Locations = baseDto.Locations,
            };
        }
    }
}