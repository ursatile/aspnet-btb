---
title: "7.1 Razor Components"
layout: module
nav_order: 10701
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll build an interactive TicketPicker using Razor Components - a way to build reusable, interactive controls and components without writing JavaScript."
previous: rockaway602
complete: rockaway701
examples: examples/701/Rockaway
---

Let's upgrade our ticket picker.

At the moment, it looks like this:

![image-20240128022308557](/images/image-20240128022308557.png)

Wouldn't it be cool if, instead of typing numbers in the boxes, you could use little plus/minus buttons to add and remove tickets? And get realtime feedback on the total price of your order - in the right currency, properly formatted?

What if you could do all that without writing any JavaScript?

Let's meet Razor Components: a way to combine HTML, Razor and C# code to create interactive components without writing any JavaScript.

Add a new folder `Rockaway.WebApp/Components`, and create:

`Rockaway.WebApp/Components/_Imports.razor`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Components/_Imports.razor %}
```

`Rockaway.WebApp/Components/App.razor`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Components/App.razor %}
```

`Rockaway.WebApp/Components/TicketPicker.razor`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Components/TicketPicker.razor %}
```

### Razor Client Support

Edit `Rockaway.WebApp/Pages/Shared/_Base.cshtml`.

Add this to the `<head>` section:

```html
<!-- Required for Razor Component support -->
<base href="~/"/>
```

and add the Blazor client script at the end, between `site.js` and `@await RenderSectionAsync()`:

```html
<script src="~/js/site.js" asp-append-version="true"></script>

<!-- Required for Razor Component support -->
<script src="_framework/blazor.web.js"></script>

@await RenderSectionAsync("Scripts", required: false)
```

### Razor Server Support

Add these sections to `Program.cs`:

Add a `using` directive:

```csharp
using Rockaway.WebApp.Components;
```

Before `var app = builder.Build();`, add:

```csharp
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
```

and finally, before `app.Run()`, add:

```csharp
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();
```

### Using Razor Components from Razor Views

Here's the updated code for `Rockaway.WebApp/Views/Tickets/Show.cshtml`:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Tickets/Show.cshtml %}
```

### CSS Isolation for Razor Components

One powerful feature of Razor Components is the ability to add **scoped CSS** - custom CSS which only targets elements within your components.

We're going to add some CSS rules to style our ticket picker.

Create `Rockaway.WebApp/Components/TicketPicker.razor.css`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Components/TicketPicker.razor.css %}
```

To add scoped CSS support to our app, add a line to the `<head>` of `_Base.cshtml`:

```html
<link rel="stylesheet" href="~/Rockaway.WebApp.styles.css" asp-append-version="true"/>
```

Scoped CSS adds a randomly generated HTML attribute to every element rendered by the component, and adds a corresponding CSS selector to the CSS rules rendered by the `<link />` tag:

![image-20240128031034068](/images/image-20240128031034068.png)

