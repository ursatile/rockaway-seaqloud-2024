// Rockaway.WebApp/Hosting/HostEnvironmentExtensions.cs

using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Hosting;

public static class HostEnvironmentExtensions {
	private static readonly string[] sqliteEnvironments
		= { "UnitTest", Environments.Development };

	public static bool UseSqlite(this IHostEnvironment env)
	=> sqliteEnvironments.Contains(env.EnvironmentName);

	public static IServiceCollection AddRockawayStatusReporter(this IServiceCollection services) {
		services.AddSingleton<IStatusReporter>(new StatusReporter());
		return services;
	}
}