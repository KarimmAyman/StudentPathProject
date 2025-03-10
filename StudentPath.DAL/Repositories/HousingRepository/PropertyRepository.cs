using Microsoft.EntityFrameworkCore;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.HousingRepository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly StudentPathContext _context;

        public PropertyRepository(StudentPathContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Property> GetByIdAsync(int propertyId)
        {
            return await _context.Properties
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId && !p.IsDeleted);
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
        }

        // Soft delete implementation: mark the property as deleted.
        public async Task DeleteAsync(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property != null && !property.IsDeleted)
            {
                property.IsDeleted = true;
                _context.Properties.Update(property);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Feature>> GetAllFeaturesAsync()
        {
            return await _context.Features.ToListAsync();
        }
    }
}
