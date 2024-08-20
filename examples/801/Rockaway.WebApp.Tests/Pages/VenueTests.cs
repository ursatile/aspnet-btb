using System.Net;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests.Pages;

public class VenueTests {
	[Fact]
	public async Task Venue_Page_Contains_All_Venues() {
		await using var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/venues");
		var decodedHtml = WebUtility.HtmlDecode(html);
		using var scope = factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
		var expected = db.Venues.ToList();
		foreach (var venue in expected) decodedHtml.ShouldContain(venue.Name);
	}
}