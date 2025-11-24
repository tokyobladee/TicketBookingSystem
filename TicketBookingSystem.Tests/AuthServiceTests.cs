using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using TicketBookingSystem.Domain.Entities;
using TicketBookingSystem.Infrastructure.Repositories;
using TicketBookingSystem.Infrastructure.Services;

namespace TicketBookingSystem.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "YourSuperSecretKeyForJwtTokenGeneration123456789",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        });

        _authService = new AuthService(_userRepositoryMock.Object, jwtSettings);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldSucceed()
    {
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var (success, token, user) = await _authService.RegisterAsync(
            "test@example.com", "password123", "John", "Doe");

        success.Should().BeTrue();
        token.Should().NotBeEmpty();
        user.Should().NotBeNull();
        user!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldFail()
    {
        var existingUser = new User { Email = "test@example.com" };
        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        var (success, token, user) = await _authService.RegisterAsync(
            "test@example.com", "password123", "John", "Doe");

        success.Should().BeFalse();
        token.Should().BeEmpty();
        user.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldSucceed()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var existingUser = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            PasswordHash = passwordHash,
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        var (success, token, user) = await _authService.LoginAsync(
            "test@example.com", "password123");

        success.Should().BeTrue();
        token.Should().NotBeEmpty();
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldFail()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var existingUser = new User
        {
            Email = "test@example.com",
            PasswordHash = passwordHash
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        var (success, token, user) = await _authService.LoginAsync(
            "test@example.com", "wrongpassword");

        success.Should().BeFalse();
        token.Should().BeEmpty();
    }
}
