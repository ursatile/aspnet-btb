using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Models;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Services.Mail;

namespace Rockaway.WebApp.Areas.Admin.Controllers;

[Area("admin")]
public class TicketOrdersController(RockawayDbContext context,
	IMailBodyRenderer mailRenderer) : Controller {

	public async Task<IActionResult> Index()
		=> View(await context.TicketOrders.ToListAsync());

	// GET: TicketOrders/Details/5
	public async Task<IActionResult> Details(Guid? id) {
		if (id == null) return NotFound();
		var ticketOrder = await context.TicketOrders
			.FirstOrDefaultAsync(m => m.Id == id);
		if (ticketOrder == default) return NotFound();
		return View(ticketOrder);
	}

	// GET: TicketOrders/Create
	public IActionResult Create() => View();

	// POST: TicketOrders/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(
		[Bind("Id,CustomerName,CustomerEmail,CreatedAt,CompletedAt")]
		TicketOrder ticketOrder
	) {
		if (!ModelState.IsValid) return View(ticketOrder);
		ticketOrder.Id = Guid.NewGuid();
		context.Add(ticketOrder);
		await context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	// GET: TicketOrders/Edit/5
	public async Task<IActionResult> Edit(Guid? id) {
		if (id == null) return NotFound();
		var ticketOrder = await context.TicketOrders.FindAsync(id);
		if (ticketOrder == default) return NotFound();
		return View(ticketOrder);
	}

	// POST: TicketOrders/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(Guid id, [Bind("Id,CustomerName,CustomerEmail,CreatedAt,CompletedAt")] TicketOrder ticketOrder) {
		if (id != ticketOrder.Id) return NotFound();
		if (!ModelState.IsValid) return View(ticketOrder);
		try {
			context.Update(ticketOrder);
			await context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException) {
			if (!TicketOrderExists(ticketOrder.Id)) return NotFound();
			throw;
		}
		return RedirectToAction(nameof(Index));
	}

	// GET: TicketOrders/Delete/5
	public async Task<IActionResult> Delete(Guid? id) {
		if (id == null) return NotFound();
		var ticketOrder = await context.TicketOrders.FirstOrDefaultAsync(m => m.Id == id);
		if (ticketOrder == null) return NotFound();
		return View(ticketOrder);
	}

	// POST: TicketOrders/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(Guid id) {
		var ticketOrder = await context.TicketOrders.FindAsync(id);
		if (ticketOrder != null) context.TicketOrders.Remove(ticketOrder);
		await context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	public async Task<IActionResult> Mail(Guid id, string format = "html") {
		var ticketOrder = await context.TicketOrders
			.Include(o => o.Contents).ThenInclude(item => item.TicketType)
			.Include(o => o.Show).ThenInclude(s => s.HeadlineArtist)
			.Include(o => o.Show).ThenInclude(s => s.Venue)
			.Include(o => o.Show).ThenInclude(s => s.SupportSlots).ThenInclude(ss => ss.Artist)
			.FirstOrDefaultAsync(m => m.Id == id);
		if (ticketOrder == default) return NotFound();
		// ReSharper disable once InvokeAsExtensionMethod
		var data = new TicketOrderMailData(ticketOrder, UriExtensions.GetWebsiteBaseUri(Request));
		switch (format) {
			case "html":
				var html = mailRenderer.RenderOrderConfirmationHtml(data);
				return Content(html, "text/html");
			default:
				var text = mailRenderer.RenderOrderConfirmationText(data);
				return Content(text, "text/plain", Encoding.UTF8);
		}
	}

	private bool TicketOrderExists(Guid id) {
		return context.TicketOrders.Any(e => e.Id == id);
	}
}