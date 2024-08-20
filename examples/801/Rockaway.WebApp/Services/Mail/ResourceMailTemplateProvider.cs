using System.Reflection;

namespace Rockaway.WebApp.Services.Mail;

public class ResourceMailTemplateProvider : IMailTemplateProvider {

	public string OrderConfirmationMjml => ReadEmbeddedResource("OrderConfirmation.mjml");
	public string OrderConfirmationText => ReadEmbeddedResource("OrderConfirmation.txt");

	private static string ReadEmbeddedResource(string resourceFileName) {
		var assembly = Assembly.GetEntryAssembly()!;
		var qualifiedName = $"{assembly.GetName().Name}.Templates.Mail.{resourceFileName}";
		var stream = assembly.GetManifestResourceStream(qualifiedName)!;
		return new StreamReader(stream).ReadToEnd();
	}

}