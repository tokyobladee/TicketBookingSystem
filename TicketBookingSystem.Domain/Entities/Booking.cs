using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TicketBookingSystem.Domain.Enums;

namespace TicketBookingSystem.Domain.Entities;

public class Booking
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("showtimeId")]
    [BsonRequired]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ShowtimeId { get; set; } = string.Empty;

    [BsonElement("seatNumbers")]
    [BsonRequired]
    public List<string> SeatNumbers { get; set; } = new();

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [BsonElement("bookedAt")]
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("version")]
    public long Version { get; set; } = 0;
}
