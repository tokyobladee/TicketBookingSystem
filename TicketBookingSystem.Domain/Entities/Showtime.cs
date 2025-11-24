using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketBookingSystem.Domain.Entities;

public class Showtime
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("movieId")]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MovieId { get; set; } = string.Empty;

    [BsonElement("hallId")]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    public string HallId { get; set; } = string.Empty;

    [BsonElement("startTime")]
    [BsonRequired]
    public DateTime StartTime { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("bookedSeats")]
    public List<string> BookedSeats { get; set; } = new();

    [BsonElement("version")]
    public long Version { get; set; } = 0;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
