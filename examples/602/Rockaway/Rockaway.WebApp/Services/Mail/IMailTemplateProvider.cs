namespace Rockaway.WebApp.Services.Mail;

public interface IMailTemplateProvider {
	string OrderConfirmationMjml { get; }
	string OrderConfirmationText { get; }
}
