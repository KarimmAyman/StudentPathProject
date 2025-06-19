using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Recommendations;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.RecommendationServices
{
    public class RecommendationService
    {
        private readonly StudentPathContext context;
        private readonly HttpClient httpClient;

        public RecommendationService(StudentPathContext context,HttpClient httpClient)
        {
            this.context = context;
            this.httpClient = httpClient;
        }



        public async Task<IEnumerable<RecommendedTrip>> GetRecommendationsAsync(RecommendationRequestDTO request, int? id = null)
        {
            // 1. Validate input
            if (string.IsNullOrEmpty(request.UserId))
                throw new ArgumentException("UserId is required.");
            if (float.IsNaN(request.Latitude) || float.IsNaN(request.Longitude))
                throw new ArgumentException("Invalid latitude or longitude.");

            // 2. Fetch user's past and upcoming trips
            var pastTrips = await context.Trips
             .Join(context.Bookings,
                 t => t.TripId,
                 b => b.TripId,
                 (t, b) => new { Trip = t, Booking = b })
             .Where(x => x.Booking.UserId == request.UserId && x.Trip.DepartureTime <= DateTime.UtcNow)
             .Select(x => x.Trip)
             .ToListAsync();

            var upcomingTrips = await context.Trips
                .Where(t => t.DepartureTime >= DateTime.UtcNow)
                .ToListAsync();

            // 3. Fetch all relevant locations
            var locationIds = pastTrips.Concat(upcomingTrips)
                .SelectMany(t => new[] { t.FromLocationId, t.ToLocationId })
                .Distinct()
                .ToList();

            var locations = await context.TripLocations
                .Where(l => locationIds.Contains(l.Id))
                .ToListAsync();

            // 4. Validate data
            if (!locations.Any())
                throw new InvalidOperationException("No locations found for the trips.");
            var missingLocationIds = locationIds.Except(locations.Select(l => l.Id)).ToList();
            if (missingLocationIds.Any())
                throw new InvalidOperationException($"Missing locations for IDs: {string.Join(", ", missingLocationIds)}");

            // 5. Map to AI model
            var modelRequest = new AiRecommendationRequest
            {
                UserId = request.UserId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PastTrips = pastTrips.Select(t => MapToAiTrip(t)).ToList(),
                UpcomingTrips = upcomingTrips.Select(t => MapToAiTrip(t)).ToList(),
                Locations = locations.Select(l => new AiLocation
                {
                    Id = l.Id,
                    Latitude = (float)l.Latitude,
                    Longitude = (float)l.Longitude
                }).ToList()
            };

            // 6. Call AI model
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
            var response = await httpClient.PostAsJsonAsync("https://recommend-trips.onrender.com/recommend_trips", modelRequest, jsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Model Error: {response.StatusCode}, Details: {errorContent}");
            }

            // Debug response
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AI Model Error: {response.StatusCode}, Details: {errorContent}");
                throw new HttpRequestException($"AI Model Error: {response.StatusCode}, Details: {errorContent}");
            }

            // Debug response
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}, Content-Type: {response.Content.Headers.ContentType}, Content: {responseContent}");

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                Console.WriteLine("Empty response received from AI model.");
                return new List<RecommendedTrip>();
            }

            try
            {
                var aiResponse = await response.Content.ReadFromJsonAsync<AiRecommendationResponse>(jsonOptions);
                if (aiResponse == null || aiResponse.RecommendedTrips == null)
                {
                    Console.WriteLine("Deserialization returned null or no recommended trips.");
                    return new List<RecommendedTrip>();
                }

                // Fetch trip details from database
                var tripIds = aiResponse.RecommendedTrips; // e.g., [5,7,8,6]
                var trips = await context.Trips
                    .Where(t => tripIds.Contains(t.TripId))
                    .Include(t => t.FromLocation)
                    .Include(t => t.ToLocation)
                    .Include(t => t.Driver)
                    .ToDictionaryAsync(t => t.TripId); // Create lookup by TripId

                var locationsDict = await context.TripLocations
                    .Where(l => trips.Values.Select(t => t.FromLocationId).Concat(trips.Values.Select(t => t.ToLocationId)).Contains(l.Id))
                    .ToDictionaryAsync(l => l.Id);

                // Fetch vehicle info
                var vehicleDict = await context.vehicleInfos
                    .Where(v => trips.Values.Select(t => t.DriverId).Contains(v.DriverId))
                    .GroupBy(v => v.DriverId)
                    .ToDictionaryAsync(g => g.Key, g => g.FirstOrDefault());

                // Map to RecommendedTrip in AI model order
                var recommendedTrips = new List<RecommendedTrip>();
                foreach (var tripId in tripIds)
                {
                    if (trips.TryGetValue(tripId, out var trip))
                    {
                        recommendedTrips.Add(MapToRecommendedTrip(trip, locationsDict, vehicleDict));
                    }
                    else
                    {
                        Console.WriteLine($"Trip ID {tripId} not found in database.");
                    }
                }

                // Log date fields
                foreach (var trip in recommendedTrips)
                {
                    Console.WriteLine($"Trip ID {trip.Id}: CreatedAt={trip.CreatedAt}, FormattedDepartureTime={trip.BasicInfo?.FormattedDepartureTime}");
                }

                // Filter by Id if provided
                if (id.HasValue)
                {
                    var trip = recommendedTrips.FirstOrDefault(t => t.Id == id.Value);
                    Console.WriteLine($"Trip with ID {id.Value} {(trip != null ? "found" : "not found")}");
                    return trip != null ? new[] { trip } : Enumerable.Empty<RecommendedTrip>();
                }

                return recommendedTrips;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}, Response: {responseContent}");
                throw;
            }
        }

        


        private static AiTrip MapToAiTrip(Trip trip)
        {
            return new AiTrip
            {
                TripId = trip.TripId,
                FromLocationId = trip.FromLocationId,
                ToLocationId = trip.ToLocationId,
                PricePerSeat = (float)trip.PricePerSeat,
                EstimatedDistance = trip.EstimatedDistance.HasValue ? (float)trip.EstimatedDistance.Value : 0f,
                EstimatedDuration = trip.EstimatedDuration.HasValue ? (float)trip.EstimatedDuration.Value.TotalHours : 0f,
                DepartureTime = trip.DepartureTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                HasAirConditioning = trip.HasAirConditioning ? 1 : 0,
                HasFreeWater = trip.HasFreeWater ? 1 : 0,
                HasMusic = trip.HasMusic ? 1 : 0,
                HasPhoneCharger = trip.HasPhoneCharger ? 1 : 0,
                HasWiFi = trip.HasWiFi ? 1 : 0,
                AvailableSeats = trip.AvailableSeats
            };
        }
        private RecommendedTrip MapToRecommendedTrip(Trip trip, Dictionary<int, TripLocation> locationsDict, Dictionary<string, VehicleInfo> vehicleDict)
        {
            var fromLocation = locationsDict.GetValueOrDefault(trip.FromLocationId);
            var toLocation = locationsDict.GetValueOrDefault(trip.ToLocationId);

            VehicleInfo vehicle = null;

            if (!string.IsNullOrEmpty(trip.DriverId) && vehicleDict.TryGetValue(trip.DriverId, out var foundVehicle))
            {
                vehicle = foundVehicle;
            }
            var additionalInfo = new AdditionalInfoDTO
            {
                StartingPoint = fromLocation?.DisplayName ?? "Unknown",
                Notes = trip.DriverNotes,
                Amenities = new List<string>()
                    .AppendIf(trip.HasWiFi, "WiFi")
                    .AppendIf(trip.HasMusic, "Music")
                    .AppendIf(trip.HasPhoneCharger, "Phone Charger")
                    .AppendIf(trip.HasAirConditioning, "Air Conditioning")
                    .AppendIf(trip.HasFreeWater, "Free Water")
            };

            return new RecommendedTrip
            {
                Id = trip.TripId,
                FromLocation = new LocationInfo
                {
                    Latitude = (float)(fromLocation?.Latitude ?? 0),
                    Longitude = (float)(fromLocation?.Longitude ?? 0),
                    DisplayName = fromLocation?.DisplayName ?? "Unknown",
                    FullAddress = fromLocation?.FullAddress ?? "Unknown"
                },
                ToLocation = new LocationInfo
                {
                    Latitude = (float)(toLocation?.Latitude ?? 0),
                    Longitude = (float)(toLocation?.Longitude ?? 0),
                    DisplayName = toLocation?.DisplayName ?? "Unknown",
                    FullAddress = toLocation?.FullAddress ?? "Unknown"
                },
                BasicInfo = new BasicInfo
                {
                    FormattedDepartureTime = TimeZoneInfo.ConvertTimeFromUtc(
                   trip.DepartureTime,
                 TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"))
              .ToString("yyyy-MM-dd HH:mm"), FormattedDistance = trip.EstimatedDistance.HasValue ? $"{trip.EstimatedDistance.Value:F0} KM" : "N/A",
                    FormattedDuration = trip.EstimatedDuration.HasValue ? $"{trip.EstimatedDuration.Value.TotalMinutes:F0} min" : "N/A",
                    AvailableSeats = trip.AvailableSeats
                },
                DriverInfo = new DriverInfo
                {
                    DriverName = trip.Driver?.UserName,
                    PersonalPhotoPath = trip.Driver.ImgUrl,
                 DriverPhone = trip.Driver?.PhoneNumber,
                    VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                          .Where(v => v.DriverId == trip.DriverId).
                     Select(v => new VehicleInfoDTO
                     {
                         VehicleModel = v.VehicleModel,
                         SeatingCapacity = v.SeatingCapacity,

                         PlateNumber = v.PlateNumber

                     })
                    .FirstOrDefault()
                },
                AdditionalInfo = additionalInfo,
                PricePerSeat = trip.PricePerSeat,
                CreatedAt = trip.CreatedAt,
                Status = trip.Status
            };
        }
    }



    public static class ListExtensions
    {
        public static List<T> AppendIf<T>(this List<T> list, bool condition, T value)
        {
            if (condition) list.Add(value);
            return list;
        }
    }
}


