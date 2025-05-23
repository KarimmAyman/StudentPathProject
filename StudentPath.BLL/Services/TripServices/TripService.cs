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
using static StudentPath.BLL.Dtoes.Trips.TripResponseDto;

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
                // Check for existing active trip
                var activeTrip = await _unitOfWork.Trips.GetActiveTripByDriverIdAsync(driverId);
                if (activeTrip != null)
                {
                    // Calculate remaining seats
                    var confirmedBookings = await _unitOfWork.Bookings
                        .GetAsync(b => b.TripId == activeTrip.TripId && b.BookingStatus == BookingStatus.Confirmed);

                    var reservedSeats = confirmedBookings.Sum(b => b.NumberOfSeats);
                    var remainingSeats = activeTrip.AvailableSeats - reservedSeats;
                    if (remainingSeats < 0) remainingSeats = 0;

                    // Map to TripResponseDto
                    new AdditionalInfoDTO
                    {
                        StartingPoint = activeTrip.FromLocation.DisplayName,
                        Notes = activeTrip.DriverNotes,
                        HasWiFi = activeTrip.HasWiFi,
                        HasMusic = activeTrip.HasMusic,
                        HasPhoneCharger = activeTrip.HasPhoneCharger,
                        HasAirConditioning = activeTrip.HasAirConditioning,
                        HasFreeWater = activeTrip.HasFreeWater
                    }.PopulateAmenities();

                    var tripResponse = new TripResponseDto
                    {
                        Id = activeTrip.TripId,
                        FromLocation = new TripLocationDto
                        {
                            Latitude = activeTrip.FromLocation.Latitude,
                            Longitude = activeTrip.FromLocation.Longitude,
                            DisplayName = activeTrip.FromLocation.DisplayName,
                            FullAddress = activeTrip.FromLocation.FullAddress,
                            AdditionalNotes = activeTrip.FromLocation.AdditionalNotes
                        },
                        ToLocation = new TripLocationDto
                        {
                            Latitude = activeTrip.ToLocation.Latitude,
                            Longitude = activeTrip.ToLocation.Longitude,
                            DisplayName = activeTrip.ToLocation.DisplayName,
                            FullAddress = activeTrip.ToLocation.FullAddress,
                            AdditionalNotes = activeTrip.ToLocation.AdditionalNotes
                        },
                        BasicInfo = new BasicInfoDTO
                        {
                            DepartureTime = activeTrip.DepartureTime,
                            EstimatedDistance = activeTrip.EstimatedDistance,
                            EstimatedDuration = activeTrip.EstimatedDuration,
                            AvailableSeats = remainingSeats // Use real-time remaining seats
                        },
                        DriverInfo = new DriverInfoDto
                        {
                            DriverId = activeTrip.DriverId,
                            DriverName = activeTrip.Driver?.UserName,
                            DriverPhone = activeTrip.Driver?.PhoneNumber,
                            VehicleInfo = (activeTrip.Driver as Driver)?.VehicleInfo?
                                .Where(v => v.DriverId == activeTrip.DriverId)
                                .Select(v => new VehicleInfoDto
                                {
                                    VehicleModel = v.VehicleModel,
                                    SeatingCapacity = v.SeatingCapacity,
                                    PlateNumber = v.PlateNumber
                                })
                                .FirstOrDefault()
                        },
                        AdditionalInfo = new AdditionalInfoDTO
                        {
                            StartingPoint = activeTrip.FromLocation.DisplayName,
                            Notes = activeTrip.DriverNotes,
                        },

                        PricePerSeat = activeTrip.PricePerSeat,
                        CreatedAt = activeTrip.CreatedAt
                    };

                    return ApiResponse<TripResponseDto>.SuccessResponse(
                        "Driver already has an active trip",
                        200,
                        tripResponse);
                }
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
                    EstimatedDistance = dto.EstimatedDistance,
                    EstimatedDuration = dto.EstimatedDuration,
                    EstimatedArrivalTime = dto.EstimatedArrivalTime,
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



                var additionalInfo = new AdditionalInfoDTO
                {
                    StartingPoint = trip.FromLocation.DisplayName,
                    Notes = trip.DriverNotes,
                    HasWiFi = trip.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                    HasMusic = trip.HasMusic, // Assuming `HasMusic` exists in the trip model
                    HasPhoneCharger = trip.HasPhoneCharger,
                    HasAirConditioning = trip.HasAirConditioning,
                    HasFreeWater = trip.HasFreeWater

                };
                additionalInfo.PopulateAmenities();

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
                    BasicInfo = new BasicInfoDTO
                    {
                        DepartureTime = trip.DepartureTime,
                        EstimatedDistance = trip.EstimatedDistance,
                        EstimatedDuration = trip.EstimatedDuration,
                        AvailableSeats = trip.AvailableSeats
                    },
                    DriverInfo = new DriverInfoDto
                    {
                        DriverId = trip.DriverId,
                        DriverName = trip.Driver?.UserName,
                        DriverPhone = trip.Driver?.PhoneNumber,
                        VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                      .Where(v => v.DriverId == trip.DriverId).
                 Select(v => new VehicleInfoDto
                 {
                     VehicleModel = v.VehicleModel,
                     SeatingCapacity = v.SeatingCapacity,
                     PlateNumber = v.PlateNumber

                 })
                .FirstOrDefault()// Get the first vehicle (or use .ToList() to return all)
                    },
                    AdditionalInfo=additionalInfo,
                   
                  PricePerSeat = trip.PricePerSeat,

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
                    includeProperties: [
                        t => t.FromLocation,
                        t => t.ToLocation
                    ]);

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
                    includeProperties: [
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver,
                        t => t.Bookings
                    ]);

                if (trip == null)
                    return ApiResponse<TripResponseDto>.ErrorResponse("Trip not found", 404);

                var additionalInfo = new AdditionalInfoDTO
                {
                    StartingPoint = trip.FromLocation.DisplayName,
                    Notes = trip.DriverNotes,
                    HasWiFi = trip.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                    HasMusic = trip.HasMusic, // Assuming `HasMusic` exists in the trip model
                    HasPhoneCharger = trip.HasPhoneCharger,
                    HasAirConditioning = trip.HasAirConditioning,
                    HasFreeWater = trip.HasFreeWater

                };
                additionalInfo.PopulateAmenities();

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

                    BasicInfo = new BasicInfoDTO
                    {
                        DepartureTime= trip.DepartureTime,
                        EstimatedDistance = trip.EstimatedDistance,
                        EstimatedDuration = trip.EstimatedDuration,
                        AvailableSeats = trip.AvailableSeats
                    },
                    DriverInfo = new DriverInfoDto
                    {
                        DriverId = trip.DriverId,
                        DriverName = trip.Driver?.UserName,
                        DriverPhone = trip.Driver?.PhoneNumber,
                        VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                      .Where(v => v.DriverId == trip.DriverId).
                 Select(v => new VehicleInfoDto
                 {
                     VehicleModel = v.VehicleModel,
                     SeatingCapacity = v.SeatingCapacity,
                      PlateNumber = v.PlateNumber
                   
                 })
                .FirstOrDefault()// Get the first vehicle (or use .ToList() to return all)
                    },
                    AdditionalInfo = additionalInfo,



                    PricePerSeat = trip.PricePerSeat,
                   
                    CreatedAt = trip.CreatedAt
                };


                return ApiResponse<TripResponseDto>.SuccessResponse("Trip retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TripResponseDto>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> GetAllTripsAsync(bool includePast = false)
        {
            try
            {
                // Fixed filter logic
                Expression<Func<Trip, bool>>? filter = includePast
                    ? null
                    : t => t.DepartureTime > DateTime.UtcNow; // Changed to >

                var trips = await _unitOfWork.Trips.GetAsync(
                    filter: filter,
                    orderBy: q => q.OrderBy(t => t.DepartureTime), // Consider OrderByDescending
                    includeProperties: [
                        
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                        
                    ]);

                // Manual mapping
                var result = trips.Select(trip =>

                {
                    var additionalInfo = new AdditionalInfoDTO
                    {
                        StartingPoint = trip.FromLocation.DisplayName,
                        Notes = trip.DriverNotes,
                        HasWiFi = trip.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                        HasMusic = trip.HasMusic, // Assuming `HasMusic` exists in the trip model
                        HasPhoneCharger = trip.HasPhoneCharger,
                        HasAirConditioning = trip.HasAirConditioning,
                        HasFreeWater = trip.HasFreeWater

                    };
                    additionalInfo.PopulateAmenities();




                    return new TripResponseDto
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
                        BasicInfo = new BasicInfoDTO
                        {
                            DepartureTime = trip.DepartureTime,
                            EstimatedDistance = trip.EstimatedDistance,
                            EstimatedDuration = trip.EstimatedDuration,
                            AvailableSeats = trip.AvailableSeats
                        },
                        DriverInfo = new DriverInfoDto
                        {
                            DriverId = trip.DriverId,
                            DriverName = trip.Driver?.UserName,
                            DriverPhone = trip.Driver?.PhoneNumber,
                            VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                          .Where(v => v.DriverId == trip.DriverId).
                     Select(v => new VehicleInfoDto
                     {
                         VehicleModel = v.VehicleModel,
                         SeatingCapacity = v.SeatingCapacity,

                         PlateNumber = v.PlateNumber

                     })
                    .FirstOrDefault()// Get the first vehicle (or use .ToList() to return all)
                        },

                        AdditionalInfo = additionalInfo,
                        PricePerSeat = trip.PricePerSeat,

                        CreatedAt = trip.CreatedAt
                    };
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
                    includeProperties: [

                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    ]);

                // Manual mapping
                var result = trips.Select(trip =>
                { 
                    var additionalInfo = new AdditionalInfoDTO
                    {
                        StartingPoint = trip.FromLocation.DisplayName,
                        Notes = trip.DriverNotes,
                        HasWiFi = trip.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                        HasMusic = trip.HasMusic, // Assuming `HasMusic` exists in the trip model
                        HasPhoneCharger = trip.HasPhoneCharger,
                        HasAirConditioning = trip.HasAirConditioning,
                        HasFreeWater = trip.HasFreeWater

                    };
                    additionalInfo.PopulateAmenities();



                    return new TripResponseDto
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
                        BasicInfo = new BasicInfoDTO
                        {
                            DepartureTime = trip.DepartureTime,
                            EstimatedDistance = trip.EstimatedDistance,
                            EstimatedDuration = trip.EstimatedDuration,
                            AvailableSeats = trip.AvailableSeats
                        },
                        DriverInfo = new DriverInfoDto
                        {
                            DriverId = trip.DriverId,
                            DriverName = trip.Driver?.UserName,
                            DriverPhone = trip.Driver?.PhoneNumber,
                            VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                          .Where(v => v.DriverId == trip.DriverId).
                     Select(v => new VehicleInfoDto
                     {
                         VehicleModel = v.VehicleModel,
                         SeatingCapacity = v.SeatingCapacity,

                         PlateNumber = v.PlateNumber

                     })
                    .FirstOrDefault()// Get the first vehicle (or use .ToList() to return all)
                        },
                        AdditionalInfo = additionalInfo,
                        PricePerSeat = trip.PricePerSeat,
                        CreatedAt = trip.CreatedAt
                    };
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Driver trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<TripResponseDto>>> SearchTripsAsync(string fromAddress, string toAddress)
        {
            try
            {
                // Case-insensitive search on FullAddress containing the search terms
                Expression<Func<Trip, bool>> filter = t =>
                    t.FromLocation.FullAddress.ToLower().Contains(fromAddress.ToLower()) &&
                    t.ToLocation.FullAddress.ToLower().Contains(toAddress.ToLower())&&
                    t.DepartureTime > DateTime.UtcNow;

                var trips = await _unitOfWork.Trips.GetAsync(
                    filter,
                    includeProperties: [    
                        t => t.FromLocation,
                        t => t.ToLocation,
                        t => t.Driver
                    ]);

                // Manual mapping
                var result = trips.Select(trip =>

                {

                    var additionalInfo = new AdditionalInfoDTO
                    {
                        StartingPoint = trip.FromLocation.DisplayName,
                        Notes = trip.DriverNotes,
                        HasWiFi = trip.HasWiFi,  // Assuming `HasWiFi` exists in the trip model
                        HasMusic = trip.HasMusic, // Assuming `HasMusic` exists in the trip model
                        HasPhoneCharger = trip.HasPhoneCharger,
                        HasAirConditioning = trip.HasAirConditioning,
                        HasFreeWater = trip.HasFreeWater

                    };
                    additionalInfo.PopulateAmenities();





                    return new TripResponseDto
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
                        BasicInfo = new BasicInfoDTO
                        {
                            DepartureTime = trip.DepartureTime,
                            EstimatedDistance = trip.EstimatedDistance,
                            EstimatedDuration = trip.EstimatedDuration,
                            AvailableSeats = trip.AvailableSeats
                        },
                        DriverInfo = new DriverInfoDto
                        {
                            DriverId = trip.DriverId,
                            DriverName = trip.Driver?.UserName,
                            DriverPhone = trip.Driver?.PhoneNumber,
                            VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                          .Where(v => v.DriverId == trip.DriverId).
                     Select(v => new VehicleInfoDto
                     {
                         VehicleModel = v.VehicleModel,
                         SeatingCapacity = v.SeatingCapacity,
                         PlateNumber = v.PlateNumber

                     })
                    .FirstOrDefault()// Get the first vehicle (or use .ToList() to return all)
                        },
                        AdditionalInfo = additionalInfo,
                        PricePerSeat = trip.PricePerSeat,
                        CreatedAt = trip.CreatedAt
                    };
                });

                return ApiResponse<IEnumerable<TripResponseDto>>.SuccessResponse("Trips retrieved successfully", data: result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<TripResponseDto>>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<TripResponseDto>> GetDriverTripDetailsAsync(string driverId)
        {
            try
            {
                // Fetch the active trip with related data
                var trip = await _unitOfWork.Trips.GetActiveTripByDriverIdAsync(driverId);
                if (trip == null)
                {
                    return ApiResponse<TripResponseDto>.ErrorResponse("No active trip found", 404);
                }

                // Calculate remaining seats
                var confirmedBookings = await _unitOfWork.Bookings
                    .GetAsync(b => b.TripId == trip.TripId && b.BookingStatus == BookingStatus.Confirmed);

                var reservedSeats = confirmedBookings.Sum(b => b.NumberOfSeats);
                var remainingSeats = trip.AvailableSeats - reservedSeats;
                if (remainingSeats < 0) remainingSeats = 0;

                // Map to TripResponseDto
                var additionalInfo = new AdditionalInfoDTO
                {
                    StartingPoint = trip.FromLocation.DisplayName,
                    Notes = trip.DriverNotes,
                    HasWiFi = trip.HasWiFi,
                    HasMusic = trip.HasMusic,
                    HasPhoneCharger = trip.HasPhoneCharger,
                    HasAirConditioning = trip.HasAirConditioning,
                    HasFreeWater = trip.HasFreeWater
                };
                additionalInfo.PopulateAmenities();

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
                    BasicInfo = new BasicInfoDTO
                    {
                        DepartureTime = trip.DepartureTime,
                        EstimatedDistance = trip.EstimatedDistance,
                        EstimatedDuration = trip.EstimatedDuration,
                        AvailableSeats = remainingSeats // Use real-time remaining seats
                    },
                    DriverInfo = new DriverInfoDto
                    {
                        DriverId = trip.DriverId,
                        DriverName = trip.Driver?.UserName,
                        DriverPhone = trip.Driver?.PhoneNumber,
                        VehicleInfo = (trip.Driver as Driver)?.VehicleInfo?
                            .Where(v => v.DriverId == trip.DriverId)
                            .Select(v => new VehicleInfoDto
                            {
                                VehicleModel = v.VehicleModel,
                                SeatingCapacity = v.SeatingCapacity,
                                PlateNumber = v.PlateNumber
                            })
                            .FirstOrDefault()
                    },
                    AdditionalInfo = additionalInfo,
                    PricePerSeat = trip.PricePerSeat,
                    CreatedAt = trip.CreatedAt
                };

                return ApiResponse<TripResponseDto>.SuccessResponse("Trip details retrieved successfully", 200, result);
            }
            catch (Exception ex)
            {
                return ApiResponse<TripResponseDto>.ErrorResponse($"An error occurred: {ex.Message}", 500);
            }
        }
    }
}