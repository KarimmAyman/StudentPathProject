using StudentPath.BLL.Dtoes.HousingDtoes;
using StudentPath.DAL.Data.Models.Housing;
using StudentPath.DAL.Repositories.HousingRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.HousingServices
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return properties.Select(MapPropertyToPropertyDto);
        }

        public async Task<PropertyDto> GetPropertyByIdAsync(int propertyId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with id {propertyId} not found.");
            }
            return MapPropertyToPropertyDto(property);
        }

        public async Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto createDto)
        {
            // تحويل الـ DTO إلى الـ Entity
            var property = new Property
            {
                AdvertisingStatus = createDto.AdvertisingStatus,
                HasInsurance = createDto.HasInsurance,
                HousingType = createDto.HousingType,
                Rooms = createDto.Rooms,
                Bathrooms = createDto.Bathrooms,
                GrossArea = createDto.GrossArea,
                NetArea = createDto.NetArea,
                WarmingType = createDto.WarmingType,
                BuildingAge = createDto.BuildingAge,
                FloorLocation = createDto.FloorLocation,
                IsFurnished = createDto.IsFurnished,
                IsAvailableForLoan = createDto.IsAvailableForLoan,
                Dues = createDto.Dues,
                Front = createDto.Front,
                RentPrice = createDto.RentPrice,
                Description = createDto.Description,
                Price = createDto.Price,
                Currency = createDto.Currency,
                UserId = createDto.UserId
            };

            // تحويل المواقع لو وُجدت
            if (createDto.Locations != null && createDto.Locations.Any())
            {
                property.Locations = createDto.Locations.Select(l => new LocationProperty
                {
                    City = l.City,
                    Country = l.Country,
                    Street = l.Street,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList();
            }

            // تحويل الصور لو وُجدت
            if (createDto.Images != null && createDto.Images.Any())
            {
                property.PropertyImages = createDto.Images.Select(i => new PropertyImage
                {
                    ImageUrl = i.ImageUrl
                }).ToList();
            }

            // تحويل الخصائص باستخدام قائمة الـ Feature IDs
            if (createDto.FeatureIds != null && createDto.FeatureIds.Any())
            {
                property.PropertyFeatures = createDto.FeatureIds.Select(id => new PropertyFeature
                {
                    FeatureId = id
                }).ToList();
            }

            await _propertyRepository.AddAsync(property);
            return MapPropertyToPropertyDto(property);
        }

        public async Task<PropertyDto> UpdatePropertyAsync(PropertyUpdateDto updateDto)
        {
            // استرجاع الـ property الحالي
            var property = await _propertyRepository.GetByIdAsync(updateDto.PropertyId);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with id {updateDto.PropertyId} not found.");
            }

            // تحديث الحقول الأساسية
            property.AdvertisingStatus = updateDto.AdvertisingStatus;
            property.HasInsurance = updateDto.HasInsurance;
            property.HousingType = updateDto.HousingType;
            property.Rooms = updateDto.Rooms;
            property.Bathrooms = updateDto.Bathrooms;
            property.GrossArea = updateDto.GrossArea;
            property.NetArea = updateDto.NetArea;
            property.WarmingType = updateDto.WarmingType;
            property.BuildingAge = updateDto.BuildingAge;
            property.FloorLocation = updateDto.FloorLocation;
            property.IsFurnished = updateDto.IsFurnished;
            property.IsAvailableForLoan = updateDto.IsAvailableForLoan;
            property.Dues = updateDto.Dues;
            property.Front = updateDto.Front;
            property.RentPrice = updateDto.RentPrice;
            property.Description = updateDto.Description;
            property.Price = updateDto.Price;
            property.Currency = updateDto.Currency;
            property.UserId = updateDto.UserId;

            // تحديث المواقع (نقوم بمسح القديمة وإعادة رسمها)
            if (updateDto.Locations != null)
            {
                property.Locations.Clear();
                property.Locations = updateDto.Locations.Select(l => new LocationProperty
                {
                    City = l.City,
                    Country = l.Country,
                    Street = l.Street,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList();
            }

            // تحديث الصور
            if (updateDto.Images != null)
            {
                property.PropertyImages.Clear();
                property.PropertyImages = updateDto.Images.Select(i => new PropertyImage
                {
                    ImageUrl = i.ImageUrl
                }).ToList();
            }

            // تحديث الخصائص (Features)
            if (updateDto.Features != null)
            {
                property.PropertyFeatures.Clear();
                property.PropertyFeatures = updateDto.Features.Select(f => new PropertyFeature
                {
                    FeatureId = f.Id
                }).ToList();
            }

            await _propertyRepository.UpdateAsync(property);
            return MapPropertyToPropertyDto(property);
        }

        public async Task DeletePropertyAsync(int propertyId)
        {
            await _propertyRepository.DeleteAsync(propertyId);
        }

        // دالة تحويل من الـ Entity إلى DTO بحيث تُحول قيم الـ enum إلى أسماء (string)
        private PropertyDto MapPropertyToPropertyDto(Property property)
        {
            return new PropertyDto
            {
                PropertyId = property.PropertyId,
                AdvertisingStatus = property.AdvertisingStatus.ToString(),
                HasInsurance = property.HasInsurance,
                HousingType = property.HousingType.ToString(),
                Rooms = property.Rooms,
                Bathrooms = property.Bathrooms,
                GrossArea = property.GrossArea,
                NetArea = property.NetArea,
                WarmingType = property.WarmingType?.ToString(),
                BuildingAge = property.BuildingAge,
                FloorLocation = property.FloorLocation,
                IsFurnished = property.IsFurnished,
                IsAvailableForLoan = property.IsAvailableForLoan,
                Dues = property.Dues,
                Front = property.Front?.ToString(),
                RentPrice = property.RentPrice,
                Description = property.Description,
                Price = property.Price,
                Currency = property.Currency.ToString(),
                UserId = property.UserId,
                Locations = property.Locations.Select(l => new PropertyLocationDto
                {
                    Id = l.Id,
                    City = l.City,
                    Country = l.Country,
                    Street = l.Street,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList(),
                Images = property.PropertyImages.Select(i => new PropertyImageDto
                {
                    PropertyImageId = i.PropertyImageId,
                    ImageUrl = i.ImageUrl
                }).ToList(),
                Features = property.PropertyFeatures.Select(pf => new FeatureDto
                {
                    Id = pf.FeatureId,
                    Name = pf.Feature?.Name,
                    Category = pf.Feature != null ? pf.Feature.Category.ToString() : string.Empty
                }).ToList()
            };
        }
        public async Task<List<FeatureDto>> GetAllFeaturesAsync()
        {
            var features = await _propertyRepository.GetAllFeaturesAsync();
            return features.Select(f => new FeatureDto
            {
                Id = f.Id,
                Name = f.Name,
                Category = f.Category.ToString()
            }).ToList();
        }
    }
}
