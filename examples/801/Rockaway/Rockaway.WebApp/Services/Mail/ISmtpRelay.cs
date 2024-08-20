using MimeKit;

namespace Rockaway.WebApp.Services.Mail;

public interface ISmtpRelay {
	Task<string> SendMailAsync(MimeMessage message);
}