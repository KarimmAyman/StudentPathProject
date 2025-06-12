using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.TripServices
{
    public class TripStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TripStatusBackgroundService> _logger;

        public TripStatusBackgroundService(
            IServiceProvider services,
            ILogger<TripStatusBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        // Update planned trips that should be active
                        var now = DateTime.UtcNow;
                        var plannedTrips = await unitOfWork.Trips.GetAsync(
                            t => t.Status == TripStatus.Planned &&
                                 t.DepartureTime <= now);

                        foreach (var trip in plannedTrips)
                        {
                            trip.Status = TripStatus.Active;
                            await unitOfWork.Trips.CreateOrUpdateAsync(trip);
                        }

                        // Update active trips that should be completed
                        var activeTrips = await unitOfWork.Trips.GetAsync(
                            t => t.Status == TripStatus.Active &&
                                 t.EstimatedArrivalTime < now);

                        foreach (var trip in activeTrips)
                        {
                            trip.Status = TripStatus.Completed;
                            await unitOfWork.Trips.CreateOrUpdateAsync(trip);
                        }

                        await unitOfWork.Save();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating trip statuses");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Run every 5 minutes
            }
        }
    }
}
