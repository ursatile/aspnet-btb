using MailKit.Net.Smtp;
using MimeKit;

namespace Rockaway.WebApp.Services.Mail;

public class SmtpRelay(SmtpSettings settings, ILogger<SmtpRelay> logger) : ISmtpRelay {
	public async Task<string> SendMailAsync(MimeMessage mail) {
		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(settings.Host, settings.Port);
		if (settings.Authenticate) {
			await smtp.AuthenticateAsync(settings.Username, settings.Password);
		}
		var result = await smtp.SendAsync(mail);
		await smtp.DisconnectAsync(true);
		return result;
	}
}