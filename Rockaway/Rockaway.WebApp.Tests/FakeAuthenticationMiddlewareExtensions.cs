// Rockaway.WebApp.Tests/FakeAuthenticationMiddlewareExtensions.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Rockaway.WebApp.Tests;

public static class FakeAuthenticationMiddlewareExtensions {
	public static IWebHostBuilder AddFakeAuthentication(this IWebHostBuilder builder, string email) {
		builder.ConfigureServices(services => {
			services.AddTransient<IStartupFilter>(_ => new FakeAuthenticationFilter(email));
		});
		return builder;
	}
}
