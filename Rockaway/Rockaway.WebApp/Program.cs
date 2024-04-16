using Rockaway.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IStatusReporter>(new StatusReporter());

var app = builder.Build();

app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());

app.MapGet("/uptime", (IStatusReporter reporter) => (long)reporter.GetUptime().TotalSeconds);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();