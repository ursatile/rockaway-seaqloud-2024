using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IStatusReporter>(new StatusReporter());

var sqliteConnection = new SqliteConnection("Data Source=:memory:");
sqliteConnection.Open();
builder.Services.AddDbContext<RockawayDbContext>(options
	=> options.UseSqlite(sqliteConnection));

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
	using (var db = scope.ServiceProvider.GetService<RockawayDbContext>()!) {
		db.Database.EnsureCreated();
	}
}

app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());
app.MapGet("/uptime", (IStatusReporter reporter) => (long)reporter.GetUptime().TotalSeconds);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();