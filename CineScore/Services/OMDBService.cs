using CineScore.Configuration;
using CineScore.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace CineScore.Services
{
    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;

        public OmdbService(HttpClient httpClient, IOptions<OmdbConf> options)
        {
            _httpClient = httpClient;
            var config = options.Value;
            _uri = $"{config.BaseUri}?apikey={config.OmdbKey}";
        }

        public async Task<Movie> GetMovieById(string id)
        {
            Log.Information($"Get by id: {id}");
            var requestUri = $"{_uri}&i={id}";
            var response = await _httpClient.GetAsync(requestUri);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Invalid OMDB API key");

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<Movie>(content);

            return movie;
        }

        public async Task<Movie> GetMovieByTitle(string title)
        {
            Log.Information($"Get by title: {title}");
            var requestUri = $"{_uri}&t={title}";
            var response = await _httpClient.GetAsync(requestUri);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Invalid OMDB API key");

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<Movie>(content);

            return movie;
        }
    }
}
