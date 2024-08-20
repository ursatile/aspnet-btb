using System.Net;

namespace Rockaway.WebApp.Tests.Areas.Admin;

public class SecurityTests {
	[Fact]
	public async Task Admin_Returns_Redirect_When_Not_Signed_In() {
		await using var factory = new WebApplicationFactory<Program>();
		var doNotFollowRedirects = new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false };
		using var client = factory.CreateClient(doNotFollowRedirects);
		using var response = await client.GetAsync("/admin");
		response.StatusCode.ShouldBe(HttpStatusCode.Found);
		response.Headers.Location?.ToString().ShouldContain("/identity/account/login");
	}
}