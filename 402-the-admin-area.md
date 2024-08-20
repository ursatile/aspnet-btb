---
title: "4.2 The Admin Area"
layout: module
nav_order: 10402
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll move our CRUD controllers into a dedicated area called Admin, and secure this area so it's only available to authenticated users."
previous: rockaway401
complete: rockaway402
---

## Areas in ASP.NET Core

> Areas are **an ASP.NET feature used to organize related functionality into a group as a separate namespace (for routing) and folder structure (for views)**. Using areas creates a hierarchy for the purpose of routing by adding another route parameter, area , to controller and action or a Razor Page page .
>
> [https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/areas](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/areas)

The controllers we've set up so far support Create, Read, Update, Delete operations - often known as "CRUD controllers".

We don't really want random internet strangers to be creating and deleting records in our database, so let's move these controllers into a dedicated area which we can restrict to authenticated users only.

We'll use the `dotnet` command line tools to create our new area. Inside your `Rockaway.WebApp` project folder, run:

```
dotnet aspnet-codegenerator area Admin
```

All this generator actually does is create a bunch of empty folders... but hey, saves us having to create them ourselves, right?

Next, we need to move our controllers and views into the new area:

```
/Controllers
	/ArtistsController.cs	--> /Areas/Admin/Controllers/ArtistsController.cs
	/VenuesController.cs	--> /Areas/Admin/Controllers/VenuesControllers.cs
/Views
	/Artists/* 				--> move everything to /Areas/Admin/Views/Artists
	/Venues/* 				--> move everything to /Areas/Admin/Views/Venues
```

*(If you do this in an IDE, it'll probably fix the namespaces for you; if not, you'll need to fix them yourself)*

Add the `[Area("admin")]` attribute to both of our controllers:

```csharp
[Area("admin")]
public class VenuesController : Controller {
  //...
}
```

Add a new route to `Program.cs` which uses our admin area, and use the `RequireAuthorization` method to restrict this area to users who are already signed in.

```csharp
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization();
```

> For this workshop, we're not restricting users - *any* authenticated user can access our admin area. For a production app, you'd need to either restrict the ability to create new users, or user a more restrictive policy so that only selected users could access admin features.

Let's move the standard layouts with all the Bootstrap & scaffolding stuff into their own area, so we can keep all the advantages of rapid development for admin area code, but have total control of our customer-facing app.

### Move the login partial

Move `_LoginPartial.cshtml` to `/Areas/Admin/Views/Shared`

Remove the reference to LoginPartial from `/Views/Shared/_Layout.cshtml`

While we're there, let's add an admin link to the page footer:

### Create the admin layout

`Rockaway.WebApp/Areas/Admin/Views/Shared/_AdminLayout.cshtml`

```html
{% include_relative examples/402/Rockaway/Rockaway.WebApp/Areas/Admin/Views/Shared/_AdminLayout.cshtml %}
```

### Set up _ViewStarts for the Admin and Identity areas

Create `/Areas/Admin/_ViewStart.cshtml` :

```
@{
	Layout = "_AdminLayout";
}
```

Create `/Areas/Identity/Pages/_ViewStart.cshtml` - you'll probably need to create the `Areas/Identity` and `Areas/Identity/Pages` folders.

```csharp
@{
    Layout = "/Areas/Admin/Views/Shared/_AdminLayout.cshtml";
}
```

> Because we're using the Microsoft Identity UI package, any requests to `/identity/` URLs will be handled by this package -- **unless the runtime finds one of our pages at the requested URL**. So by creating our own layout file, we override only this specific element of the identity UI.

### Create the admin home page:

`Areas/Admin/Pages/Index.cshtml`

```html
{% include_relative examples/402/Rockaway/Rockaway.WebApp/Areas/Admin/Pages/Index.cshtml %}
```

Lock down the home page (and any other Razor Pages under `/admin/`)

```csharp
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin","/"));
```

And we're done:

* We've moved our CRUD controllers into an area under `/admin`
* We've locked down this area so that controllers and pages inside this area are only accessible to authenticated users.
  * Note how we've had to authorise controllers and pages separately; the URLs look the same but we're dealing with two completely separate parts of the ASP.NET routing system so we need to authorise each of them separately.
* We've created a separate layout for our admin area and our frontend area

### Testing the Admin Area

Since we now have a secure area of our website, we'd like to test two specific scenarios:

1. Requests to `/admin` that aren't signed in will be redirected to the login page
2. Authenticated (signed in) requests to `/admin` succeed.

The first one uses patterns we've already seen, with one key difference - we need to specify that our test HTTP client should **not follow redirects** (otherwise we'll ask for `/admin` and get back the `200 OK` from the login page we're redirected to.)

```csharp
// Rockaway.WebApp.Tests/Areas/Admin/SecurityTests.cs

{% include_relative examples/402/Rockaway/Rockaway.WebApp.Tests/Areas/Admin/SecurityTests.cs %}
```

To test authenticated requests, we need something more complicated. We need to add a custom startup filter into our `WebApplicationFactory`, which will inject some fake middleware into our request pipeline that sets the `User.Identity` to a fake user, so that the application code then thinks we're signed in.

> Read more about custom startup filters:
>
> [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/startup](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/startup#extend-startup-with-startup-filters)

The key to this is the `FakeAuthenticationFilter` class, which contains the `FakeAuthenticationMiddleware`, the `FakeAuthenticationOptions` class (which we use to specify the email address our fake user should be signed in with), and some static helper methods we can use to add this to our pipeline:

```csharp
// Rockaway.WebApp.Tests/FakeAuthenticationFilter.cs

{% include_relative examples/402/Rockaway/Rockaway.WebApp.Tests/FakeAuthenticationFilter.cs %}
```

There's also a static class containing the `AddFakeAuthentication` extension method:

```csharp
// Rockaway.WebApp.Tests/FakeAuthenticationMiddlewareExtensions.cs

{% include_relative examples/402/Rockaway/Rockaway.WebApp.Tests/FakeAuthenticationMiddlewareExtensions.cs %}
```

Once we've added those to our project, we can write a test which uses them along with the `WebApplicationFactory` and AngleSharp. We're going to make a request to `/admin` and verify that the returned page includes the `<a id="manage">` element, with the `InnerHTML` set to `"Hello <emailAddress>!"`

```csharp
// Rockaway.WebApp.Tests/Areas/Admin/PageTests.cs

{% include_relative examples/402/Rockaway/Rockaway.WebApp.Tests/Areas/Admin/PageTests.cs %}
```

This pattern -- creating a class that implements `IStartupFilter` and using it to inject custom middleware -- can be useful for testing all kinds of scenarios where you need to bypass a "real" security system. What makes it particularly powerful is that it doesn't require any changes to your application code; there's no "back doors" deployed to production, no secret "test account" credentials, so the only place where you're compromising security for the sake of testability is in your test code itself.

## Reinstating /artists and /venues

Finally, we'll resurrect the old Razor Pages code we used earlier, to create the public-facing pages at `/artists` and `/venues`







