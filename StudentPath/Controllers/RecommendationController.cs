using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Recommendations;
using StudentPath.BLL.Services.RecommendationServices;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService recommendationService;

        public RecommendationController(RecommendationService recommendationService)
        {
            this.recommendationService = recommendationService;
        }
        [HttpPost("get")]
        public async Task<IActionResult> GetRecommendations([FromBody] RecommendationRequestDTO request)
        {
            try
            {
                var result = await recommendationService.GetRecommendationsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
