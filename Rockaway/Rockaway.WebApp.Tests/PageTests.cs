using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Rockaway.WebApp.Data;
using Shouldly;
using System.Net;
using Rockaway.WebApp.Data.Sample;

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



public class ArtistTests {

	public static IEnumerable<string> SampleArtists() {
		return SampleData.Artists.AllArtists.Select(a => a.Name);
	}
	[Theory]
	[MemberData(nameof(SampleArtists))]
	public async Task Artist_Page_Contains_All_Artists(string artistName) {
		await using var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/artists");
		var decodedHtml = WebUtility.HtmlDecode(html);
		//using var scope = factory.Services.CreateScope();
		//var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
		//var expected = db.Artists.ToList();
		decodedHtml.ShouldContain(artistName);
	}
}
