using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace StudentPath.DAL.Repositories.TripRepository
{
    public class TripRepository : GenericRepo<Trip>, ITripRepository
    {
        private readonly StudentPathContext _context;

        public TripRepository(StudentPathContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trip>> GetUpcomingTripsAsync()
        {
            return await _context.Trips
                .Where(t => t.DepartureTime > DateTime.UtcNow)
                .OrderBy(t => t.DepartureTime)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.Driver)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> SearchTripsAsync(Expression<Func<Trip, bool>> predicate)
        {
            return await _context.Trips
                .Where(predicate)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.Driver)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId)
        {
            return await _context.Trips
                .Where(t => t.DriverId == driverId)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .ToListAsync();
        }
        public async Task<Trip> GetActiveTripByDriverIdAsync(string driverId)
        {
            var now = DateTime.UtcNow;
            return await _context.Trips
                .Where(t => t.DriverId == driverId && t.DepartureTime > now)
                .OrderBy(t => t.DepartureTime) // Ensures the earliest upcoming trip is returned
                .Include(t => t.FromLocation)  // Include related data for display
                .Include(t => t.ToLocation)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync();
        }
    }
}