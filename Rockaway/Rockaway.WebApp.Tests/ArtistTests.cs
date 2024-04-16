using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Rockaway.WebApp.Data.Sample;
using Shouldly;

namespace Rockaway.WebApp.Tests;

public class ArtistTests {

	public static IEnumerable<object[]> SampleArtists() {
		return SampleData.Artists.AllArtists.Select(a => new [] { a.Name });
	}

	[Theory]
	[MemberData(nameof(SampleArtists))]
	public async Task Artist_Page_Contains_All_Artists(string artistName) {
		await using var factory = new WebApplicationFactory<Program>();
		var client = factory.CreateClient();
		var html = await client.GetStringAsync("/artists");
		var decodedHtml = WebUtility.HtmlDecode(html);
		decodedHtml.ShouldContain(artistName);
	}
}
