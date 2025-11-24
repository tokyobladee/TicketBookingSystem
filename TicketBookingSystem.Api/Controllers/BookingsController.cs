using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketBookingSystem.Api.DTOs;
using TicketBookingSystem.Infrastructure.Services;

namespace TicketBookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var (success, booking, message) = await _bookingService.CreateBookingAsync(
            userId, request.ShowtimeId, request.SeatNumbers);

        if (!success || booking == null)
        {
            return BadRequest(new { message });
        }

        var response = new BookingResponse(
            booking.Id,
            booking.ShowtimeId,
            booking.UserId,
            booking.SeatNumbers,
            booking.TotalPrice,
            booking.Status,
            booking.BookedAt
        );

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var bookings = await _bookingService.GetUserBookingsAsync(userId!);
        var booking = bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        var response = new BookingResponse(
            booking.Id,
            booking.ShowtimeId,
            booking.UserId,
            booking.SeatNumbers,
            booking.TotalPrice,
            booking.Status,
            booking.BookedAt
        );

        return Ok(response);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        var response = bookings.Select(b => new BookingResponse(
            b.Id,
            b.ShowtimeId,
            b.UserId,
            b.SeatNumbers,
            b.TotalPrice,
            b.Status,
            b.BookedAt
        ));

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var (success, message) = await _bookingService.CancelBookingAsync(id, userId);

        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new { message });
    }
}
