using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Repositories;
using TicketBookingSystem.Infrastructure.Services;

namespace TicketBookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController : ControllerBase
{
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IBookingService _bookingService;

    public ShowtimesController(IShowtimeRepository showtimeRepository, IBookingService bookingService)
    {
        _showtimeRepository = showtimeRepository;
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var showtimes = await _showtimeRepository.GetAllAsync();
        return Ok(showtimes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var showtime = await _showtimeRepository.GetByIdAsync(id);
        if (showtime == null)
        {
            return NotFound();
        }
        return Ok(showtime);
    }

    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetByMovieId(string movieId)
    {
        var showtimes = await _showtimeRepository.GetByMovieIdAsync(movieId);
        return Ok(showtimes);
    }

    [HttpGet("{id}/available-seats")]
    public async Task<IActionResult> GetAvailableSeats(string id)
    {
        var seats = await _bookingService.GetAvailableSeatsAsync(id);
        return Ok(new { showtimeId = id, availableSeats = seats });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Showtime showtime)
    {
        var created = await _showtimeRepository.CreateAsync(showtime);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
