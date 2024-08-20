using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Controllers;

public class CheckoutController(RockawayDbContext db, IMailSender mailSender, IClock clock) : Controller {

	private async Task<TicketOrder?> FindOrderAsync(Guid id) {
		return await db.TicketOrders
			.Include(o => o.Contents)
			.ThenInclude(c => c.TicketType).ThenInclude(tt => tt.Show).ThenInclude(s => s.Venue)
			.Include(o => o.Contents)
			.ThenInclude(c => c.TicketType).ThenInclude(tt => tt.Show).ThenInclude(s => s.HeadlineArtist)
			.Include(o => o.Contents)
			.ThenInclude(c => c.TicketType).ThenInclude(tt => tt.Show).ThenInclude(s => s.SupportSlots).ThenInclude(ss => ss.Artist)
			.FirstOrDefaultAsync(order => order.Id == id);
	}

	[HttpPost]
	public async Task<IActionResult> Confirm(Guid id, OrderConfirmationPostData post) {
		var ticketOrder = await FindOrderAsync(post.TicketOrderId);
		if (ticketOrder == default) return NotFound();
		if (id != ticketOrder.Id) return BadRequest();
		post.TicketOrder = new(ticketOrder);
		if (! ModelState.IsValid) return View(post);
		ticketOrder.CustomerEmail = post.CustomerEmail;
		ticketOrder.CustomerName = post.CustomerName;
		ticketOrder.CompletedAt = clock.GetCurrentInstant();
		await db.SaveChangesAsync();
		// ReSharper disable once InvokeAsExtensionMethod
		var mailData = new TicketOrderMailData(ticketOrder, UriExtensions.GetWebsiteBaseUri(Request));
		await mailSender.SendOrderConfirmationAsync(mailData);
		return RedirectToAction("Completed", new { id });
	}

	[HttpGet]
	public async Task<IActionResult> Completed(Guid id) {
		var ticketOrder = await FindOrderAsync(id);
		var data = new TicketOrderViewData(ticketOrder);
		return View(data);
	} 

	[HttpGet]
	public async Task<IActionResult> Confirm(Guid id) {
		var ticketOrder = await FindOrderAsync(id);
		if (ticketOrder == default) return NotFound();
		var model = new OrderConfirmationPostData() {
			TicketOrderId = id,
			TicketOrder = new(ticketOrder)
		};
		return View(model);
	}
}