# Ticket Booking System

A cinema ticket booking system built with ASP.NET Core and MongoDB, featuring optimistic locking for concurrent booking management.

## Features

- User authentication with JWT
- Movie and hall management
- Showtime scheduling
- Seat booking with concurrency control
- MongoDB replica set with transactions
- RESTful API with Swagger documentation

## Tech Stack

- .NET 9.0
- ASP.NET Core Web API
- MongoDB 7.0
- Docker & Docker Compose
- MongoDB.Driver
- JWT Bearer Authentication
- xUnit for testing

## Architecture

```
TicketBookingSystem/
├── TicketBookingSystem.Domain/
│   ├── Entities/
│   └── Enums/
├── TicketBookingSystem.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   └── Services/
├── TicketBookingSystem.Api/
│   ├── Controllers/
│   └── DTOs/
└── TicketBookingSystem.Tests/
```

### Project Dependencies

- API depends on Infrastructure and Domain
- Infrastructure depends on Domain
- Tests depend on all layers

## Prerequisites

- .NET 9.0 SDK
- Docker Desktop

## Installation

Clone the repository:
```bash
git clone <repository-url>
cd TicketBookingSystem
```

Start MongoDB with Docker Compose:
```bash
docker compose up -d
```

Run the application:
```bash
cd TicketBookingSystem.Api
dotnet run
```

The API will be available at `http://localhost:5220`

## API Documentation

Access Swagger UI at `http://localhost:5220` when the application is running.

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

### Movies

- `GET /api/movies` - List all movies
- `GET /api/movies/{id}` - Get movie by ID
- `GET /api/movies/title/{title}` - Get movie by title
- `GET /api/movies/genre/{genre}` - Get movies by genre
- `POST /api/movies` - Create movie

### Halls

- `GET /api/halls` - List all halls
- `GET /api/halls/{id}` - Get hall by ID
- `POST /api/halls` - Create hall

### Showtimes

- `GET /api/showtimes` - List all showtimes
- `GET /api/showtimes/{id}` - Get showtime by ID
- `GET /api/showtimes/{id}/available-seats` - Get available seats
- `POST /api/showtimes` - Create showtime

### Bookings

- `GET /api/bookings/my` - Get user bookings
- `GET /api/bookings/{id}` - Get booking by ID
- `POST /api/bookings` - Create booking
- `DELETE /api/bookings/{id}` - Cancel booking

## Database

MongoDB runs in a Docker container on port 27017. Database name is `TicketBookingDb`.

### Collections

- `users` - User accounts
- `movies` - Movie catalog
- `halls` - Cinema halls
- `showtimes` - Movie showtimes
- `bookings` - Ticket bookings

### MongoDB UI

Access Mongo Express at `http://localhost:8081` for database management.

## Configuration

Configuration is in `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:admin123@localhost:27017/?authSource=admin&replicaSet=rs0",
    "DatabaseName": "TicketBookingDb"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJwtTokenGeneration123456789",
    "Issuer": "TicketBookingSystem",
    "Audience": "TicketBookingUsers",
    "ExpiryMinutes": 60
  }
}
```

**Note:** The credentials above are for local development only. For production, use environment variables or secure configuration management.

## Testing

Run unit tests:
```bash
dotnet test
```

Build the solution:
```bash
dotnet build
```

Clean build artifacts:
```bash
dotnet clean
```

## Concurrency Control

The system uses optimistic locking with version fields to handle concurrent booking requests. MongoDB transactions ensure data consistency across operations.

### How it works

1. Each showtime has a version field
2. Booking updates check the version
3. Failed updates trigger retry (up to 3 attempts)
4. Transactions guarantee atomicity

## Docker Commands

Start services:
```bash
docker compose up -d
```

Stop services:
```bash
docker compose down
```

View logs:
```bash
docker compose logs -f mongodb
```

Remove all data:
```bash
docker compose down -v
```

## License

MIT
