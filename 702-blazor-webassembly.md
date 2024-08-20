---
title: "7.2 Razor and Web Assembly"
layout: module
nav_order: 10702
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll turn our Razor Component into a standalone project which we can deploy using Blazor and web assembly to create interactive controls which don't rely on a network connection."
previous: rockaway701
complete: rockaway702
examples: examples/702/Rockaway
---

In the last module, we created an interactive `TicketPicker`, a Razor component that provides immediate feedback and a rich client experience when customers are selecting gig tickets.

It worked great, but the interactivity relied on making network calls to the server -- behind the scenes, whenever you click a button, ASP.NET is using SignalR to send a message to the server, run the code, get the modified HTML, and update the state of the controls on the page.

In this module, we'll see what we need to do to deploy our TicketPicker as a component that runs directly in the browser using web assembly (WASM).

## Razor Component Render Modes

A Razor Component ends up on your browser screen in one of five different ways, known as **[render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0)**:

**Static:** the control is rendered on the server. The client gets static HTML, and you're done -- no interactions, no behaviour.

```
@(await Html.RenderComponentAsync<MyComponent>(RenderMode.Static))
```

**Server:** the page sends a placeholder to the client; the client then sends a network request to the server, fetches the state of the component, and renders it into the placeholder. Clients get a "loading flicker" - they'll see an empty placeholder for a moment or two while the client fetches the initial state of the component.


```
@(await Html.RenderComponentAsync<MyComponent>(RenderMode.Server))
```

**ServerPrerendered:** the page sends rendered HTML to the client; the client then sends a network request to the server, fetches the state of the component, and replaces the rendered HTML.

```
@(await Html.RenderComponentAsync<MyComponent>(RenderMode.ServerPrerendered))
```

**WebAssembly:** The entire control is delivered as client-side web assembly code, and nothing appears until the browser has downloaded all the assets, initialised the Blazor web assembly runtime, and initialised the control.

```
@(await Html.RenderComponentAsync<MyComponent>(RenderMode.WebAssembly))
```

**WebAssemblyPrerendered**: yep, you guessed it. Static HTML to start with, that gets replaced by client-side WASM code as soon as it's available.

```csharp
@(await Html.RenderComponentAsync<MyComponent>(RenderMode.WebAssemblyPrerendered))
```

We're going to turn our `TicketPicker` into a standalone component that will support any rendering style (although **static** won't do anything useful).

## Creating a Razor Class Library project

To use the `WebAssembly` render modes, you need to ship **the entire project** to the client.

For projects which are 100% Blazor, this makes perfect sense -- running a Blazor app is like downloading native binaries; once you've downloaded it, everything you need is available locally so you don't need to keep making calls across the network.

If you want to create reusable components that run on web assembly but which you can host inside your Razor Pages or MVC projects, things get a little more complicated. We don't want to ship a huge binary containing the whole of `Rockaway.WebApp` to our end users, so we're going to create a standalone .NET class library which contains just our Razor Components.

> .NET includes a project template `razorclasslib` which is supposedly for creating Razor class libraries -- packages which can contain pages, views, Razor components, etc. At the time of writing (January 2024), the only way I could find to get a Razor class library project to expose WASM components to a normal web application was to hack the project files until it was basically a Blazor standalone project, so we're going to save ourselves some confusing hacking and use the `blazorwasm` template instead.

### Creating Rockaway.RazorComponents

First we'll create the new project and add it to our solution:

``` dotnetcli
dotnet new blazorwasm-empty -o Rockaway.RazorComponents
dotnet sln add Rockaway.RazorComponents
```

Move `TicketPicker.razor` and `TicketPicker.razor.css` into the root of the new `Rockaway.RazorComponents` project.

We have a **circular dependency** problem now: `Rockaway.WebApp` needs to reference `Rockaway.RazorComponents`, but we've now got code in `Rockaway.RazorComponents` that relies on `TicketTypeViewData`, which is part of `Rockaway.WebApp`. We have three choices:

1. Move `TicketTypeViewData` into the `RazorComponents` project. Not ideal, since then anything which wants to use `TicketTypeViewData` will need to reference our components library.
2. Modify `TicketPicker` so it doesn't use `TicketTypeViewData` -- maybe we pass in dictionaries instead of a strongly-typed view model.
3. Created a new shared project, move  `TicketTypeViewData` into this shared project, and then reference it from both `WebApp` and `RazorComponents`. *(If we do this, our shared project will be sent to WebAssembly clients along with our component library, so don't put anything confidential in it!)*

Let's modify our component so it doesn't have any dependencies on Rockaway.WebApp.

We'll create a `TicketPickerItem` class that's part of our component library:

```csharp
{% include_relative {{ page.examples }}/Rockaway.RazorComponents/TicketPickerItem.cs %}
```

and modify `TicketPicker.razor` to use this type instead our view data:

```csharp
{% include_relative {{ page.examples }}/Rockaway.RazorComponents/TicketPicker.razor %}
```

Create `Rockaway.RazorComponents/_Imports.razor`:

```
{% include_relative {{ page.examples }}/Rockaway.RazorComponents/_Imports.razor %}
```

Create `Rockaway.RazorComponents/Program.cs` -- we're not actually going to run it but the tooling won't build our Razor project unless it's there:

```csharp
{% include_relative {{ page.examples }}/Rockaway.RazorComponents/Program.cs %}
```

## Adding Web Assembly support to Web Apps

Add WebAssembly Server support to `Rockaway.WebApp`

```dotnetcli
dotnet add Rockaway.WebApp package Microsoft.AspNetCore.Components.WebAssembly.Server
```

Add a reference to the `Rockaway.RazorComponents` project:

```
dotnet add Rockaway.WebApp reference Rockaway.RazorComponents
```

Add web assembly support to `Program.cs`:

```
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();
```

```
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode();
```

Finally, update `Rockaway.WebApp/Views/Tickets/Show.cshtml` to use our new component:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Tickets/Show.cshtml %}
```







