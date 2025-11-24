using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketBookingSystem.Domain.Entities;

public class Hall
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("rows")]
    public int Rows { get; set; }

    [BsonElement("seatsPerRow")]
    public int SeatsPerRow { get; set; }

    [BsonElement("totalSeats")]
    public int TotalSeats => Rows * SeatsPerRow;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
