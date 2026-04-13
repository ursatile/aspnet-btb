using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Pages;

public class ArtistsModel(RockawayDbContext db) : PageModel {
	public IEnumerable<Artist> Artists = default!;

	public void OnGet() {
		Artists = db.Artists.OrderBy(a => a.Name);
	}
}