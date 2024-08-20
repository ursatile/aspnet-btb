using System.Net;
using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests.Pages;

public class ArtistTests {
	[Fact]
	public async Task Artist_Page_Contains_All_Artists() {
		await using var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/artists");
		var decodedHtml = WebUtility.HtmlDecode(html);
		using var scope = factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
		var expected = db.Artists.ToList();
		foreach (var artist in expected) decodedHtml.ShouldContain(artist.Name);
	}
}