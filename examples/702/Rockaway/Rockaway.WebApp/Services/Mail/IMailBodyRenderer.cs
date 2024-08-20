using Rockaway.WebApp.Models;

namespace Rockaway.WebApp.Services.Mail;

public interface IMailBodyRenderer {
	string RenderOrderConfirmationHtml(TicketOrderViewData data);
	string RenderOrderConfirmationText(TicketOrderViewData data);
}