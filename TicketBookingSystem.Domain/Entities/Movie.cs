using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketBookingSystem.Domain.Entities;

public class Movie
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    [BsonRequired]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("durationMinutes")]
    public int DurationMinutes { get; set; }

    [BsonElement("genre")]
    public string Genre { get; set; } = string.Empty;

    [BsonElement("posterUrl")]
    public string PosterUrl { get; set; } = string.Empty;

    [BsonElement("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
