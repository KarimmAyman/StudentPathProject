using AutoMapper;
using StudentPath.BLL.Dtoes.HousingDtoes;
using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.AutoMappers.HousingMapper
{
    public class PropertyMappingProfile :Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.PropertyImages.Select(i => i.ImageUrl).ToList()))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.PropertyFeatures.Select(pf => pf.Feature.Name).ToList()))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationDto
                {
                    City = src.Locations.FirstOrDefault().City,
                    Country = src.Locations.FirstOrDefault().Country,
                    Street = src.Locations.FirstOrDefault().Street,
                    Latitude = src.Locations.FirstOrDefault().Latitude,
                    Longitude = src.Locations.FirstOrDefault().Longitude
                }));

            CreateMap<PropertyCreateDto, Property>()
                .ForMember(dest => dest.PropertyImages, opt => opt.MapFrom(src => src.ImageUrls.Select(url => new PropertyImage { ImageUrl = url })))
                .ForMember(dest => dest.PropertyFeatures, opt => opt.MapFrom(src => src.FeatureIds.Select(id => new PropertyFeature { FeatureId = id })))
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => new List<LocationProperty>
                {
                new LocationProperty
                {
                    City = src.Location.City,
                    Country = src.Location.Country,
                    Street = src.Location.Street,
                    Latitude = src.Location.Latitude,
                    Longitude = src.Location.Longitude
                }
                }));

            CreateMap<PropertyUpdateDto, Property>()
                .ForAllMembers(opt => opt.Condition((src, dest, value) => value != null));
        }
    }
}
