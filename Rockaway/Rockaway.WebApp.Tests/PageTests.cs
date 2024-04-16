using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Rockaway.WebApp.Data;
using Shouldly;

namespace Rockaway.WebApp.Tests;

public class PageTests {

	[Theory]
	[InlineData("/")]

	[InlineData("/privacy")]
	public async Task Page_Works(string path) {
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		using var response = await client.GetAsync(path);
		response.EnsureSuccessStatusCode();
	}

	[Fact]
	public async Task Homepage_Has_H1_Welcome_Message() {
		var browsingContext = BrowsingContext.New(Configuration.Default);
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		var html = await client.GetStringAsync("/");
		var dom = await browsingContext.OpenAsync(req => req.Content(html));
		var h1 = dom.QuerySelector("h1");
		h1.ShouldNotBeNull();
		h1.InnerHtml.ShouldBe("Welcome, Seaqloud!");
	}
}