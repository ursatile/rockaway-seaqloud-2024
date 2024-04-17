using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Rockaway.WebApp.Tests;

class FakeAuthenticationFilter : IStartupFilter {
	private readonly string emailAddress;

	public FakeAuthenticationFilter(string emailAddress) {
		this.emailAddress = emailAddress;
	}

	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
		builder => {
			var options = FakeAuthenticationOptions.Create(emailAddress);
			builder.UseMiddleware<FakeAuthenticationMiddleware>(options);
			next(builder);
		};

	private class FakeAuthenticationOptions {
		private FakeAuthenticationOptions(string emailAddress) {
			this.EmailAddress = emailAddress;
		}

		public string EmailAddress { get; }

		internal static IOptions<FakeAuthenticationOptions> Create(string emailAddress)
			=> Options.Create(new FakeAuthenticationOptions(emailAddress));
	}

	private class FakeAuthenticationMiddleware {
		private readonly RequestDelegate next;
		private readonly IOptions<FakeAuthenticationOptions> options;

		public FakeAuthenticationMiddleware(RequestDelegate next, IOptions<FakeAuthenticationOptions> options) {
			this.next = next;
			this.options = options;
		}

		private readonly string authenticationType = IdentityConstants.ApplicationScheme;
		private string EmailAddress => options.Value.EmailAddress;
		public async Task InvokeAsync(HttpContext context) {
			var claims = new Claim[] {
				new(ClaimTypes.Name, EmailAddress),
				new(ClaimTypes.Email, EmailAddress)
			};
			var identity = new ClaimsIdentity(claims, authenticationType);
			context.User = new(identity);
			await next(context);
		}
	}
}
