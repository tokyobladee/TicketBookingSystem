using FluentAssertions;
using Moq;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Domain.Enums;
using TicketBookingSystem.Infrastructure.Data;
using TicketBookingSystem.Infrastructure.Repositories;
using TicketBookingSystem.Infrastructure.Services;

namespace TicketBookingSystem.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IShowtimeRepository> _showtimeRepositoryMock;
    private readonly Mock<IHallRepository> _hallRepositoryMock;
    private readonly Mock<IMongoDbContext> _contextMock;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _showtimeRepositoryMock = new Mock<IShowtimeRepository>();
        _hallRepositoryMock = new Mock<IHallRepository>();
        _contextMock = new Mock<IMongoDbContext>();
    }

    [Fact]
    public async Task GetAvailableSeatsAsync_ShouldReturnUnbookedSeats()
    {
        var hall = new Hall
        {
            Id = "hall1",
            Rows = 5,
            SeatsPerRow = 10
        };

        var showtime = new Showtime
        {
            Id = "showtime1",
            HallId = "hall1",
            BookedSeats = new List<string> { "1-1", "1-2", "2-5" }
        };

        _showtimeRepositoryMock.Setup(x => x.GetByIdAsync("showtime1"))
            .ReturnsAsync(showtime);

        _hallRepositoryMock.Setup(x => x.GetByIdAsync("hall1"))
            .ReturnsAsync(hall);

        var service = new BookingService(
            _bookingRepositoryMock.Object,
            _showtimeRepositoryMock.Object,
            _hallRepositoryMock.Object,
            _contextMock.Object
        );

        var availableSeats = await service.GetAvailableSeatsAsync("showtime1");

        availableSeats.Should().HaveCount(47);
        availableSeats.Should().NotContain("1-1");
        availableSeats.Should().NotContain("1-2");
        availableSeats.Should().NotContain("2-5");
        availableSeats.Should().Contain("1-3");
    }

    [Fact]
    public async Task GetUserBookingsAsync_ShouldReturnUserBookings()
    {
        var userId = "user123";
        var bookings = new List<Booking>
        {
            new Booking { Id = "1", UserId = userId, Status = BookingStatus.Confirmed },
            new Booking { Id = "2", UserId = userId, Status = BookingStatus.Pending }
        };

        _bookingRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(bookings);

        var service = new BookingService(
            _bookingRepositoryMock.Object,
            _showtimeRepositoryMock.Object,
            _hallRepositoryMock.Object,
            _contextMock.Object
        );

        var result = await service.GetUserBookingsAsync(userId);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(b => b.UserId.Should().Be(userId));
    }
}
