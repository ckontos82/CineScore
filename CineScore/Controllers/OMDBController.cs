using CineScore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CineScore.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class OmdbController(IOmdbService service) : ControllerBase
    {
        private readonly IOmdbService _service = service;

        [HttpGet("movie")]
        public async Task<IActionResult> GetMovie([FromQuery]string? id, [FromQuery] string? title)
        {
            try
            {
                var result = (id, title) switch
                {
                    (not null, null) => await _service.GetMovie(id, true),
                    (null, not null) => await _service.GetMovie(title, false),
                    _ => null
                };

                return result == null ? new BadRequestObjectResult("Please provide either 'id' or 'title' query parameter.")
                    : string.IsNullOrEmpty(result.ImdbId) ? new NotFoundObjectResult("Movie not found") : new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new BadRequestObjectResult("An error occurred.");
            }
        }
    }
}
