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

        public async Task<Movie> GetMovie(string queryValue, bool isById = true)
        {
            Log.Information($"Get by {(isById ? "ID" : "title")}: {queryValue}");

            var queryParam = isById ? "i" : "t";
            var requestUri = $"{_uri}&{queryParam}={queryValue}";

            var response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Invalid OMDB API key");

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<Movie>(content);

            return movie;
        }
    }
}
