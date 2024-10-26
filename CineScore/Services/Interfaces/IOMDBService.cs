using CineScore.Models;

namespace CineScore.Services;

public interface IOmdbService
{
    Task<Movie> GetMovieById(string id);
    Task<Movie> GetMovieByTitle(string title);
}