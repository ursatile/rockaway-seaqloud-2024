using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Pages;

public class ArtistsModel : PageModel {
	private readonly RockawayDbContext db;
	public IEnumerable<Artist> Artists = default!;

	public ArtistsModel(RockawayDbContext db) {
		this.db = db;
	}

	public void OnGet() {
		Artists = db.Artists.OrderBy(a => a.Name);
	}
}