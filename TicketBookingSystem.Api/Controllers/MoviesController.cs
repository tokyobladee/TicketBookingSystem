using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Repositories;

namespace TicketBookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieRepository.GetAllAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [HttpGet("genre/{genre}")]
    public async Task<IActionResult> GetByGenre(string genre)
    {
        var movies = await _movieRepository.GetByGenreAsync(genre);
        return Ok(movies);
    }

    [HttpGet("title/{title}")]
    public async Task<IActionResult> GetByTitle(string title)
    {
        var movie = await _movieRepository.GetByTitleAsync(title);
        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Movie movie)
    {
        var created = await _movieRepository.CreateAsync(movie);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
