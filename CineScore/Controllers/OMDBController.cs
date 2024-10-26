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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(string id)
        {
            try
            {
                Log.Information($"Get by id: {id} [{Environment.MachineName}] [{Environment.UserName}]");
                var result = await _service.GetMovieById(id);
                if (!string.IsNullOrEmpty(result.ImdbId))
                    return new OkObjectResult(result);
  
                return new NotFoundObjectResult("Not found");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message );
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
