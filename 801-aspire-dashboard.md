---
title: "8.1 OpenTelemetry and ASP.NET Aspire"
layout: module
nav_order: 10702
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll configure our web app to run under ASP.NET Aspire, and add OpenTelemetry so we can use the Aspire dashboard to see what our requests are doing."
previous: rockaway702
complete: rockaway801
examples: examples/801/Rockaway
---

First, we'll create a new app host project, and add it to our solution:

```dotnetcli
dotnet new aspire-apphost -o Rockaway.AppHost
dotnet sln add Rockaway.AppHost
```

Add a reference in `Rockaway.AppHost` to `Rockaway.WebApp`:

```dotentcli
dotnet add Rockaway.AppHost reference Rockaway.WebApp
```

Add a **service defaults** project:

```dotnetcli
dotnet new aspire-servicedefaults -o Rockaway.ServiceDefaults
dotnet sln add Rockaway.ServiceDefaults
```

This gives us a stack of useful defaults for adding OpenTelemetry to an ASP.NET application -- take a look at `Rockaway.ServiceDefaults\Extensions.cs` to see what's actually getting wired up:

```csharp
{% include_relative {{ page.examples }}/Rockaway.ServiceDefaults/Extensions.cs %}
```

Now, add a reference in the web app to the service defaults project:

```dotnetcli
dotnet add Rockaway.WebApp reference Rockaway.ServiceDefaults
```

Update `Rockaway.WebApp/Program.cs` and add a call to `builder.AddServiceDefaults()` after `WebApplication.CreateBuilder`, and a call to `app.MapDefaultEndpoints()` just before `app.Run()`:

```csharp
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.AddServiceDefaults();

/* ...rest of Program.cs... */

app.MapDefaultEndpoints();
app.Run();
```

Update `Rockaway.AppHost/Program.cs`:

```csharp
{% include_relative examples/801/Rockaway.AppHost/Program.cs %}
```

Make sure Docker or Podman is running, then start the solution:

```
dotnet run --project Rockaway.AppHost
```



![image-20240604145157931](/images/image-20240604145157931.png)

Let's add telemetry for our database queries.

```dotnetcli
dotnet add package OpenTelemetry.Instrumentation.SqlClient --prerelease
```



