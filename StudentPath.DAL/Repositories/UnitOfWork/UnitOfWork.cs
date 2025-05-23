using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.DriverRepository;
using StudentPath.DAL.Repositories.GenericRepository;
using StudentPath.DAL.Repositories.UserRepository;
using StudentPath.DAL.Repositories.TripRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentPathContext db;
        public IUserRepo User { get; private set; }
        public IDriverRepo Driver { get; private set; }  // Add Driver repository

        public IGenericRepo<VehicleInfo> VehicleInfo { get; private set; }  // Add Driver repository
        public IGenericRepo<Location> Locations { get; private set; }  // Add Driver repository
        public IGenericRepo<TripLocation> TripLocations { get; private set; }  // Add TripLocations repository
        public ITripRepository Trips { get; private set; } // Changed from IGenericRepo<Trip> to ITripRepository
        public IGenericRepo<Booking> Bookings { get; private set; } // Added for Booking entity


        public UnitOfWork(StudentPathContext _db)
        {
            this.db = _db;

            User = new UserRepo(db);
            Driver = new DriverRepo(db);  // Add Driver repository
            VehicleInfo = new GenericRepo<VehicleInfo>(db);  // Add this
            Locations = new GenericRepo<Location>(db);       // Add this
            Trips = new TripRepository.TripRepository(db);
            TripLocations = new GenericRepo<TripLocation>(db);       // Add this
            Bookings = new GenericRepo<Booking>(db);

        }


        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
