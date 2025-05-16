using StudentPath.BLL.Dtoes.HousingDtoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.HousingServices
{
    public interface IPropertyService
    {
        // Retrieve all properties that are not soft-deleted.
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();

        // Retrieve a single property by its ID.
        Task<PropertyDto> GetPropertyByIdAsync(int propertyId);

        // Create a new property using the CreatePropertyDto.
        Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto createDto);

        // Update an existing property using the UpdatePropertyDto.
        Task<PropertyDto> UpdatePropertyAsync(PropertyUpdateDto updateDto);

        // Soft delete a property by its ID.
        Task DeletePropertyAsync(int propertyId);

        Task<List<FeatureDto>> GetAllFeaturesAsync();
        Task<IEnumerable<PropertyDto>> GetPropertiesByUserIdAsync(string userId);


    }
}
