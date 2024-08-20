---
title: "5.8 Tickets and Prices"
layout: module
nav_order: 10508
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll add tickets and ticket prices to our domain model, and build a page where customers can select the tickets they want to order."
previous: rockaway507
complete: rockaway508
examples: examples/508
migration: AddTicketTypes
---

Let's add tickets to our ticketing system.

Actually, we're going to add **ticket types** to our system. A **ticket type** defines a type of ticket - what's it called, which show is it for, how much does it cost, and how many are available for sale:

```csharp
// Rockaway.WebApp/Data/Entities/TicketType.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Entities/TicketType.cs %}
```

We'll also plug a bit of code into the `Show` class to add ticket types:

```csharp
// Show.cs:

public List<TicketType> TicketTypes { get; set; } = [];

public Show WithTicketType(string name, decimal price, int? limit = null) {
    this.TicketTypes.Add(new(this, name, price, limit));
    return this;
}
```

We're also going to create a property called `RouteData`, which we'll use to build URLs for creating links and forms targeting the show page:

```csharp
// Show.cs:

public Dictionary<string, string> RouteData => new() {
    { "venue", this.Venue.Slug },
    { "date", this.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) }
};
```

and use this to update the tickets link in `/Pages/Artist.cshtml`:

```html
<a  asp-action="show"
	asp-controller="tickets"
	asp-all-route-data="@show.RouteData">tickets</a>
```

That'll create a link which includes the venue slug and show date, like: `/tickets/bataclan-paris/2024-05-18`

## Adding Ticket Types to DbContext and Sample Data

We can use the `Show.WithTicketType` method to add a bunch of tickets to our sample data:

```csharp
// Rockaway.WebApp/Data/Entities/Sample/SampleData.Shows.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Sample/SampleData.Shows.cs %}
```

We'll add seed data helpers for ticket types:

```csharp
// Rockaway.WebApp/Data/Entities/Sample/SeedData.cs

public static IEnumerable<object> For(IEnumerable<TicketType> ticketTypes)
    => ticketTypes.Select(ToSeedData);

static object ToSeedData(TicketType tt) => new {
    tt.Id,
    ShowVenueId = tt.Show.Venue.Id,
    ShowDate = tt.Show.Date,
    tt.Price,
    tt.Name
};
```

We'll also need to add mappings to `RockawayDbContext`.

Add this to the mapping for `Entity<Show>`:

```csharp
entity.HasMany(show => show.TicketTypes)
	.WithOne(tt => tt.Show).OnDelete(DeleteBehavior.Cascade);
```

Override the default column type for the ticket price:

```csharp
modelBuilder.Entity<TicketType>(entity => {
    entity.Property(tt => tt.Price).HasColumnType("money");
});
```

and the `HasData` call:

```
modelBuilder.Entity<TicketType>()
    .HasData(SeedData.For(SampleData.Shows.AllTicketTypes));
```

### Creating the tickets view

Now, let's add a page to list the available tickets for a specific show. We're going to do this using the model/view/controller pattern, but rather than using the scaffolding tools, we'll create our classes by hand.

Here's the code for `TicketsController.cs`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Controllers/TicketsController.cs %}
```

Notice that we've got two `Show` methods, one decorated with `[HttpGet]` and the other with `[HttpPost]`.

The controller uses **attribute routing**, so we need to add support for this in `Program.cs`:

```csharp
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Add support for attribute routing:
app.MapControllers();

app.Run();
```

We'll extend  `ShowViewData`, to include a `List<TicketType>`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Models/ShowViewData.cs %}
```

To format the ticket price in the venue's local currency, we add a method to `Venue.cs`:

```csharp
// Venue.cs

public CultureInfo Culture
    => CultureInfo.GetCultures(CultureTypes.SpecificCultures)
           .FirstOrDefault(ci => ci.Name == CultureName)
       ??
       CultureInfo.InvariantCulture;

public string FormatPrice(decimal price) => price.ToString("C", Culture);
```

and then add a `FormattedPrice` property to `TicketType`:

```csharp
public string FormattedPrice
    => this.Show.Venue.FormatPrice(this.Price);
```

Here's the associated view, `Show.cshtml`:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Tickets/Show.cshtml %}
```

We're using a self-targeting `<form>` -- because we haven't provided an `action` attribute, the form will post back to the current URL, which preserves the venue slug and date.

The actual ticket quantity selector uses this syntax:

```
	<input type="number" name="tickets[@tt.Id]" value="0" min="0" max="10" />
```

ASP.NET will automatically bind this to a `Dictionary<Guid, int>`, associating each ticket type ID with the requested quantity of tickets:

```csharp
// TicketsController.cs
[HttpPost]
public async Task<IActionResult> Show(string venue, LocalDate date, Dictionary<Guid, int> tickets) {
    var show = await FindShow(venue, date);
    if (show == default) return NotFound();
    //TODO: create orders, add to database, and redirect to checkout
    return Ok(tickets);
}
```



