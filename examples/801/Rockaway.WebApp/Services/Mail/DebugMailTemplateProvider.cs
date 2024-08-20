using System.Runtime.CompilerServices;

namespace Rockaway.WebApp.Services.Mail;

public class DebugMailTemplateProvider : IMailTemplateProvider {
	private string ReadFile(string filename, [CallerFilePath] string callerFilePath = "") {
		var path = Path.Combine(callerFilePath, "..", "..", "..", "Templates", "Mail", filename);
		return File.ReadAllText(path);
	}
	public string OrderConfirmationMjml => ReadFile("OrderConfirmation.mjml");
	public string OrderConfirmationText => ReadFile("OrderConfirmation.txt");
}