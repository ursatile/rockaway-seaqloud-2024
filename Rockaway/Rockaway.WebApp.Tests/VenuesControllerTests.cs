using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Controllers;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Shouldly;

namespace Rockaway.WebApp.Tests;

public class VenuesControllerTests {

	[Fact]
	public async Task Insecure_Venue_Url_Returns_Error() {
		var sqliteConnection = new SqliteConnection("Data Source=:memory:");
		sqliteConnection.Open();
		var options = new DbContextOptionsBuilder<RockawayDbContext>()
			.UseSqlite(sqliteConnection)
			.Options;
		var db = new RockawayDbContext(options);
		var c = new VenuesController(db);
		var v = new Venue {
			Name = "name",
			Slug = "my-venue",
			WebsiteUrl = "ftp://venue.com"
		};
		var result = await c.Create(v) as ViewResult;
		result.ShouldNotBeNull();
	}
}
