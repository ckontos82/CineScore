using System.Text.Json.Serialization;

namespace CineScore.Models
{
    public class Movie
    {
        public Guid Id { get; set; }

        [JsonPropertyName("imdbID")]
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        [JsonPropertyName("imdbRating")]
        public string ImdbRating { get; set; }
    }
}
