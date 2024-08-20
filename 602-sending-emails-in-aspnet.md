---
title: "6.2 Sending Email"
layout: module
nav_order: 10602
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll learn how to use Mailkit and Mimekit to send email from an ASP.NET web application, and how to test email delivery using tools like Papercut and MailTrap."
previous: mwnet601
complete: mwnet602
examples: examples/602/Rockaway
---

In the last module, we rendered text and HTML versions of our order confirmation email. Now we need to actually send it.

Here's the workflow:

![mail-workflow](/images/mail-workflow.png)

In the last module we built the Renderer, Templates, MJML and RazorEngine components; now, we'll build the SmtpRelay and MailSender components.

To actually create and send the email, we'll use two open source libraries, [MailKit](https://github.com/jstedfast/MailKit) and [MimeKit](https://github.com/jstedfast/MimeKit):

``` 
dotnet add package MailKit
dotnet add package MimeKit
```

Now, add `IMailSender`:

```csharp
// Rockaway.WebApp/Services/Mail/IMailSender.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Services/Mail/IMailSender.cs %}
```
and `ISmtpRelay`:

```csharp
// Rockaway.WebApp/Services/Mail/ISmtpRelay.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Services/Mail/ISmtpRelay.cs %}
```
and their implementations, `SmtpMailSender`:

```csharp
// Rockaway.WebApp/Services/Mail/SmtpMailSender.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Services/Mail/SmtpMailSender.cs %}
```
and `SmtpRelay`:

```csharp
// Rockaway.WebApp/Services/Mail/SmtpRelay.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Services/Mail/SmtpRelay.cs %}
```
We also create a class to handle the SMTP relay configuration, `SmtpSettings`:

```csharp
// Rockaway.WebApp/Services/Mail/SmtpSettings.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Services/Mail/SmtpSettings.cs %}
```

Using configuration binding, we can map configuration settings directly to an instance of `SmtpSettings`: 

```csharp
// Add this to Program.cs

builder.Services.AddSingleton<IMailSender, SmtpMailSender>();
var smtpSettings = new SmtpSettings();
builder.Configuration.Bind("Smtp", smtpSettings);
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddSingleton<ISmtpRelay, SmtpRelay>();
```

By default, we'll connect to `localhost` on port 25 -- which, by a happy coincidence, is exactly what we need to test our emails using Papercut.

### Testing email on localhost with Papercut

Papercut SMTP ([https://github.com/ChangemakerStudios/Papercut-SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP)) is "a 2-in-1 email viewer and built-in SMTP server". It's a Windows application that runs in your system notification area: send email to localhost on port 25, and Papercut will capture it so you can inspect it.

![image-20240128020835816](/images/image-20240128020835816.png)

## Testing email with Mailtrap

Mailtrap ([https://mailtrap.io/](https://mailtrap.io/)) is a mail testing, sandbox, and delivery platform.

Sign up (they have a free tier, no credit card required - yay!), and once you've created your inbox, find the SMTP Settings. To configure the connection to the SMTP server, we can add a config section to `appsettings.Development.json`:

```json
{
	"Smtp": {
		"Host": "sandbox.smtp.mailtrap.io",
		"Port": 587,
		"Username": "YOUR_SMTP_USERNAME",
		"Password": "YOUR_SMTP_PASSWORD"
	}	
}
```

As well as previewing different email formats, Mailtrap offers spam analysis and HTML checks:

![image-20240128021725390](/images/image-20240128021725390.png)

## Configuring SMTP on Azure for production

Azure doesn't use appsettings.json; instead, we need to configure SMTP via the Azure dashboard configuration settings.

Azure configuration settings aren't based on JSON, so we can't nest settings. Instead, we need to separate configuration keys with a double underscore:

```
Smtp__Host		<your SMTP host>
Smtp__Port		587
Smtp__Username	<your SMTP username>
Smtp__Password	<your SMTP password>
```

