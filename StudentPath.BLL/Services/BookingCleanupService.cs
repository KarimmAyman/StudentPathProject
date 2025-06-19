using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCleanupService> _logger; // Add logger

        public BookingCleanupService(IServiceProvider serviceProvider, ILogger<BookingCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DeleteExpiredBookings();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in BookingCleanupService: {Message}", ex.Message);
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            }
        }

        private async Task DeleteExpiredBookings()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StudentPathContext>();

            try
            {

                var expirationTime = DateTime.UtcNow.AddMinutes(-10);
                var minBookingDate = DateTime.UtcNow.AddHours(-12);

                var unpaidBookings = await dbContext.Bookings
                    .Where(b => b.PaymentStatus == PaymentStatus.Pending &&b.BookingStatus==BookingStatus.Pending&& b.BookingDate < expirationTime && b.BookingDate >= minBookingDate)
                    .ToListAsync();

                   if (unpaidBookings.Any())
                  {
                    foreach (var booking in unpaidBookings)
                    {
                        if (booking.Trip != null)
                        {
                            booking.Trip.AvailableSeats += booking.NumberOfSeats; // return seats
                        }
                    }
                    var bookingIds = unpaidBookings.Select(b => b.BookingId).ToList();

                    var relatedPayments = await dbContext.Payments
                        .Where(p => p.BookingId.HasValue && bookingIds.Contains(p.BookingId.Value))
                        .ToListAsync();

                    if (relatedPayments.Any())
                    {
                        dbContext.Payments.RemoveRange(relatedPayments);
                    }

                    dbContext.Bookings.RemoveRange(unpaidBookings);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Deleted {Count} expired bookings.", unpaidBookings.Count);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete expired bookings: {Message}", ex.Message);
                throw; // Re-throw to maintain default behavior; remove if setting Ignore
            }
        }
    }
}
