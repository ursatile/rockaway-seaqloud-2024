using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Hosting;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Api;
using Rockaway.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin", "/"));
builder.Services.AddControllersWithViews(options => {
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddSingleton<IStatusReporter>(new StatusReporter());
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

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
		logger.LogInformation("apply-migrations=true was specified. Applying EF migrations and then exiting.");
		db.Database.Migrate();
		logger.LogInformation("EF database migrations applied successfully.");
		Environment.Exit(0);
	}
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());
app.MapGet("/uptime", (IStatusReporter reporter) => (long) reporter.GetUptime().TotalSeconds);
app.MapAreaControllerRoute(
	name: "admin",
	areaName: "Admin",
	pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.MapArtistEndpoints();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();

ILogger<T> CreateAdHocLogger<T>() => LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger<T>();