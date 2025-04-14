using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.HousingRepository
{
    public interface IPropertyRepository
    {
        // Retrieve all properties that are not soft-deleted.
        Task<IEnumerable<Property>> GetAllAsync();

        // Retrieve a single property by its id (if not soft-deleted).
        Task<Property> GetByIdAsync(int propertyId);

        // Add a new property.

        Task AddAsync(Property property);

        // Update an existing property.

        Task UpdateAsync(Property property);


        // Soft delete a property by setting its IsDeleted flag to true.

        Task DeleteAsync(int propertyId);

        Task<List<Feature>> GetAllFeaturesAsync();

    }
}
