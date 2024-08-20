using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Rockaway.WebApp.Services;
using Shouldly;

namespace Rockaway.WebApp.Tests {
	public class EndpointTests {

		[Fact]
		public async Task Status_Endpoint_Works() {
			await using var factory = new WebApplicationFactory<Program>();
			var client = factory.CreateClient();
			var result = await client.GetAsync("/status");
			result.EnsureSuccessStatusCode();
		}
		
		private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

		private static readonly ServerStatus testStatus = new() {
			Assembly = "TEST_ASSEMBLY",
			Modified = new DateTimeOffset(2021, 2, 3, 4, 5, 6, TimeSpan.Zero).ToString("O"),
			Hostname = "TEST_HOST",
			DateTime = new DateTimeOffset(2022, 3, 4, 5, 6, 7, TimeSpan.Zero).ToString("O")
		};

		private class TestStatusReporter : IStatusReporter {
			public ServerStatus GetStatus() => testStatus;
		}

		[Fact]
		public async Task Status_Endpoint_Returns_Status() {
			await using var factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder => builder.ConfigureServices(services => {
					services.AddSingleton<IStatusReporter>(new TestStatusReporter());
				}));
			using var client = factory.CreateClient();
			var json = await client.GetStringAsync("/status");
			var status = JsonSerializer.Deserialize<ServerStatus>(json, jsonSerializerOptions);
			status.ShouldNotBeNull();
			status.ShouldBeEquivalentTo(testStatus);
		}
	}
}