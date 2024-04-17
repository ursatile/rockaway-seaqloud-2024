using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Areas.Admin.Controllers;

[Area("admin")]
public class VenuesController : Controller {
	private readonly RockawayDbContext context;

	public VenuesController(RockawayDbContext context) {
		this.context = context;
	}

	// GET: Venues
	public async Task<IActionResult> Index() {
		var venues = await context.Venues.ToListAsync();
		//TODO: implement a security policy that isn't stupid!
		var model = new VenueListViewModel(venues) {
			AllowCreate = DateTime.Now.Second % 2 == 0,
			AllowEdit = DateTime.Now.Second % 3 == 0,
			AllowView = DateTime.Now.Second % 5 == 0
		};
		return View(model);
	}

	// GET: Venues/Details/5
	public async Task<IActionResult> Details(Guid? id) {
		if (id == null) return NotFound();
		var venue = await context.Venues.FirstOrDefaultAsync(m => m.Id == id);
		if (venue == null) return NotFound();
		return View(venue);
	}

	// GET: Venues/Create
	public IActionResult Create() {
		return View();
	}

	// POST: Venues/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("Id,Name,Slug,Address,City,CountryCode,PostalCode,Telephone,WebsiteUrl")] Venue venue) {
		if (!venue.WebsiteUrl.StartsWith("https")) {
			ModelState.AddModelError(nameof(venue.WebsiteUrl), "Websites must be secured!");
		}
		if (!ModelState.IsValid) return View(venue);
		venue.Id = Guid.NewGuid();
		context.Add(venue);
		await context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	// GET: Venues/Edit/5
	public async Task<IActionResult> Edit(Guid? id) {
		if (id == null) {
			return NotFound();
		}

		var venue = await context.Venues.FindAsync(id);
		if (venue == null) {
			return NotFound();
		}
		return View(venue);
	}

	// POST: Venues/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Slug,Address,City,CountryCode,PostalCode,Telephone,WebsiteUrl")] Venue venue) {
		if (id != venue.Id) {
			return NotFound();
		}

		if (ModelState.IsValid) {
			try {
				context.Update(venue);
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException) {
				if (!VenueExists(venue.Id)) {
					return NotFound();
				} else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(venue);
	}

	// GET: Venues/Delete/5
	public async Task<IActionResult> Delete(Guid? id) {
		if (id == null) {
			return NotFound();
		}

		var venue = await context.Venues
			.FirstOrDefaultAsync(m => m.Id == id);
		if (venue == null) {
			return NotFound();
		}

		return View(venue);
	}

	// POST: Venues/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(Guid id) {
		var venue = await context.Venues.FindAsync(id);
		if (venue != null) {
			context.Venues.Remove(venue);
		}

		await context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool VenueExists(Guid id) {
		return context.Venues.Any(e => e.Id == id);
	}
}