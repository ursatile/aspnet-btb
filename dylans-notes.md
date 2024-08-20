---
title: Dylan's Notes
layout: home
nav_order: 99999
typora-copy-images-to: ./images
summary: Dylan's notes and scribblings from workshop prep
---

### That Razor Blazor Client Library Thing

Creating the bits:

```
dotnet new blazor -int WebAssembly -o Bongolis
```

Add a Razor class lib project:

```
dotnet new razorclasslib -o Bongolis.RazorClassLib
dotnet add Bongolis reference .\Bongolis.RazorClassLib\
dotnet sln add Bongolis.RazorClassLib
```

plug in:

```
builder.Services.AddRazorPages();
```

and

```
app.MapRazorPages();
```

Add `/Pages/MyPage.cshtml`:

```
@page
@model Bongolis.Pages.MyPage

<!DOCTYPE html>

<html>
<head>
	<title>My Page</title>
</head>
<body>
<div>
	MY PAGE!
</div>
</body>
</html>
```







#### Commit Logs from NDC Porto

```
Mon 09:23:02 Initial commit
Mon 09:30:40 dotnet new razor -o Rockaway.WebApp
Mon 09:37:47 dotnet new xunit; new solution.
Mon 09:45:26 editorconfig, serilog, and our solution structure
Mon 10:10:04 Page testing using Shouldly and AngleSharp
Mon 10:56:02 Fixed all the tests!
Mon 11:20:55 Add or update the Azure App Service build and deployment workflow config
Mon 11:30:26 Fix project paths in GitHub Actions YML file
Mon 11:37:07 Update step versions in YAML so we don't get NodeJS warnings
Mon 11:48:36 Add StatusReporter service and endpoint
Mon 12:09:38 FakeStatusReporter in test project

Mon 14:24:09 Sqlite, EF Core, Artists, and RockawayDbContext
Mon 14:30:30 Change YML from ubuntu-latest to ubuntu-2004 because that might fix a thing...
Mon 14:36:52 Commit before running dotnet aspnet-codegenerator for ArtistsController
Mon 14:52:44 Push before doing Venues exercise
Mon 15:42:36 Scaffoled VenuesController and entity
Mon 16:08:23 Setup for connecting to SQL database in Staging mode
Mon 16:22:41 Migrations for Artist and Venue
Mon 16:27:17 Deployment step for Azure workflow
Mon 16:35:22 Deployment step for Azure workflow, except actually for real this time.
Mon 16:52:27 Identity migration and admin user
Mon 17:04:37 Secured the admin area

Tue 09:12:54 Remove _LoginPartial reference from frontend layout page
Tue 09:18:33 Change package from false to . in build YAML
Tue 09:44:44 Split layout into layout and base
Tue 09:45:34 Oops.
Tue 09:53:36 Stripped down page layout to minimal semantic HTML
Tue 09:56:35 Add head section to layout page
Tue 10:12:01 Bootstrap and SASS compilation steps
Tue 10:14:48 Added elements and bootstrap cheat sheet to _layout footer nav
Tue 10:27:04 Remove scoped CSS and _Layout.cshtml.css - we're not using scoping in this workshop :)
Tue 11:13:24 start using mixins to apply styling to semantic layout
Tue 11:30:26 Rockaway page layout works on my machine... ;)
Tue 12:03:43 WebHacks demo for responsive nav
Tue 12:24:46 Added responsive nav for mobile
Tue 12:36:45 Responsive footer layout so we get dynamic design on smartphones

Tue 13:59:14 Elements
Tue 14:34:49 The tests pass! It must all be fine!
Tue 14:44:32 Artist views works!
Tue 15:02:05 Tour dates are working! Yay!
Tue 16:03:24 Finally got tag helper for country code working! Yay!
Tue 16:05:42 Country code working with helper attributes! Yay!
Tue 16:23:21 Add TicketTypes and sample data
Tue 16:39:40 Localised ticket prices!
Tue 16:43:06 DB migration for shows and ticket types
Tue 16:48:08 DB migration for shows and ticket types
```



Monday:

09:00-10:30 Razor Pages, minimal APIs, service registration, testing. **Start the Azure deployment before the coffee break!**

10:45-12:30 Model/View/Controller, our first entity, Areas, plumb in the admin area

13:30-15:00 Style up the frontend, SASS, responsive navigation

15:30-17:00 



In this section, we're going to set up the basic structure of our web and test projects.

MS1: Razor pages, minimal APIs, service registration

First, we'll create a new Razor Pages web application, a new xUnit test project, and a solution to put them in, and create a reference to our web app inside our test project:

```console
dotnet new razor -o Rockaway.WebApp
dotnet new xunit -o Rockaway.WebApp.Tests
dotnet new sln
dotnet sln add Rockaway.WebApp
dotnet sln add Rockaway.WebApp.Tests
dotnet add Rockaway.WebApp.Tests reference Rockaway.WebApp
```

This gives us a basic web app with Razor Pages support.

Add the `.editorconfig`

`dotnet format`

Add Serilog:

```
cd Rockaway.WebApp
dotnet add package Serilog.AspNetCore
```

and in Program.cs

```csharp
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
```

That's enough. Go live. Ship it.

Set up the Azure app. It'll push the `yml` file to the Git repo

Pull it.

Fix the project path, add the unit tests

Watch it go live.

Turn on application insights



How can we test it?

In `Rockaway.WebApp.Tests`:

```console
$ dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

This gives us access to the `WebApplicationFactory`.

Hack the Rockaway.WebApp.csproj to add InternalsVisibleTo:

```xml
	<ItemGroup>
		<InternalsVisibleTo Include="Rockaway.WebApp.Tests" />
	</ItemGroup>
```

(NCrunch: this requires a Visual Studio close & reopen)

Remove UnitTest1

Add `PagesTests.cs`, in the `Pages` namespace:

```csharp
namespace Rockaway.WebApp.Tests.Pages;

public class PagesTests {
	[Fact]
	public async Task Index_Page_Returns_Success() {
		var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		var result = await client.GetAsync("/");
		result.EnsureSuccessStatusCode();
	}
}
```

Add Shouldly and AngleSharp.

```console
dotnet add package AngleSharp
dotnet add package Shouldly
```

Wire in a test that the `<title`> element of the home page says **Rockaway**

```csharp
[Fact]
public async Task Homepage_Title_Has_Correct_Content() {
    var browsingContext = BrowsingContext.New(Configuration.Default);
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    var html = await client.GetStringAsync("/");
    var dom = await browsingContext.OpenAsync(req => req.Content(html));
    var title = dom.QuerySelector("title");
    title.ShouldNotBeNull();
    title.InnerHtml.ShouldBe("Rockaway");
}
```



**Service registration and the status endpoint**

We're going to add an endpoint at /status which will return a JSON response containing:

* The full name of the assembly that's hosting our application
* The last modification time of the assembly file (so we know when our app was built)
* The hostname of the server that's running our application
* The current local UTC time on that server

This endpoint should return a JSON document in this format:

```json
{
    "assembly": "Rockaway.WebApp",
    "modified": "2023-10-08T10:15:42Z",
    "hostname": "dylan-windows-pc",
    "datetime": "2023-10-08T12:12:40Z"
}
```

Get it live

**Create a new web app + database setup on Azure**

Exercises

1. Create a Contact Us page at /contact
2. The page title should be "Contact Us" - verify this with a test
3. Create an uptime endpoint, which returns a JSON object showing how long it is since the application was last started or restarted.

## END OF PART ONE

### Part 2: ASP.NET MVC and Entity Framework, DbContext, 

Milestone: /artists - a list of artists

Add MVC support:

```csharp
builder.Services.AddControllersWithViews();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");	
```

Plug in EF Core. We're going to install versions for both SQL Server and Sqlite:

```console
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Create our first entity:

```csharp
namespace Rockaway.WebApp.Data.Entities;

public class Artist {
	public Guid Id { get; set; }
	[MaxLength(100)]
	public string Name { get; set; } = String.Empty;
	[MaxLength(500)]
	public string Description { get; set; } = String.Empty;

	[MaxLength(100)]
	[Unicode(false)]
    [RegularExpression("^[a-z0-9-]{2,100}", 
		ErrorMessage = "Slug must be 2-100 characters and contain only a-z, 0-9 and hyphen (-) characters")]
	public string Slug { get; set; } = String.Empty;

	public Artist() { }

	public Artist(Guid id, string name, string description, string slug) {
		this.Id = id;
		this.Name = name;
		this.Description = description;
		this.Slug = slug;
	}
}
```

Create our **DbContext**:

```csharp
using Microsoft.EntityFrameworkCore.Infrastructure;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Data;

public class RockawayDbContext : DbContext {
	// We must declare a constructor that takes a DbContextOptions<RockawayDbContext>
	// if we want to use Asp.NET to configure our database connection and provider.
	public RockawayDbContext(DbContextOptions<RockawayDbContext> options) : base(options) { }

	public DbSet<Artist> Artists { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		// Override EF Core's default table naming (which pluralizes entity names)
		// and use the same names as the C# classes instead
		// and use the same names as the C# classes instead
		var rockawayEntities = modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType.Namespace == typeof(Artist).Namespace);
		foreach (var entity in rockawayEntities) entity.SetTableName(entity.DisplayName());

		modelBuilder.Entity<Artist>(entity => {
			entity.HasIndex(artist => artist.Slug).IsUnique();
		});
	}
}
```

Register the DbContext:

```
var sqlite = new SqliteConnection("Data Source=:memory:");
sqlite.Open();
builder.Services.AddDbContext<RockawayDbContext>(options => options.UseSqlite(sqlite));
```

Add the code generator:

```dotnetcli
dotnet tool install -g dotnet-aspnet-codegenerator
```

If you run it now, it'll say

```
Building project ...
Scaffolding failed.
Add Microsoft.VisualStudio.Web.CodeGeneration.Design package to the project as a NuGet package reference.
To see more information, enable tracing by setting environment variable 'codegen_trace' = 1.
RunTime 00:00:05.05
```

So we install the package

```
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

and this one:

```
 Microsoft.EntityFrameworkCore.Tools
```

Scaffold the ArtistController

```dotnetcli
dotnet aspnet-codegenerator controller -name ArtistsController -m Artist -dc RockawayDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

OK, run it.

It'll look like garbage

Move ViewStart and ViewImports into / so they apply to *everything*, not just pages.

Sorted.

### Sample Data

Two problems:

1. We don't have any data.
2. Any data we *do* create will get deleted every time we restart the app.

Solution: Sample Data

* Add the `SampleData.Artist` class
* Add the `SampleData` TestGuid int
* add the HasData

*At this point, we're seeding from the Artist entity directly.*

## Exercise: Add Venues

* Add the Venue.cs class

* Scaffold the VenuesController

* Add Venues to the RockawayDbContext:

  * Make the Slug property unique

  

  

While we're here, let's add /Artists and /Venues to the site navigation menu in _Layout.cshtml





### Going Live: Switching from Sqlite to SQL Server

Wire up the switch of DB providers

```
dotnet run --environment Staging
```

| Environment | Errors    | Database   |
| ----------- | --------- | ---------- |
| Development | Developer | Sqlite     |
| Staging     | Developer | SQL Server |
| Production  | Sanitised | SQL Server |
| UnitTest*   | Sanitised | Sqlite     |

*The UnitTest environment is used by the NCrunch test runner*

Generate the migration

```console
dotnet ef migrations add InitialCreate -- --environment Staging
dotnet format
```

Inspect it.

Roll it back

Fix the table naming convention

```csharp
// Override EF Core's default table naming (which pluralizes entity names)
// and use the same names as the C# classes instead
foreach (var entity in modelBuilder.Model.GetEntityTypes()) {
    entity.SetTableName(entity.DisplayName());
}
```

Recreate it:

```
dotnet ef migrations add InitialCreate -- --environment Staging
dotnet format
```

Apply it locally:

```
dotnet ef database update -- --environment Staging
```

Commit it

Apply DB migrations via GitHub Actions

1. Add the connection string as an Actions Secret
   1. from the project repo, Settings, Secrets and Variables, add a new secret AZURE_SQL_CONNECTIONSTRING
2. Modify Program.cs to take a migration switch
3. Add the chunk to the Github Action

### NEXT

**Adding identity support**

Scaffolding Identity:

[https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-7.0&tabs=netcore-cli](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-7.0&tabs=netcore-cli)

```
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

To wire up identity, we actually need to do these things:

1. Make RockawayDbContext inherit from `IdentityDbContext<IdentityUser>`
2. Create `/Pages/Shared/_LoginPartial.cshtml`:

```html
@using Microsoft.AspNetCore.Identity

@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager

<ul class="navbar-nav">
@if (SignInManager.IsSignedIn(User))
{
    <li class="nav-item">
        <a id="manage" class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Manage/Index" title="Manage">Hello @UserManager.GetUserName(User)!</a>
    </li>
    <li class="nav-item">
        <form id="logoutForm" class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Page("/Index", new { area = "" })">
            <button id="logout" type="submit" class="nav-link btn btn-link text-dark border-0">Logout</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-dark" id="register" asp-area="Identity" asp-page="/Account/Register">Register</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" id="login" asp-area="Identity" asp-page="/Account/Login">Login</a>
    </li>
}
</ul>

```

Add LoginPartial to _Layout.cshtml

```html
	                </ul>
	                <partial name="_LoginPartial" />
                </div>
            </div>
        </nav>
```



Edit `_ViewImports.cshtml` and add:

```
@using Microsoft.AspNetCore.Identity
```

and finally, in Program.cs

```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<RockawayDbContext>();

```

That's it. Now we have identity.

### Add a sample user

To add a sample user:

```
using Microsoft.AspNetCore.Identity;

namespace Rockaway.WebApp.Data.Sample;

public partial class SampleData {

	public static class Users {
		static Users() {
			var hasher = new PasswordHasher<IdentityUser>();
			Admin = new() {
				Id = "rockaway-sample-admin-user",
				Email = "admin@rockaway.dev",
				NormalizedEmail = "admin@rockaway.dev".ToUpperInvariant(),
				UserName = "admin@rockaway.dev",
				NormalizedUserName = "admin@rockaway.dev".ToUpperInvariant(),
				LockoutEnabled = true,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			Admin.PasswordHash = hasher.HashPassword(Admin, "p@ssw0rd");
		}
		public static IdentityUser Admin { get; }
	}
}
```

and

```
modelBuilder.Entity<IdentityUser>().HasData(Users.Admin);
```

Run it. Boom. It works.

## DB Migrations for Identity

Now, we need to create the migration:

```console
dotnet ef migrations add AddAspNetIdentity -- --environment Staging
```

Inspect it - erk. It's generated some PROPERLY weird table names.

We need to tighten up our filter.

Remove the migration:

```console
dotnet ef migrations remove -- --environment Staging
```

Modify the table name code:

```csharp
var rockawayEntityNamespace = typeof(Artist).Namespace;
var rockawayEntities = modelBuilder.Model.GetEntityTypes()
    .Where(e => e.ClrType.Namespace == rockawayEntityNamespace);
foreach (var entity in rockawayEntities) {
    entity.SetTableName(entity.DisplayName());
}
```

Recreate and inspect the migration:

```
dotnet ef migrations add AddAspNetIdentity -- --environment Staging
dotnet format
```

Sweet.

Let's check the migration works when we run it locally:

```
dotnet ef database update -- --environment Staging
```

OK, and now when we deploy our code, the Github Action will apply the same migration.

Boom!

## Moving admin into a secure area

Next up: move all the admin stuff into an `/admin` area and secure it.

```
dotnet aspnet-codegenerator area Admin
```

Move the controllers and views:

```
/Controllers
	/ArtistsController.cs	--> /Areas/Admin/Controllers/ArtistsController.cs
	/VenuesController.cs	--> /Areas/Admin/Controllers/VenuesControllers.cs
/Views
	/Artists/* 				--> move everything to /Areas/Admin/Views/Artists
	/Venues/* 				--> move everything to /Areas/Admin/Views/Venues
```

(If you do this in an IDE, it'll probably fix the namespaces for you; if not, you'll need to fix them yourself)

Add the attribute to the controllers:

```csharp
[Area("admin")]
public class VenuesController : Controller {
  //...
}
```

Add a new route to program.cs which uses our admin area:

```csharp
app.MapAreaControllerRoute("admin", "Admin", "Admin/{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
```

## Securing the admin area

Add `.RequireAuthorization()` to the area route; this makes our admin area only accessible to users who are already signed in.

## Isolating admin area layout

Let's move the standard layouts with all the Bootstrap & scaffolding stuff into their own area, so we can keep all the advantages of rapid development for admin area code, but have total control of our customer-facing app.

#### Move the login partial

Move _LoginPartial to `/Areas/Admin/Views/Shared`

Remove any reference to LoginPartial from `/Views/Shared/_Layout.cshtml`

#### Create the admin layout

```html
@* /Areas/Admin/Views/Shared/_AdminLayout.cshtml *@

<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>@(ViewData["Title"] ?? "Rockaway")</title>
	<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
	<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
	<link rel="stylesheet" href="~/Rockaway.WebApp.styles.css" asp-append-version="true" />
	<style>
		body { overflow-y: scroll; }
	</style>
</head>
<body>
	<header>
		<nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
			<div class="container">
				<a class="navbar-brand" asp-area="Admin" asp-page="/Index">Rockaway Admin</a>
				<button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
						aria-expanded="false" aria-label="Toggle navigation">
					<span class="navbar-toggler-icon"></span>
				</button>
				<div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
					<ul class="navbar-nav flex-grow-1">
						<li class="nav-item">
							<a class="nav-link text-dark" asp-area="Admin" asp-action="Index" asp-controller="Artists">Artists</a>
						</li>
						<li class="nav-item">
							<a class="nav-link text-dark" asp-area="Admin" asp-action="Index" asp-controller="Venues">Venues</a>
						</li>
					</ul>
					<partial name="~/Areas/Admin/Views/Shared/_LoginPartial.cshtml" />
				</div>
			</div>
		</nav>
	</header>
	<div class="container">
		<main role="main" class="pb-3">
			@RenderBody()
		</main>
	</div>
	<footer class="border-top footer text-muted">
		<div class="container">
			&copy; 2023 - Rockaway.WebApp - <a asp-area="" asp-page="/Privacy">Privacy</a>
		</div>
	</footer>
	<script src="~/lib/jquery/dist/jquery.min.js"></script>
	<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
	<script src="~/js/site.js" asp-append-version="true"></script>
	@await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

#### Set up _ViewStarts

Create `/Areas/Admin/_ViewStart.cshtml` :

```
@{
	Layout = "_AdminLayout";
}
```

Create `/Areas/Identity/Pages/_ViewStart.cshtml` (which overrides the "invisible" page that's part of the UI assembly):

```csharp
@{
    Layout = "/Areas/Admin/Views/Shared/_AdminLayout.cshtml";
}
```

#### Create the admin home page:

`Areas/Admin/Pages/Index.cshtml`

```html
@page
Welcome to the Rockaway admin area.
```

Lock down the home page (and any other Razor Pages under `/admin/`)

```csharp
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin","/"));
```

DONE.

## STYLING THE FRONTEND

Drop in the Elements view. That'll give us all the bits we're talking about.

Enabling SASS

Grab the Bootstrap SASS source code, unzip it

https://getbootstrap.com/docs/4.1/getting-started/download/

https://github.com/twbs/bootstrap/archive/v4.1.3.zip

**DO NOT USE THE NUGET PACKAGE IT IS MADE OUT OF LIES**

Copy the `scss` folder from the download to `wwwroot/lib/bootstrap/dist/`

Install the SASS compiler:

https://github.com/koenvzeijl/AspNetCore.SassCompiler

```
dotnet add package AspNetCore.SassCompiler
```

Add the config to `appsettings.json`

```
"SassCompiler": {
    "SourceFolder": "wwwroot/sass",
    "TargetFolder": "wwwroot/css",
    "Arguments": "--style=compressed",
    // You can override specific options based on the build configuration
    "Configurations": {
        "Debug": { // These options apply only to Debug builds
            "Arguments": "--style=expanded"
        }
    }
}
```

Add the service to debug builds:

```csharp
#if DEBUG
builder.Services.AddSassCompiler();
#endif
```

Update `/Pages/Shared/_Layout.cshtml`

```html
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>@(ViewData["Title"] ?? "Rockaway")</title>
	<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
	@await RenderSectionAsync("Head", required: false)
</head>
<body class="container">
	<header>
		<a asp-area="Admin" asp-page="/Index">Rockaway</a>
		<nav>
			<ul>
				<li>
					<a asp-action="Index" asp-controller="Artists">Artists</a>
				</li>
				<li class="nav-item">
					<a asp-action="Index" asp-controller="Venues">Venues</a>
				</li>
			</ul>
		</nav>
	</header>
	<main>
		@RenderBody()
	</main>
	<footer>
		&copy; 2023 - Rockaway.WebApp - <a asp-area="" asp-page="/Privacy">Privacy</a>
	</footer>
	<script src="~/lib/jquery/dist/jquery.min.js"></script>
	<script src="~/js/site.js" asp-append-version="true"></script>
	@await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

Create `wwwroot/sass/site.scss`:

```scss
@import '../lib/bootstrap/dist/scss/bootstrap-grid.scss';


```

STILL TO DO

## Page layout, font and colour

set up fancy-panel

add PT Sans Narrow 

```
@import url('https://fonts.googleapis.com/css2?family=PT+Sans+Narrow:wght@400;700&display=swap');

$font-family: 'PT Sans Narrow', Arial, helvetica, sans-serif;
```



Responsive navigation

Artists browser (with photos!)

Venue browser (with country code helpers)

Add shows

* Admin area
* NodaTime

Browser checkout process

Sending confirmation email

Done













Let's lock it down:

```Authorization/AuthorizationPolicyExtensions.cs`

```
using Microsoft.AspNetCore.Authorization;
namespace Rockaway.WebApp.Authorization;

public static class AuthorizationPolicyExtensions {
	private static AuthorizationPolicy BuildEmailDomainPolicy(string domain) => new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireEmailDomain(domain)
		.Build();

	public static IServiceCollection BuildEmailDomainPolicy(this IServiceCollection services,
		string policyName, string emailDomain) {
		var policy = BuildEmailDomainPolicy("rockaway.dev");
		services.AddAuthorization(options => options.AddPolicy(policyName, policy));
		return services;
	}

	public static AuthorizationPolicyBuilder RequireEmailDomain(this AuthorizationPolicyBuilder builder, string domain) {
		domain = domain.StartsWith("@") ? domain : $"@{domain}";
		return builder.RequireAssertion(ctx => {
			var email = ctx.User?.Identity?.Name ?? "";
			return email.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase);
		});
	}
}
```

Wire up an admin homepage:

```console
dotnet aspnet-codegenerator controller -name Home -namespace Rockaway.WebApp.Areas.Admin.Controllers -outDir Areas/Admin/Controllers
dotnet aspnet-codegenerator view Areas/Admin/Views/Home/Index Empty --useDefaultLayout
```

Add the Area attribute:

```
using Microsoft.AspNetCore.Mvc;

namespace Rockaway.WebApp.Areas.Admin.Controllers; 

[Area("admin")]
public class HomeController : Controller {
	public IActionResult Index() {
		return View();
	}
}
```

Bingo. We have:

a secure Admin area

CRUD operations for artists and venues

What's next?

Let's wire up the rest of our domain model:



/snip

OK, now we need to plug in the admin controller for editing shows.

This is where it gets gnarly.































**Exercise**

Let's plug in Venues

Here's the entity class:

```
namespace Rockaway.WebApp.Data.Entities;

public class Venue {

	public Venue() { }

	public Venue(Guid id, string name, string address, string city, string countryCode, string? postalCode, string? telephone, string? websiteUrl) {
		Id = id;
		Name = name;
		Address = address;
		City = city;
		CountryCode = countryCode;
		PostalCode = postalCode;
		Telephone = telephone;
		WebsiteUrl = websiteUrl;
	}

	public Guid Id { get; set; }
	[MaxLength(100)]
	public string Name { get; set; } = String.Empty;

	[MaxLength(500)]
	public string Address { get; set; } = String.Empty;

	public string City { get; set; } = String.Empty;

	[Unicode(false)]
	[MaxLength(2)]
	public string CountryCode { get; set; } = String.Empty;

	public string? PostalCode { get; set; }

	[Phone]
	public string? Telephone { get; set; }

	[Url]
	public string? WebsiteUrl { get; set; }

	public string FullName => $"{Name}, {City}, {CountryCode}";
}
```

Here's the sample data for Venues:

```
dotnet aspnet-codegenerator controller -name VenuesController -m Venue -dc RockawayDbContext --useDefaultLayout --referenceScriptLibraries
```

Add custom validation for Country

Plug in the Country class

Add tests for country validation

```
	public async Task CreateVenueWithInvalidCountryCodeReturnsError() {
		var c = new VenuesController(null!);
		var post = new Venue { CountryCode = "XX" };
		var result = await c.Create(post) as ViewResult;
		result.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
	}
```



Add test demonstrating how to create country by using an inmemory DB Context:

```
	[Fact]
	public async Task CreateVenueWithValidCountryCreatesCountry() {
		var options = new DbContextOptionsBuilder<RockawayDbContext>().UseSqlite("Data Source=:memory:").Options;
		var db = new RockawayDbContext(options);
		await db.Database.GetDbConnection().OpenAsync();
		await db.Database.EnsureCreatedAsync();
		var c = new VenuesController(db);
		var venue = new Venue {
			Name = "Test Venue", Address = "123 Test Street", City = "Test", CountryCode = "PT",
			Telephone = "1234", WebsiteUrl = "https://example.com"
		};
		await c.Create(venue);
		var created = db.Venues.Single(v => v.Name == "Test Venue");
		created.ShouldBeEquivalentTo(venue);
	}
```

Replace Country input with dropdownlist on the input page:

```
<label asp-for="CountryCode" class="control-label"></label>
<select asp-for="CountryCode" asp-items="Country.AllCountries.Select(c => new SelectListItem(c.Name, c.Code))">
    <option>select...</option>
</select>
<span asp-validation-for="CountryCode" class="text-danger"></span>
```

Get it live on Azure

This means making it work with SQL Server, so we need to generate the migrations

```

```

do a dotnet ef database update:

```console

```

and then build and deploy.

**Script to deploy all the things:**

```cmd
for /d %1 in (C:\projects\github\ursatile\mwnet\examples\*) do xcopy /s /Y %~f1\Rockaway\ . & git add . & git commit -m "Module %~n1"
```



