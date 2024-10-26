using CineScore.Models;

namespace CineScore.Services;

public interface IOmdbService
{
    Task<Movie> GetMovie(string queryValue, bool isById = true);
}