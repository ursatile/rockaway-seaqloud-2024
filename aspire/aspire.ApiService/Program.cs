var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

var random = new Random();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (ILogger<Program> logger) => {
	var r = random.Next(10);
	switch (r) {
		case 9:
			logger.LogError("Simulated weather exception");
			throw new Exception("Simulated exception from weather forecast");
		default:
			await Task.Delay(TimeSpan.FromSeconds(r/3));
			break;
	}
	logger.LogInformation("Information about a weather forecast");
	logger.LogCritical("Critical weather forecast");
	logger.LogDebug("Debug information about weather");
	logger.LogError("Weather error");
	logger.LogTrace("Trace weather!");
	logger.LogWarning("Weather warning");

	var forecast = Enumerable.Range(1, 5).Select(index =>
		new WeatherForecast
		(
			DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			Random.Shared.Next(-20, 55),
			summaries[Random.Shared.Next(summaries.Length)]
		))
		.ToArray();
	return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) {
	public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}
