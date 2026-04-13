using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rockaway.WebApp.Pages;

public class ContactModel : PageModel {
	private readonly ILogger<ContactModel> _logger;

	public ContactModel(ILogger<ContactModel> logger) {
		_logger = logger;
	}

	public void OnGet() {
	}
}