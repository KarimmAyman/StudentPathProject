using AutoMapper;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.AutoMappers.TripMapper
{
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<TripCreateDto, Trip>();
            CreateMap<TripLocationDto, TripLocation>();
            CreateMap<TripLocation, TripLocationDto>();
            CreateMap<Trip, TripResponseDto>();
        }
    }
}