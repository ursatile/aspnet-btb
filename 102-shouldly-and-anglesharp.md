---
title: Shouldly and AngleSharp
layout: module
nav_order: 10102
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module we cover some more advanced test patterns you can use with the WebApplicationFactory, and two of my favourite NuGet packages - Shouldly and AngleSharp."
previous: 101
complete: 102
---

Let's install some more NuGet packages. **Shouldly** is a library for doing fluent assertions with .NET, and **AngleSharp** gives us the ability to query HTML documents from C#.

```
dotnet add Rockaway.WebApp.Tests package Shouldly
dotnet add Rockaway.WebApp.Tests package AngleSharp
```

#### Scenario

We need to modify the `<title>` element of our pages:

* Any page which does not set `ViewData["Title"]` should have the title **Rockaway**
* Individual pages should be able to override this by setting `ViewData["Title"]`
* If a page overrides the title, the layout page must use the exact title specified by the page - so if a page sets `ViewData["Title"] = "Tour Dates"`, the output should be `<title>Tour Dates</title>`

We'll create a test which verifies that our homepage has the proper `<title>` element. We'll use the `WebApplicationFactory` to retrieve the page's HTML, and then use `AngleSharp` to extract the right element and `Shouldly` to assert that it has the right value. We can also use xUnit's `[InlineData]` attribute to apply the same test to multiple pages. Say we want to confirm that multiple pages on our site have the correct `<title>` tags.

```csharp
{% include_relative examples/102/Rockaway/Rockaway.WebApp.Tests/Pages/PageTests.cs %}
```

## Exercises

#### Add a "Contact Us" page

1. Create a "Contact Us" page as part of the Rockaway web application.
2. Create a test that verifies that the Contact Us page URL returns a successful response
3. Create a test that verifies that the Contact Us page has the correct page title **Contact Us**

#### Extra Credit

How would you verify that the Contact Us page includes the correct email address and telephone number?



