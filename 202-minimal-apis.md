---
title: "2.2 Using Minimal APIs"
layout: module
nav_order: 10202
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll use ASP.NET's Minimal APIs feature to add a status endpoint to our application"
previous: mwnet201
complete: mwnet202
---

In the last module, we set up continuous deployment (CD) for our project using GitHub Actions.

CD is great: run your tests, if they're green, you're good to go. But that just means you know your project worked when you deployed it... is it still working now?

We can add monitoring endpoints to various parts of our application, but I've often found it really useful to create a dedicated status endpoint that'll tell me a few vital things about the status of the application.

ASP.NET Core makes this really straightforward thanks to a feature called **minimal APIs** -- a way to map arbitrary request URLs onto snippets of code. In fact, thanks to minimal APIs, the smallest web application in .NET looks like this:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
```

We're going to use minimal APIs to add a **status endpoint** to our application.

First we'll create an interface for our status reporter service, an implementation of that service, and a class representing the status itself:

```csharp
// Rockaway.WebApp/Services/IStatusReporter.cs
{% include_relative examples/202/Rockaway.WebApp/Services/IStatusReporter.cs %}
```

```csharp
// Rockaway.WebApp/Services/StatusReporter.cs
{% include_relative examples/202/Rockaway.WebApp/Services/StatusReporter.cs %}
```

```csharp
// Rockaway.WebApp/Services/ServerStatus.cs
{% include_relative examples/202/Rockaway.WebApp/Services/ServerStatus.cs %}
```

Minimal APIs can accept a service, like an instance of `IStatusReporter`, as an input, but we need to register the service to make this work.

```csharp
// Rockaway.WebApp/Program.cs
{% include_relative examples/202/Rockaway.WebApp/Program.cs %}
```

Now if we load `/status` in a browser, we can see a few useful bits of information:

```json
{
    "assembly": "Rockaway.WebApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
    "modified": "2023-10-13T18:20:42.0000000+00:00",
    "hostname": "c4817b452332",
    "dateTime": "2023-10-13T18:42:27.9913632+00:00"
}
```

> The **modified** date here is particularly useful if you're troubleshooting a problematic deployment, because it means you can see *exactly* when the assembly was built -- and if it isn't changing when you push a new release, it means your new build isn't making it onto the server.

### Testing the Status Endpoint

The `StatusReporter` service works great, but it's hard to test because it's **non-deterministic** -- every time we call it, it could return different information depending on where it's running, when it was build, and the time of day when we run our test.

We can test that the endpoint returns a successful status:

```csharp
[Fact]
public async Task Status_Endpoint_Returns_Status() {
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    var result = await client.GetAsync("/status");
    result.EnsureSuccessStatusCode();
}
```

But... we can also override the `StatusReporter` service, so we can inject our own implementation of `IStatusReporter`. This is one of the most useful features of WebApplicationFactory, since it gives us a very straightforward way to replace dependencies and external services in our integration tests.

To do this, we call `.WithWebHostBuilder` when creating our factory:

```csharp
var factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder => builder.ConfigureServices(services => {
        services.AddSingleton<IStatusReporter>(new TestStatusReporter());
    }));
var client = factory.CreateClient();
```



We can also use the `System.Text.Json.JsonSerializer` to unpack the JSON response from our status endpoint and verify that it's returning the correct information:

```csharp
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
    var factory = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder => builder.ConfigureServices(services => {
            services.AddSingleton<IStatusReporter>(new TestStatusReporter());
        }));
    var client = factory.CreateClient();
    var json = await client.GetStringAsync("/status");
    var status = JsonSerializer.Deserialize<ServerStatus>(json, jsonSerializerOptions);
    status.ShouldNotBeNull();
    status.ShouldBeEquivalentTo(testStatus);
}
```

> Note the `status.ShouldBeEquivalentTo()` method we're calling here; this is a `Shouldly` method which compares object graphs, checking that every property on one object is equal to the corresponding property on another; it's ideal for this kind of scenario.

## Exercise: Add Uptime

We want to see how long it is since our application was last deployed or restarted.

1. Add Uptime to the existing `/status` endpoint, as a human-readable string - e.g. `"142:28:46"` indicates 142 hours, 28 minutes and 46 seconds of uptime.
2. Add a new API endpoint at `/uptime` which returns a single number, being the number of seconds since the app was last restarted - so in the scenario above, the endpoint will return `512926`

Include endpoint tests for your uptime service.





