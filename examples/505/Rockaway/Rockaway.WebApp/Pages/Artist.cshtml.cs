using Rockaway.WebApp.Data;
using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Pages;

public class ArtistModel(RockawayDbContext db) : PageModel {
	public ArtistViewData Artist = default!;

	public IActionResult OnGet(string slug) {
		var artist = db.Artists.FirstOrDefault(a => a.Slug == slug);
		if (artist == default) return NotFound();
		Artist = new(artist);
		return Page();
	}
}