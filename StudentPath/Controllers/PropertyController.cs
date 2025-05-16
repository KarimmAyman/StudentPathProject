using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentPath.BLL.Dtoes.HousingDtoes;
using StudentPath.BLL.Services.HousingServices;
using StudentPath.DAL.Data.Models.Housing;
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
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(new { successed = true, data = properties });
        }

        // GET: api/properties/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound(new { successed = false, errors = new[] { "Property not found." } });
            }
            return Ok(new { successed = true, data = property });
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPropertiesByUser(string userId)
        {
            var properties = await _propertyService.GetPropertiesByUserIdAsync(userId);
            return Ok(properties);
        }


        // POST: api/properties
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] PropertyCreateDto createDto, [FromForm] string locationsJson)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { successed = false, errors = new[] { "User ID not found in token." } });
            }

            createDto.UserId = userId;

            // 🧠 Deserialize the locations from JSON string
            if (!string.IsNullOrWhiteSpace(locationsJson))
            {
                try
                {
                    createDto.Locations = JsonConvert.DeserializeObject<List<LocationCreateDto>>(locationsJson);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { successed = false, errors = new[] { "Invalid locations format", ex.Message } });
                }
            }

            var createdProperty = await _propertyService.CreatePropertyAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = createdProperty.PropertyId }, new { successed = true, data = createdProperty });
        }



        // PUT: api/properties/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, PropertyUpdateDto updateDto)
        {
            if (id != updateDto.PropertyId)
            {
                return BadRequest(new { successed = false, errors = new[] { "Mismatched property ID." } });
            }

            var updatedProperty = await _propertyService.UpdatePropertyAsync(updateDto);
            return Ok(new { successed = true, data = updatedProperty });
        }

        // DELETE: api/properties/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _propertyService.DeletePropertyAsync(id);
            return Ok(new { successed = true, message = "Property deleted successfully." });
        }
        // GET: api/properties/enums (Unique route to avoid conflicts)
        [HttpGet("enums")]
        public ActionResult<Dictionary<string, List<EnumValueDto>>> GetEnums()
        {
            var enums = EnumHelper.GetAllEnums();
            return Ok(enums);
        }

        [HttpGet("features")]
        public async Task<IActionResult> GetFeatures()
        {
            var features = await _propertyService.GetAllFeaturesAsync();
            return Ok(new { successed = true, data = features });
        }

    }
}
