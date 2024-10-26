using CineScore.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CineScore.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OMDBController(OMDBService service) : ControllerBase
    {
        private readonly OMDBService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetMovieById([FromQuery]string? id, [FromQuery] string? title)
        {
            try
            {
                var result = (id, title) switch
                {
                    (not null, null) => await _service.GetMovieById(id),
                    (null, not null) => await _service.GetMovieByTitle(title),
                    _ => null
                };

                return result == null ? new BadRequestObjectResult("Please provide either 'id' or 'title' query parameter.")
                    : string.IsNullOrEmpty(result.ImdbId) ? new NotFoundObjectResult("Movie not found") : new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
