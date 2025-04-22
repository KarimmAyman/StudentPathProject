using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.DAL.Data.Models;
using StudentPath.DAL.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.TripServices
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<TripResponseDto>> CreateTripAsync(TripCreateDto dto, string driverId)
        {
            try
            {
                // Validate driver exists
                var driver = await _unitOfWork.Driver.GetFirstOrDefaultAsync(u => u.Id == driverId);
                if (driver == null)
                    return ApiResponse<TripResponseDto>.ErrorResponse("Driver not found", 404);

                // Create locations
                var fromLocation = new TripLocation
                {
                    Latitude = dto.FromLocation.Latitude,
                    Longitude = dto.FromLocation.Longitude,
                    DisplayName = dto.FromLocation.DisplayName,
                    FullAddress = dto.FromLocation.FullAddress,
                    AdditionalNotes = dto.FromLocation.AdditionalNotes
                };

                var toLocation = new TripLocation
                {
                    Latitude = dto.ToLocation.Latitude,
                    Longitude = dto.ToLocation.Longitude,
                    DisplayName = dto.ToLocation.DisplayName,
                    FullAddress = dto.ToLocation.FullAddress,
                    AdditionalNotes = dto.ToLocation.AdditionalNotes
                };

                await _unitOfWork.TripLocations.CreateOrUpdateAsync(fromLocation);
                await _unitOfWork.TripLocations.CreateOrUpdateAsync(toLocation);
                await _unitOfWork.Save();

                // Create trip
                var trip = new Trip
                {
                    FromLocationId = fromLocation.Id,
                    ToLocationId = toLocation.Id,
                    DepartureTime = dto.DepartureTime,
                    AvailableSeats = dto.AvailableSeats,
                    PricePerSeat = dto.PricePerSeat,
                    DriverNotes = dto.DriverNotes,
                    HasWiFi = dto.HasWiFi,
                    HasPhoneCharger = dto.HasPhoneCharger,
                    HasAirConditioning = dto.HasAirConditioning,
                    HasFreeWater = dto.HasFreeWater,
                    HasMusic = dto.HasMusic,
                    DriverId = driverId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Trips.CreateOrUpdateAsync(trip);
                await _unitOfWork.Save();

                // Manual mapping for response
                var result = new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = fromLocation.Latitude,
                        Longitude = fromLocation.Longitude,
                        DisplayName = fromLocation.DisplayName,
                        FullAddress = fromLocation.FullAddress,
                        AdditionalNotes = fromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = toLocation.Latitude,
                        Longitude = toLocation.Longitude,
                        DisplayName = toLocation.DisplayName,
                        FullAddress = toLocation.FullAddress,
                        AdditionalNotes = toLocation.AdditionalNotes
                    },
                    DriverName = driver.UserName,
                    DriverPhone = driver.PhoneNumber,
                    StartingPoint = fromLocation.DisplayName,
                    Destination = toLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                };

                return ApiResponse<TripResponseDto>.SuccessResponse("Trip created successfully", 201, result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TripResponseDto>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        //public async Task<ApiResponse<TripResponseDto>> UpdateTripAsync(TripUpdateDto dto, string driverId)
        //{
        //    try
        //    {
        //        var trip = await _unitOfWork.Trips.GetFirstOrDefaultAsync(
        //            t => t.TripId == dto.Id,
        //            includeProperties: new Expression<Func<Trip, object>>[] {
        //                t => t.FromLocation,
        //                t => t.ToLocation,
        //                t => t.Driver
        //            });

        //        if (trip == null)
        //            return ApiResponse<TripResponseDto>.ErrorResponse("Trip not found", 404);

        //        if (trip.DriverId != driverId)
        //            return ApiResponse<TripResponseDto>.ErrorResponse("Unauthorized to update this trip", 403);

        //        // Update locations if changed
        //        if (dto.FromLocation != null)
        //        {
        //            trip.FromLocation.Latitude = dto.FromLocation.Latitude;
        //            trip.FromLocation.Longitude = dto.FromLocation.Longitude;
        //            trip.FromLocation.DisplayName = dto.FromLocation.DisplayName;
        //            trip.FromLocation.FullAddress = dto.FromLocation.FullAddress;
        //            trip.FromLocation.AdditionalNotes = dto.FromLocation.AdditionalNotes;
        //            await _unitOfWork.TripLocations.CreateOrUpdateAsync(trip.FromLocation);
        //        }

        //        if (dto.ToLocation != null)
        //        {
        //            trip.ToLocation.Latitude = dto.ToLocation.Latitude;
        //            trip.ToLocation.Longitude = dto.ToLocation.Longitude;
        //            trip.ToLocation.DisplayName = dto.ToLocation.DisplayName;
        //            trip.ToLocation.FullAddress = dto.ToLocation.FullAddress;
        //            trip.ToLocation.AdditionalNotes = dto.ToLocation.AdditionalNotes;
        //            await _unitOfWork.TripLocations.CreateOrUpdateAsync(trip.ToLocation);
        //        }

        //        // Update trip properties
        //        trip.DepartureTime = dto.DepartureTime;
        //        trip.AvailableSeats = dto.AvailableSeats;
        //        trip.PricePerSeat = dto.PricePerSeat;
        //        trip.DriverNotes = dto.DriverNotes;
        //        trip.HasWiFi = dto.HasWiFi;
        //        trip.HasPhoneCharger = dto.HasPhoneCharger;
        //        trip.HasAirConditioning = dto.HasAirConditioning;
        //        trip.HasFreeWater = dto.HasFreeWater;
        //        trip.HasMusic = dto.HasMusic;

        //        await _unitOfWork.Trips.CreateOrUpdateAsync(trip);
        //        await _unitOfWork.Save();

        //        // Manual mapping for response
        //        var result = new TripResponseDto
        //        {
        //            Id = trip.TripId,
        //            FromLocation = new TripLocationDto
        //            {
        //                Latitude = trip.FromLocation.Latitude,
        //                Longitude = trip.FromLocation.Longitude,
        //                DisplayName = trip.FromLocation.DisplayName,
        //                FullAddress = trip.FromLocation.FullAddress,
        //                AdditionalNotes = trip.FromLocation.AdditionalNotes
        //            },
        //            ToLocation = new TripLocationDto
        //            {
        //                Latitude = trip.ToLocation.Latitude,
        //                Longitude = trip.ToLocation.Longitude,
        //                DisplayName = trip.ToLocation.DisplayName,
        //                FullAddress = trip.ToLocation.FullAddress,
        //                AdditionalNotes = trip.ToLocation.AdditionalNotes
        //            },
        //            DriverName = $"{trip.Driver?.FirstName} {trip.Driver?.LastName}",
        //            DriverPhone = trip.Driver?.PhoneNumber,
        //            StartingPoint = trip.FromLocation.DisplayName,
        //            Destination = trip.ToLocation.DisplayName,
        //            DepartureTime = trip.DepartureTime,
        //            AvailableSeats = trip.AvailableSeats,
        //            PricePerSeat = trip.PricePerSeat,
        //            Notes = trip.DriverNotes,
        //            CreatedAt = trip.CreatedAt
        //        };

        //        return ApiResponse<TripResponseDto>.SuccessResponse("Trip updated successfully", data: result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse<TripResponseDto>.ErrorResponse($"An error occurred: {ex.Message}", 500);
        //    }
        //}

        public async Task<ApiResponse> DeleteTripAsync(int tripId, string driverId)
        {
            try
            {
                var trip = await _unitOfWork.Trips.GetFirstOrDefaultAsync(
                    t => t.TripId == tripId,
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation
                    });

                if (trip == null)
                    return ApiResponse.ErrorResponse("Trip not found", 404);

                if (trip.DriverId != driverId)
                    return ApiResponse.ErrorResponse("Unauthorized to delete this trip", 403);

                await _unitOfWork.Trips.DeleteAsync(trip);
                await _unitOfWork.Save();

                return ApiResponse.SuccessResponse("Trip deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<TripResponseDto>> GetTripByIdAsync(int tripId)
        {
            try
            {
                var trip = await _unitOfWork.Trips.GetFirstOrDefaultAsync(
                    t => t.TripId == tripId,
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver,
                        t => t.Bookings
                    });

                if (trip == null)
                    return ApiResponse<TripResponseDto>.ErrorResponse("Trip not found", 404);

                // Manual mapping
                var result = new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = trip.FromLocation.Latitude,
                        Longitude = trip.FromLocation.Longitude,
                        DisplayName = trip.FromLocation.DisplayName,
                        FullAddress = trip.FromLocation.FullAddress,
                        AdditionalNotes = trip.FromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = trip.ToLocation.Latitude,
                        Longitude = trip.ToLocation.Longitude,
                        DisplayName = trip.ToLocation.DisplayName,
                        FullAddress = trip.ToLocation.FullAddress,
                        AdditionalNotes = trip.ToLocation.AdditionalNotes
                    },
                    DriverName = trip.Driver?.UserName,
                    DriverPhone = trip.Driver?.PhoneNumber,
                    StartingPoint = trip.FromLocation.DisplayName,
                    Destination = trip.ToLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                };

                return ApiResponse<TripResponseDto>.SuccessResponse("Trip retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TripResponseDto>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> GetAllTripsAsync()
        {
            try
            {
                var trips = await _unitOfWork.Trips.GetAsync(
                    filter: t => t.DepartureTime > DateTime.UtcNow,
                    orderBy: q => q.OrderBy(t => t.DepartureTime),
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    });

                // Manual mapping
                var result = trips.Select(trip => new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = trip.FromLocation.Latitude,
                        Longitude = trip.FromLocation.Longitude,
                        DisplayName = trip.FromLocation.DisplayName,
                        FullAddress = trip.FromLocation.FullAddress,
                        AdditionalNotes = trip.FromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = trip.ToLocation.Latitude,
                        Longitude = trip.ToLocation.Longitude,
                        DisplayName = trip.ToLocation.DisplayName,
                        FullAddress = trip.ToLocation.FullAddress,
                        AdditionalNotes = trip.ToLocation.AdditionalNotes
                    },
                    DriverName = trip.Driver?.UserName,
                    DriverPhone = trip.Driver?.PhoneNumber,
                    StartingPoint = trip.FromLocation.DisplayName,
                    Destination = trip.ToLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> GetDriverTripsAsync(string driverId)
        {
            try
            {
                var trips = await _unitOfWork.Trips.GetAsync(
                    t => t.DriverId == driverId,
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    });

                // Manual mapping
                var result = trips.Select(trip => new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = trip.FromLocation.Latitude,
                        Longitude = trip.FromLocation.Longitude,
                        DisplayName = trip.FromLocation.DisplayName,
                        FullAddress = trip.FromLocation.FullAddress,
                        AdditionalNotes = trip.FromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = trip.ToLocation.Latitude,
                        Longitude = trip.ToLocation.Longitude,
                        DisplayName = trip.ToLocation.DisplayName,
                        FullAddress = trip.ToLocation.FullAddress,
                        AdditionalNotes = trip.ToLocation.AdditionalNotes
                    },
                    DriverName = trip.Driver?.UserName,
                    DriverPhone = trip.Driver?.PhoneNumber,
                    StartingPoint = trip.FromLocation.DisplayName,
                    Destination = trip.ToLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Driver trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> SearchTripsAsync(string fromCity, string toCity, DateTime? date)
        {
            try
            {
                Expression<Func<Trip, bool>> filter = t =>
                    t.FromLocation.DisplayName.Contains(fromCity) &&
                    t.ToLocation.DisplayName.Contains(toCity) &&
                    t.DepartureTime > DateTime.UtcNow;

                if (date.HasValue)
                {
                    filter = t => t.FromLocation.DisplayName.Contains(fromCity) &&
                                t.ToLocation.DisplayName.Contains(toCity) &&
                                t.DepartureTime.Date == date.Value.Date;
                }

                var trips = await _unitOfWork.Trips.GetAsync(
                    filter,
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    });

                // Manual mapping
                var result = trips.Select(trip => new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = trip.FromLocation.Latitude,
                        Longitude = trip.FromLocation.Longitude,
                        DisplayName = trip.FromLocation.DisplayName,
                        FullAddress = trip.FromLocation.FullAddress,
                        AdditionalNotes = trip.FromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = trip.ToLocation.Latitude,
                        Longitude = trip.ToLocation.Longitude,
                        DisplayName = trip.ToLocation.DisplayName,
                        FullAddress = trip.ToLocation.FullAddress,
                        AdditionalNotes = trip.ToLocation.AdditionalNotes
                    },
                    DriverName = trip.Driver?.UserName,
                    DriverPhone = trip.Driver?.PhoneNumber,
                    StartingPoint = trip.FromLocation.DisplayName,
                    Destination = trip.ToLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> GetTripsByAmenitiesAsync(TripAmenitiesDto amenities)
        {
            try
            {
                Expression<Func<Trip, bool>> filter = t =>
                    t.DepartureTime > DateTime.UtcNow;

                // Apply amenity filters if specified
                if (amenities.HasWiFi.HasValue)
                    filter = filter.And(t => t.HasWiFi == amenities.HasWiFi);
                if (amenities.HasPhoneCharger.HasValue)
                    filter = filter.And(t => t.HasPhoneCharger == amenities.HasPhoneCharger);
                if (amenities.HasAirConditioning.HasValue)
                    filter = filter.And(t => t.HasAirConditioning == amenities.HasAirConditioning);
                if (amenities.HasFreeWater.HasValue)
                    filter = filter.And(t => t.HasFreeWater == amenities.HasFreeWater);
                if (amenities.HasMusic.HasValue)
                    filter = filter.And(t => t.HasMusic == amenities.HasMusic);

                var trips = await _unitOfWork.Trips.GetAsync(
                    filter,
                    includeProperties: new Expression<Func<Trip, object>>[] {
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    });

                // Manual mapping
                var result = trips.Select(trip => new TripResponseDto
                {
                    Id = trip.TripId,
                    FromLocation = new TripLocationDto
                    {
                        Latitude = trip.FromLocation.Latitude,
                        Longitude = trip.FromLocation.Longitude,
                        DisplayName = trip.FromLocation.DisplayName,
                        FullAddress = trip.FromLocation.FullAddress,
                        AdditionalNotes = trip.FromLocation.AdditionalNotes
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = trip.ToLocation.Latitude,
                        Longitude = trip.ToLocation.Longitude,
                        DisplayName = trip.ToLocation.DisplayName,
                        FullAddress = trip.ToLocation.FullAddress,
                        AdditionalNotes = trip.ToLocation.AdditionalNotes
                    },
                    DriverName = trip.Driver?.UserName,
                    DriverPhone = trip.Driver?.PhoneNumber,
                    StartingPoint = trip.FromLocation.DisplayName,
                    Destination = trip.ToLocation.DisplayName,
                    DepartureTime = trip.DepartureTime,
                    AvailableSeats = trip.AvailableSeats,
                    PricePerSeat = trip.PricePerSeat,
                    Notes = trip.DriverNotes,
                    CreatedAt = trip.CreatedAt
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}