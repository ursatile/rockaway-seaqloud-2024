using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Hosting;
using Rockaway.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IStatusReporter>(new StatusReporter());

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

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
	using (var db = scope.ServiceProvider.GetService<RockawayDbContext>()!) {
		db.Database.EnsureCreated();
	}
}

app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());
app.MapGet("/uptime", (IStatusReporter reporter) => (long) reporter.GetUptime().TotalSeconds);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();

ILogger<T> CreateAdHocLogger<T>() {
	var config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.json", false, true)
		.AddEnvironmentVariables()
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
		.Build();
	return LoggerFactory.Create(lb => lb.AddConfiguration(config)).CreateLogger<T>();
}