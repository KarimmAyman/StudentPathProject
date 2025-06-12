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
    public interface IUnitOfWork
    {
        public IUserRepo User { get; }
        public IDriverRepo Driver { get; }  // Add Driver repository

        public IGenericRepo<VehicleInfo> VehicleInfo { get; }  // Add Driver repository
        public IGenericRepo<Location> Locations { get; }  // Add Driver repository
        //public IGenericRepo<Trip> Trips { get; }  // Add Trips repository
        public ITripRepository Trips { get; } // Changed from IGenericRepo<Trip> to ITripRepository
        public IGenericRepo<TripLocation> TripLocations { get; }  // Add TripLocations repository
        public IGenericRepo<Booking> Bookings { get; } // Added for Booking entity


        public Task Save();

    }
}
