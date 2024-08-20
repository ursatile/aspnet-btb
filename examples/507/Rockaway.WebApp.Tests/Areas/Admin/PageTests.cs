namespace Rockaway.WebApp.Tests.Areas.Admin;

public class PageTests {

	[Fact]
	public async Task Admin_Has_Personalised_Nav() {
		var fakeUsername = $"{Guid.NewGuid()}@rockaway.dev";
		var browsingContext = BrowsingContext.New(Configuration.Default);
		await using var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.AddFakeAuthentication(fakeUsername));
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/admin");
		var dom = await browsingContext.OpenAsync(req => req.Content(html));
		var title = dom.QuerySelector("a#manage");
		title.ShouldNotBeNull();
		title.InnerHtml.ShouldBe($"Hello {fakeUsername}!");
	}
}
