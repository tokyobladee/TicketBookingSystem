using TicketBookingSystem.Domain.Enums;

namespace TicketBookingSystem.Api.DTOs;

public record CreateBookingRequest(
    string ShowtimeId,
    List<string> SeatNumbers
);

public record BookingResponse(
    string Id,
    string ShowtimeId,
    string UserId,
    List<string> SeatNumbers,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime BookedAt
);
