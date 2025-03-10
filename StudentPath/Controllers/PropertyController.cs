using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.HousingDtoes;
using StudentPath.BLL.Services.HousingServices;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: api/properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        // GET: api/properties/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> Get(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        // POST: api/properties
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PropertyDto>> Create(PropertyCreateDto createDto)
        {
            // Extract the authenticated user's ID from the claims.
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            createDto.UserId = userId;

            var createdProperty = await _propertyService.CreatePropertyAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = createdProperty.PropertyId }, createdProperty);
        }

        // PUT: api/properties/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyDto>> Update(int id, PropertyUpdateDto updateDto)
        {
            if (id != updateDto.PropertyId)
            {
                return BadRequest("Mismatched property ID");
            }

            var updatedProperty = await _propertyService.UpdatePropertyAsync(updateDto);
            return Ok(updatedProperty);
        }

        // DELETE: api/properties/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }
    }
}
