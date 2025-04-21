using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.GenericRepository;
using System.Linq.Expressions;

namespace StudentPath.DAL.Repositories.TripRepository
{
    public interface ITripRepository : IGenericRepo<Trip>
    {
        Task<IEnumerable<Trip>> GetUpcomingTripsAsync();
        Task<IEnumerable<Trip>> SearchTripsAsync(Expression<Func<Trip, bool>> predicate);
        Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId);
    }
}