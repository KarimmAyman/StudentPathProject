using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Services.DriverServices;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly StudentPathContext _context;
        private readonly IDriverService _driverService;

        public AdminService(
            UserManager<User> userManager,
            StudentPathContext context,
            IDriverService driverService)
        {
            _userManager = userManager;
            _context = context;
            _driverService = driverService;
        }

        public async Task<IEnumerable<DriverReadDTO>> GetPendingDriversAsync()
        {
            var pending = await _context.Set<Driver>()
                .Include(d => d.VehicleInfo)
                .Include(d => d.Locations)
                .Where(d => d.Status == ApprovalStatus.Pending)
                .ToListAsync();

            return pending.Select(d => new DriverReadDTO
            {
                Id = d.Id,
                UserName = d.UserName,
                Email = d.Email,
                PhoneNumber = d.PhoneNumber,
                Gender = d.Gender,
                NationalIdFrontPath = d.IdFrontPath,
                NationalIdBackPath = d.IdBackPath,
                CriminalStatusRecordPath = d.CriminalRecordPath,
                LicenseFrontPath = d.LicenseFrontPath,
                LicenseBackPath = d.LicenseBackPath,
                SelfieWithLicensePath = d.LicenseSelfiePath,
                LicenseNumber = d.LicenseNumber,
                LicenseExpirationDate = d.LicenseExpiryDate,
                Status = d.Status,
                VehicleInfo = d.VehicleInfo.Select(v => new VehicleInfoDto
                {
                    VehicleBrand = v.VehicleBrand,
                    PlateNumber = v.PlateNumber,
                    // …etc.
                }).ToList(),
                Locations = d.Locations.Select(l => new LocationDto
                {
                    City = l.City,
                    Country = l.Country,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList()
            });
        }


        public async Task<bool> ApproveDriverAsync(string driverId)
        {
            var driver = await _userManager.FindByIdAsync(driverId) as Driver;
            if (driver == null) return false;
            driver.Status = ApprovalStatus.Approved;
            var result = await _userManager.UpdateAsync(driver);
            return result.Succeeded;
        }

        public async Task<bool> DenyDriverAsync(string driverId)
        {
            var driver = await _userManager.FindByIdAsync(driverId) as Driver;
            if (driver == null) return false;
            driver.Status = ApprovalStatus.Denied;
            var result = await _userManager.UpdateAsync(driver);
            return result.Succeeded;
        }

        public async Task<bool> BanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            user.IsBanned = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnbanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            user.IsBanned = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}