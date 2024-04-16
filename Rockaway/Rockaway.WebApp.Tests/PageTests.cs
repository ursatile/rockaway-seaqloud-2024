using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Rockaway.WebApp.Tests;

public class PageTests {

	[Fact]
	public async Task Status_Works() {
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		using var response = await client.GetAsync("/status");
		response.EnsureSuccessStatusCode();
	}
	
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

	// [Fact]
	// public async Task Shouldly_Is_Cool() {
	// 	string s1 = "hat-shaped";
	// 	string s2 = "hat–shaped";
	// 	s1.ShouldBe(s2);
	// }

	// OR if you want to test page endpoints individually:
	// public async Task Index_Page_Returns_Success()
	// {

	// 	await using var factory = new WebApplicationFactory<Program>();
	// 	using var client = factory.CreateClient();
	// 	using var response = await client.GetAsync("/");
	// 	response.EnsureSuccessStatusCode();
	// }
	// [Fact]
	// public async Task Privacy_Page_Returns_Success()
	// {
	// 	await using var factory = new WebApplicationFactory<Program>();
	// 	using var client = factory.CreateClient();
	// 	using var response = await client.GetAsync("/privacy");
	// 	response.EnsureSuccessStatusCode();
	// }
}
