using AutoMapper;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.AutoMappers.DriverMapper
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverReadDTO>()
                 .ForMember(dest => dest.VehicleInfo, opt => opt.MapFrom(src => src.VehicleInfo));
            CreateMap<Driver, DriverDetailsDTO>();
            CreateMap<DriverAddDTO, Driver>();
            CreateMap<DriverUpdateDTO, Driver>();
            CreateMap<DriverReadDTO, Driver>();
            CreateMap<VehicleUpdateDTO, VehicleInfo>();
            CreateMap<LocationDto, Location>();
            CreateMap<VehicleInfo, VehicleUpdateDTO>();
            CreateMap<Location, LocationDto>();
            CreateMap<VehicleInfo, VehicleReadDTO>();
            CreateMap<Location, LocationDto>();
        }
    }
}