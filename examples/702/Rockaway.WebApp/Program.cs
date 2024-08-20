using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Mjml.Net;
using RazorEngineCore;
using Rockaway.RazorComponents;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Hosting;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin", "/"));
builder.Services.AddControllersWithViews(options => {
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddSingleton<IStatusReporter>(new StatusReporter());
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

#if DEBUG
builder.Services.AddSingleton<IMailTemplateProvider>(new DebugMailTemplateProvider());
#else
builder.Services.AddSingleton<IMailTemplateProvider>(new ResourceMailTemplateProvider());
#endif
builder.Services.AddSingleton<IMailBodyRenderer, MailBodyRenderer>();
builder.Services.AddSingleton<IRazorEngine, RazorEngine>();
builder.Services.AddSingleton<IMjmlRenderer, MjmlRenderer>();

builder.Services.AddSingleton<IMailSender, SmtpMailSender>();
var smtpSettings = new SmtpSettings();
builder.Configuration.Bind("Smtp", smtpSettings);
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddSingleton<ISmtpRelay, SmtpRelay>();

#if DEBUG && !NCRUNCH
builder.Services.AddSassCompiler();
#endif

var logger = CreateAdHocLogger<Program>();

logger.LogInformation("Rockaway running in {environment} environment", builder.Environment.EnvironmentName);
// A bug in .NET 8 means you can't call extension methods from Program.Main, otherwise
// the aspnet-codegenerator tools fail with "Could not get the reflection type for DbContext"
// ReSharper disable once InvokeAsExtensionMethod
if (HostEnvironmentExtensions.UseSqlite(builder.Environment)) {
	logger.LogInformation("Using Sqlite database");
	var sqliteConnection = new SqliteConnection("Data Source=:memory:");
	sqliteConnection.Open();
	builder.Services.AddDbContext<RockawayDbContext>(options => options.UseSqlite(sqliteConnection));
} else {
	logger.LogInformation("Using SQL Server database");
	var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
	builder.Services.AddDbContext<RockawayDbContext>(options => options.UseSqlServer(connectionString));
}

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<RockawayDbContext>();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction()) {
	app.UseExceptionHandler("/Error");
	app.UseHsts();
} else {
	app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope()) {
	using var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
	// ReSharper disable once InvokeAsExtensionMethod
	if (HostEnvironmentExtensions.UseSqlite(app.Environment)) {
		db.Database.EnsureCreated();
	} else if (Boolean.TryParse(app.Configuration["apply-migrations"], out var applyMigrations) && applyMigrations) {
		logger.LogInformation("apply-migrations=true was specified. Applying EF migrations:");
		db.Database.Migrate();
		logger.LogInformation("EF database migrations applied successfully.");
		Environment.Exit(0);
	}
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());
app.MapAreaControllerRoute(
	name: "admin",
	areaName: "Admin",
	pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode();

app.Run();

ILogger<T> CreateAdHocLogger<T>() => LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger<T>();