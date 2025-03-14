using AutoMapper;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models;

namespace StudentPath.BLL.AutoMappers.DriverMapper
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverReadDTO>();
            CreateMap<Driver, DriverDetailsDTO>();
            CreateMap<DriverAddDTO, Driver>();
            CreateMap<DriverUpdateDTO, Driver>();
            CreateMap<VehicleInfoDto, VehicleInfo>();
            CreateMap<LocationDto, Location>();
            CreateMap<VehicleInfo, VehicleInfoDto>();
            CreateMap<Location, LocationDto>();
        }
    }
}