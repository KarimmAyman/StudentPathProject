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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DriverService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment ,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostingEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
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
            // 1. Get existing user instead of creating new one
            var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(d => d.Id == driverDto.Id);

            if (driver == null)
            {
                throw new Exception($"Driver with ID {driverDto.Id} not found in Identity system");
            }

            // 2. Update the existing driver with additional info
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

            // 3. Create Upload Directory
            var uploadsPath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsPath);

            // 4. Upload Driver Documents
            driver.IdFrontPath = await SaveFile(driverDto.IdFront, uploadsPath, "Drivers");
            driver.IdBackPath = await SaveFile(driverDto.IdBack, uploadsPath, "Drivers");
            driver.CriminalRecordPath = await SaveFile(driverDto.CriminalRecord, uploadsPath, "Drivers");
            driver.LicenseFrontPath = await SaveFile(driverDto.LicenseFront, uploadsPath, "Drivers");
            driver.LicenseBackPath = await SaveFile(driverDto.LicenseBack, uploadsPath, "Drivers");
            driver.LicenseSelfiePath = await SaveFile(driverDto.LicenseSelfie, uploadsPath, "Drivers");

            // 5. Process Vehicles (with file form-data fallback)
            if (driverDto.VehicleAddDTOs != null)
            {
                for (int i = 0; i < driverDto.VehicleAddDTOs.Count; i++)
                {
                    var vehicleDto = driverDto.VehicleAddDTOs[i];

                    // Manually extract files using indexed form keys (e.g., VehiclePicture_0)
                    var vehiclePictureKey = $"VehiclePicture_{i}";
                    var vehicleRegFrontKey = $"VehicleRegistrationFront_{i}";
                    var vehicleRegBackKey = $"VehicleRegistrationBack_{i}";

                    var vehiclePicture = GetFormFileByKey(vehiclePictureKey);
                    var regFront = GetFormFileByKey(vehicleRegFrontKey);
                    var regBack = GetFormFileByKey(vehicleRegBackKey);

                    var vehicle = new VehicleInfo
                    {
                        VehicleBrand = vehicleDto.VehicleBrand,
                        VehicleModel = vehicleDto.VehicleModel,
                        VehicleColor = vehicleDto.VehicleColor,
                        ProductionYear = vehicleDto.ProductionYear,
                        PlateNumber = vehicleDto.PlateNumber,
                        SeatingCapacity = vehicleDto.SeatingCapacity,
                        DriverId = driver.Id,
                        VehiclePicturePath = await SaveFile(vehiclePicture, uploadsPath, "Vehicles"),
                        VehicleRegistrationFrontPath = await SaveFile(regFront, uploadsPath, "Vehicles"),
                        VehicleRegistrationBackPath = await SaveFile(regBack, uploadsPath, "Vehicles")
                    };

                    await _unitOfWork.VehicleInfo.CreateOrUpdateAsync(vehicle);
                    driver.VehicleInfo.Add(vehicle);
                }
            }

            // 6. Process Locations
            if (driverDto.Locations != null)
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
                }
            }

            // 7. Final Save
            await _unitOfWork.Driver.CreateOrUpdateAsync(driver);
            await _unitOfWork.Save();

            // 8. Fetch and Return Full Driver With Vehicles
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
    Directory.CreateDirectory(uploadsPath);

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

            // Handle file uploads for new vehicle
            newVehicle.VehiclePicturePath = vehicleDto.VehiclePicture != null 
                ? await SaveFile(vehicleDto.VehiclePicture, uploadsPath, "Vehicles") 
                : "default-vehicle.jpg";
                
            newVehicle.VehicleRegistrationFrontPath = vehicleDto.VehicleRegistrationFront != null 
                ? await SaveFile(vehicleDto.VehicleRegistrationFront, uploadsPath, "Vehicles") 
                : "default-registration-front.jpg";
                
            newVehicle.VehicleRegistrationBackPath = vehicleDto.VehicleRegistrationBack != null 
                ? await SaveFile(vehicleDto.VehicleRegistrationBack, uploadsPath, "Vehicles") 
                : "default-registration-back.jpg";

            driver.VehicleInfo.Add(newVehicle);
        }
        else
        {
            // Update existing vehicle
            _mapper.Map(vehicleDto, existingVehicle);

            // Handle file uploads for existing vehicle
            if (vehicleDto.VehiclePicture != null)
                existingVehicle.VehiclePicturePath = await SaveFile(vehicleDto.VehiclePicture, uploadsPath, "Vehicles");
                
            if (vehicleDto.VehicleRegistrationFront != null)
                existingVehicle.VehicleRegistrationFrontPath = await SaveFile(vehicleDto.VehicleRegistrationFront, uploadsPath, "Vehicles");
                
            if (vehicleDto.VehicleRegistrationBack != null)
                existingVehicle.VehicleRegistrationBackPath = await SaveFile(vehicleDto.VehicleRegistrationBack, uploadsPath, "Vehicles");
        }
    }

    // Remove vehicles not in the DTO
    var vehiclesToRemove = existingVehicles
        .Where(v => !driverDto.VehicleUpdateDTOs.Any(dto => dto.Id == v.VehicleInfoId))
        .ToList();
    
    foreach (var vehicle in vehiclesToRemove)
    {
        // Optional: Delete associated files before removing
        DeleteFileIfExists(vehicle.VehiclePicturePath);
        DeleteFileIfExists(vehicle.VehicleRegistrationFrontPath);
        DeleteFileIfExists(vehicle.VehicleRegistrationBackPath);
        
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

// Helper method to delete files
private void DeleteFileIfExists(string filePath)
{
    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
    {
        try
        {
            File.Delete(filePath);
        }
        catch
        {
            // Log error if needed
        }
    }
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
        public IFormFile GetFormFileByKey(string key)
        {
            return _httpContextAccessor.HttpContext?.Request.Form.Files.FirstOrDefault(f => f.Name == key);
        }
    }
}
