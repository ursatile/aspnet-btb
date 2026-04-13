# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Rockaway.sln

# Run the web app
dotnet run --project Rockaway.WebApp

# Run all tests
dotnet test Rockaway.sln

# Run a single test
dotnet test Rockaway.WebApp.Tests --filter "FullyQualifiedName~TestName"

# EF Core migrations (run from Rockaway.WebApp directory)
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Architecture

This is an ASP.NET Core 10 web app for "Rockaway" — a concert ticketing platform. It uses a hybrid of Razor Pages (public-facing) and MVC Controllers (ticket purchase flow and admin area).

### Project structure

- **`Rockaway.WebApp`** — main web application
- **`Rockaway.WebApp.Tests`** — xUnit test project referencing the web app

### Data layer

`RockawayDbContext` extends `IdentityDbContext<IdentityUser>` (ASP.NET Core Identity is embedded). The database provider is always SQLite in-memory (both Development and UnitTest environments); SQL Server support is included as a package reference but not wired up in `Program.cs`.

`HostEnvironmentExtensions.UseSqlite()` controls environment-based provider selection — check there if adding SQL Server support for production.

**Entity model:**
- `Artist` — has a unique `Slug`, relates to `Show` as headline act
- `Venue` — has a unique `Slug`, hosts `Show`s
- `Show` — composite PK of `(Venue.Id, Date)` using `LocalDate` (NodaTime); has `TicketType`s, `SupportSlot`s, and `TicketOrders`
- `SupportSlot` — composite PK of `(Show.Venue.Id, Show.Date, SlotNumber)`
- `TicketType` — belongs to a `Show`, has `Price` (money column) and `Name`
- `TicketOrder` / `TicketOrderItem` — `TicketOrderItem` has composite PK of `(TicketOrder.Id, TicketType.Id)`

EF table names use the class name directly (not pluralized) — enforced in `OnModelCreating`.

### NodaTime

`LocalDate` and `Instant` are used throughout instead of `DateTime`/`DateTimeOffset`. Converters are registered in `NodaTimeConverters.AddNodaTimeConverters`. `IClock` is injected (bound to `SystemClock.Instance`) for testability.

### Sample / seed data

`SampleData` (partial class, split across `SampleData.Artists.cs`, `.Venues.cs`, `.Shows.cs`, etc.) provides static test data. `SeedData` converts these entities to anonymous objects for EF's `HasData()` seeding — required because EF seed data cannot have navigation properties. The database is seeded on startup via `db.Database.EnsureCreated()`.

### Routing

- Razor Pages under `Pages/` handle public content (artists list, venues list, home, contact)
- `TicketsController` uses `[Route("[action]/{venue}/{date}")]` — the show purchase flow
- `CheckoutController` handles order confirmation (GET shows form, POST completes order)
- Admin area is under `Areas/Admin/` with MVC controllers, protected by `.RequireAuthorization()`

URLs are lowercased globally via `RouteOptions.LowercaseUrls = true`.

### Models vs Entities

View models in `Models/` (e.g., `ShowViewData`, `TicketOrderViewData`) wrap entities for display. `SeedData` uses the same pattern of projecting to anonymous objects to strip navigation properties.

## Code Style

This project is to demonstrate patterns and features of .NET 10. Always prefer a .NET 10 pattern 
or idiom where it would be a pragmatic solution to the problem: 

* Field-backed properties and the `field` keyword
* Extension members
* Null-conditional assignmments
* Partial constructors