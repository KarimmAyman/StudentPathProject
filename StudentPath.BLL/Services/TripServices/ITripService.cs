using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.DAL.Data.Models;
using System;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.TripServices
{
    public interface ITripService
    {
        Task<ApiResponse<TripResponseDto>> CreateTripAsync(TripCreateDto dto, string driverId);
        Task<ApiResponse<TripResponseDto>> UpdateTripStatusAsync(int tripId, TripStatus newStatus, string driverId);
        Task<ApiResponse> DeleteTripAsync(int tripId, string driverId);
        Task<ApiResponse<TripResponseDto>> GetTripByIdAsync(int tripId);
        Task<ApiResponse<IEnumerable<TripResponseDto>>> GetAllTripsAsync(bool includePast);
        Task<ApiResponse<IEnumerable<TripResponseDto>>> GetDriverTripsAsync(string driverId);
        Task<ApiResponse<IEnumerable<TripResponseDto>>> SearchTripsAsync(string fromAddress, string toAddress);
        Task<ApiResponse<TripWithBookingsResponseDto>> GetDriverTripDetailsAsync(string driverId);
        Task<ApiResponse<IEnumerable<TripResponseDto>>> GetTripsByStatusAsync(TripStatus status);
    }
}