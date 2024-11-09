using CineScore.Configuration;
using CineScore.Data; // Ensure correct namespace
using CineScore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using System.Text.Json;
using System.Threading.Tasks;

namespace CineScore.Services
{
    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;
        private readonly ApplicationDbContext _context;

        public OmdbService(HttpClient httpClient, IOptions<OmdbConf> options, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            var config = options.Value;
            _uri = $"{config.BaseUri}?apikey={config.OmdbKey}";
            _context = context;
        }

        public async Task<Movie> GetMovie(string queryValue, bool isById = true)
        {
            using (LogContext.PushProperty("Username", Environment.UserName))
            using (LogContext.PushProperty("Machine", Environment.MachineName))
            {
                Log.Information($"Get by {(isById ? "ID" : "title")}: {queryValue}");
            }

            var existingMovie = await FindExistingMovieAsync(queryValue, isById);

            Log.Information(existingMovie != null ? "Movie found in database." : "Movie not found in database.");

            return existingMovie ?? await FetchAndStoreMovieAsync(queryValue, isById);
        }

        private Task<Movie> FindExistingMovieAsync(string queryValue, bool isById)
        {
            return isById
                ? _context.Movies.FirstOrDefaultAsync(m => m.ImdbId == queryValue)
                : _context.Movies.FirstOrDefaultAsync(m => m.Title.ToLower() == queryValue.ToLower());
        }

        private async Task<Movie> FetchAndStoreMovieAsync(string queryValue, bool isById)
        {
            var queryParam = isById ? "i" : "t";
            var requestUri = $"{_uri}&{queryParam}={Uri.EscapeDataString(queryValue)}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Invalid OMDB API key");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch data from OMDB API. Status Code: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movie == null || string.IsNullOrEmpty(movie.ImdbId))
                return null;

            movie.Id = Guid.NewGuid();
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            Log.Information("Movie fetched from API and saved to database.");

            return movie;
        }
    }
}
