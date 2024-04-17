using System.Net;
using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Rockaway.WebApp.Tests;

public class AdminTests {
	[Theory]
	[InlineData("/admin")]
	public async Task Admin_Page_Redirects_To_Login(string path) {
		await using var factory = new WebApplicationFactory<Program>();
		var doNotFollowRedirects = new WebApplicationFactoryClientOptions() {
			AllowAutoRedirect = false
		};
		using var client = factory.CreateClient(doNotFollowRedirects);
		using var response = await client.GetAsync(path);
		response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
	}

	[Fact]
	public async Task Admin_Has_Personalised_Nav() {
		var fakeUsername = $"{Guid.NewGuid()}@rockaway.dev";
		var browsingContext = BrowsingContext.New(Configuration.Default);
		await using var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder
				=> builder.AddFakeAuthentication(fakeUsername));
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/admin");
		var dom = await browsingContext.OpenAsync(req => req.Content(html));
		var title = dom.QuerySelector("a#manage");
		title.ShouldNotBeNull();
		title.InnerHtml.ShouldBe($"Hello {fakeUsername}!");
	}
}

