using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rockaway.WebApp.Controllers;
using NodaTime.Testing;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests;

public class TicketTests {
	[Fact]
	public async Task Tickets_Page_Works() {
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		using var scope = factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetService<RockawayDbContext>();
		var show = db.Shows.Include(show => show.Venue).First();
		var venue = show.RouteData["venue"];
		var date = show.RouteData["date"];
		using var response = await client.GetAsync($"/show/{venue}/{date}");
		response.EnsureSuccessStatusCode();
	}
	[Fact]
	public async Task Buying_Tickets_Creates_Order() {

		var dbName = Guid.NewGuid().ToString();
		var db = TestDatabase.Create(dbName);
		var clock = new FakeClock(Instant.FromUtc(2024, 1, 2, 3, 4, 5));
		var c = new TicketsController(db, clock);
		var show = await db.Shows.Include(s => s.Venue).FirstOrDefaultAsync();
		show.ShouldNotBeNull();
		var orderCount = db.TicketOrders.Count();
		var tickets = show.TicketTypes.ToDictionary(tt => tt.Id, _ => 1);
		var howManyTicketsToExpect = tickets.Count;
		await c.Show(show.Venue.Slug, show.Date, tickets);

		var db2 = TestDatabase.Connect(dbName);
		db2.TicketOrders.Count().ShouldBe(orderCount + 1);
		db2.TicketOrders.First().Contents.Count.ShouldBe(howManyTicketsToExpect);
	}
}
