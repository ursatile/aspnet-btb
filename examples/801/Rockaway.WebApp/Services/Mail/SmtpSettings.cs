using Microsoft.IdentityModel.Tokens;

namespace Rockaway.WebApp.Services.Mail;

public class SmtpSettings {
	public string Host { get; set; } = "localhost";
	public string? Username { get; set; }
	public string? Password { get; set; }
	public int Port { get; set; } = 25;
	public bool Authenticate => !(Username.IsNullOrEmpty() || Password.IsNullOrEmpty());
}