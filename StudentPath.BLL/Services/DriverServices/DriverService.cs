using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
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
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostingEnvironment = hostEnvironment;
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
            // Create upload directory
            var uploadsPath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsPath);

            // 1. Create and map Driver
            var driver = new Driver
            {
                // Personal Info
                UserName = driverDto.UserName,
                Email = driverDto.Email,
                PhoneNumber = driverDto.PhoneNumber,
                DateOfBirth = driverDto.DateOfBirth,
                Gender = driverDto.Gender,

                // Driver Documents
                IdNumber = driverDto.IdNumber,
                LicenseNumber = driverDto.LicenseNumber,
                LicenseExpiryDate = driverDto.LicenseExpiryDate,

                // System Properties
                UserType = UserTypeEnum.Driver,
                RegistrationDate = DateTime.UtcNow,
                Status = ApprovalStatus.Pending,
                IsDeleted = false,

                // Initialize collections
                VehicleInfo = new List<VehicleInfo>(),
                Locations = new List<Location>()
            };

            // 2. Handle Driver Document Uploads
            driver.IdFrontPath = await SaveFile(driverDto.IdFront, uploadsPath, "Drivers"); 
            driver.IdBackPath = await SaveFile(driverDto.IdBack, uploadsPath, "Drivers");
            driver.CriminalRecordPath = await SaveFile(driverDto.CriminalRecord, uploadsPath, "Drivers");
            driver.LicenseFrontPath = await SaveFile(driverDto.LicenseFront, uploadsPath, "Drivers");
            driver.LicenseBackPath = await SaveFile(driverDto.LicenseBack, uploadsPath, "Drivers");
            driver.LicenseSelfiePath = await SaveFile(driverDto.LicenseSelfie, uploadsPath, "Drivers");

            // 3. FIRST SAVE - Create driver to generate ID
            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();
            Console.WriteLine("Vehecle Count: " + driverDto.VehicleAddDTOs?.Count);

            // 4. Process vehicles through repository
            foreach (var vehicleDto in driverDto.VehicleAddDTOs)
            {
                var vehicle = new VehicleInfo
                {
                    // Vehicle Info
                    VehicleBrand = vehicleDto.VehicleBrand,
                    VehicleModel = vehicleDto.VehicleModel,
                    VehicleColor = vehicleDto.VehicleColor,
                    ProductionYear = vehicleDto.ProductionYear,
                    PlateNumber = vehicleDto.PlateNumber,
                    SeatingCapacity = vehicleDto.SeatingCapacity,

                    // Relationship
                    DriverId = driver.Id,

                    // Vehicle Documents
                    VehiclePicturePath = vehicleDto.VehiclePicture,
                    VehicleRegistrationFrontPath = vehicleDto.VehicleRegistrationFront,
                    VehicleRegistrationBackPath = vehicleDto.VehicleRegistrationBack
                };

                // Add vehicle through repository
                await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(vehicle);
            }

            // 5. Process locations
            if (driverDto.Locations != null)
            {
                Console.WriteLine($"Locations Count: {driverDto.Locations?.Count}");

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
                }
            }

            // 6. FINAL SAVE - Save all changes
            await _unitOfWork.Save();

            // 7. Return the complete driver with vehicles
            var driverWithVehicles = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == driver.Id,
                includeProperties: d => d.VehicleInfo
            );

            return _mapper.Map<DriverReadDTO>(driverWithVehicles);
        }

        public async Task<bool> UpdateDriverAsync(string id, DriverUpdateDTO driverDto)
        {
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: d => d.VehicleInfo
            );


            if (driver == null || driver.IsDeleted) return false;

            var uploadsPath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");

            // Update driver basic info
            _mapper.Map(driverDto, driver);

            // Handle driver document updates
            if (driverDto.IdFront != null)
                driver.IdFrontPath = await SaveFile(driverDto.IdFront, uploadsPath, "Drivers");
            if (driverDto.IdBack != null)
                driver.IdBackPath = await SaveFile(driverDto.IdBack, uploadsPath, "Drivers");
            if (driverDto.CriminalRecord != null)
                driver.CriminalRecordPath = await SaveFile(driverDto.CriminalRecord, uploadsPath, "Drivers");
            if (driverDto.LicenseFront != null)
                driver.LicenseFrontPath = await SaveFile(driverDto.LicenseFront, uploadsPath, "Drivers");
            if (driverDto.LicenseBack != null)
                driver.LicenseBackPath = await SaveFile(driverDto.LicenseBack, uploadsPath, "Drivers");
            if (driverDto.LicenseSelfie != null)
                driver.LicenseSelfiePath = await SaveFile(driverDto.LicenseSelfie, uploadsPath, "Drivers");

            // Handle vehicle updates
            var existingVehicles = driver.VehicleInfo.ToList();
            foreach (var vehicleDto in driverDto.VehicleUpdateDTOs)
            {
                var existingVehicle = existingVehicles.FirstOrDefault(v => v.VehicleInfoId == vehicleDto.Id);
                if (existingVehicle == null)
                {
                    // Add new vehicle
                    var newVehicle = _mapper.Map<VehicleInfo>(vehicleDto);
                    newVehicle.DriverId = driver.Id;

                    newVehicle.VehiclePicturePath = "placeholder.jpg";
                    newVehicle.VehicleRegistrationFrontPath = "placeholder.jpg";
                    newVehicle.VehicleRegistrationBackPath = "placeholder.jpg";

                    driver.VehicleInfo.Add(newVehicle);
                }
                else
                {
                    // Update existing vehicle
                    _mapper.Map(vehicleDto, existingVehicle);

                    if (vehicleDto.VehiclePicture != null)
                        existingVehicle.VehiclePicturePath = vehicleDto.VehiclePicture;
                    if (vehicleDto.VehicleRegistrationFront != null)
                        existingVehicle.VehicleRegistrationFrontPath = vehicleDto.VehicleRegistrationFront;
                    if (vehicleDto.VehicleRegistrationBack != null)
                        existingVehicle.VehicleRegistrationBackPath = vehicleDto.VehicleRegistrationBack;
                }
            }

            // Remove vehicles not in the DTO
            var vehiclesToRemove = existingVehicles
                .Where(v => !driverDto.VehicleUpdateDTOs.Any(dto => dto.Id == v.VehicleInfoId))
                .ToList();
            foreach (var vehicle in vehiclesToRemove)
            {
                driver.VehicleInfo.Remove(vehicle);
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

            // Remove locations not in the update DTO
            var locationsToRemove = existingLocations
                .Where(l => !driverDto.Locations.Any(dto => dto.City == l.City && dto.Country == l.Country))
                .ToList();

            foreach (var location in locationsToRemove)
            {
                driver.Locations.Remove(location);
            }

            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();
            return true;
        }

        private async Task<string?> SaveFile(IFormFile file, string basePath, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!validExtensions.Contains(extension))
                throw new Exception($"Invalid file type. Allowed types: {string.Join(", ", validExtensions)}");

            var folderPath = Path.Combine(basePath, subFolder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Path.Combine("Uploads", subFolder, fileName);
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
