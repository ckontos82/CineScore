using CineScore.Configuration;
using CineScore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;

namespace CineScore.Services
{
    public class OMDBService
    {
        private readonly HttpClient _httpClient;
        private readonly OMDBConf _config;
        private readonly string uri;

        public OMDBService(HttpClient httpClient, IOptions<OMDBConf> options)
        {
            _httpClient = httpClient;
            OMDBConf _config = options.Value;
            uri = $"{_config.BaseUri}?apikey={_config.OMDB_Key}";
        }

        public async Task<Movie> GetMovieById(string Id)
        {
            var requestUri = $"{uri}&i={Id}";

            var response = await _httpClient.GetAsync(requestUri);

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonSerializer.Deserialize<Movie>(content);

            return movie;
        }
    }
}
