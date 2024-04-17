using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Hosting;

public static class ServiceCollectionExtensions {
	public static IServiceCollection AddRockawayStatusReporter(this IServiceCollection services) {
		services.AddSingleton<IStatusReporter>(new StatusReporter());
		return services;
	}
}