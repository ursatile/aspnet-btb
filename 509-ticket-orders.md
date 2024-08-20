---
title: "5.9 Creating a TicketOrder"
layout: module
nav_order: 10509
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll create the classes and tables we'll use to store a customer's ticket selection in the database."
previous: rockaway508
complete: rockaway509
examples: examples/509
migration: AddTicketOrdersAndTicketOrderItems
---

In this module, we'll create the classes and tables we need to actually store the customer's ticket order in the database.

> A purchase of tickets is always a **TicketOrder**, never just plain **Order**. The word "order" already means too many things, and `SELECT OrderId, CustomerName FROM Order ORDER BY OrderId` is just asking for trouble.

We'll create two new classes. `TicketOrder` is where we'll store customer name, email, which show the order is for, and metadata like when the order was created and when it was completed:

```csharp
// Rockaway.WebApp/Data/Entities/TicketOrder.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Entities/TicketOrder.cs %}
```

Within an order, each ticket type / quantity combination is a `TicketOrderItem`:

```csharp
// Rockaway.WebApp/Data/Entities/TicketOrderItem.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Entities/TicketOrderItem.cs %}
```

We'll add a `List<TicketOrder>` property to `Show.cs`, along with a new method `CreateOrder`: we supply the quantities of each ticket type and the current time (using `NodaTime.Instant`), and get back the `TicketOrder` object:

```csharp
// Add this to Rockaway.WebApp/Data/Entities/Show.cs

public List<TicketOrder> TicketOrders { get; set; } = [];

public TicketOrder CreateOrder(Dictionary<Guid, int> contents, Instant now) {
    var order = new TicketOrder {
        Show = this,
        CreatedAt = now
    };
    foreach (var (id, quantity) in contents) {
        var ticketType = this.TicketTypes.FirstOrDefault(tt => tt.Id == id);
        if (ticketType == default) continue;
        order.UpdateQuantity(ticketType, quantity);
    }

    this.TicketOrders.Add(order);
    return order;
}
```

## DbContext and Sample Data for TicketOrders

We can set up a bunch of test ticket orders in our sample data fixtures:

```csharp
// Rockaway.WebApp/Data/Sample/SampleData.TicketOrders.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Sample/SampleData.TicketOrders.cs %}
```

along with helper methods in `SeedData`:

```csharp
	public static IEnumerable<object> For(IEnumerable<TicketOrder> ticketOrders)
		=> ticketOrders.Select(o => new {
			o.Id,
			o.CustomerName,
			o.CustomerEmail,
			o.CreatedAt,
			o.CompletedAt,
			ShowDate = o.Show.Date,
			ShowVenueId = o.Show.Venue.Id
		});

	public static IEnumerable<object> For(IEnumerable<TicketOrderItem> ticketOrderItems)
		=> ticketOrderItems.Select(item => new {
			TicketOrderId = item.TicketOrder.Id,
			TicketTypeId = item.TicketType.Id,
			item.Quantity
		});
```

Update `RockawayDbContext`:

1. Add a `public DbSet<TicketOrder> TicketOrders { get; set; } = default!;`

2. In `modelBuilder.Entity<Show>()`, add:
   ```csharp
   entity.HasMany(show => show.TicketOrders)
       .WithOne(to => to.Show).OnDelete(DeleteBehavior.Restrict);
   ```

3. Configure the composite key for `TicketOrderItem`. *(We're calling HasKey as a static method instead of an extension method because of [a bug in the .NET 8 release](https://github.com/dotnet/Scaffolding/issues/2623) of the `aspnet-codegenerator` tools that will probably be fixed soon.)*
   ```
   modelBuilder.Entity<TicketOrderItem>(entity => {
       // ReSharper disable once InvokeAsExtensionMethod
       EntityTypeBuilderExtensions.HasKey(entity,
           toi => toi.TicketOrder.Id,
           toi => toi.TicketType.Id
       );
   });
   ```

4. Add `HasData` calls for `TicketOrder` and `TicketOrderItem`:
   ```csharp
   modelBuilder.Entity<TicketOrder>()
       .HasData(SeedData.For(SampleData.TicketOrders.AllTicketOrders));
   modelBuilder.Entity<TicketOrderItem>()
       .HasData(SeedData.For(SampleData.TicketOrders.AllTicketOrderItems));
   ```

## Update TicketsController

Modify `Rockaway.WebApp/Controllers/TicketsController.cs`:

1. Add a constructor parameter `IClock clock`, so we can get the current instant to inject into orders when they're created

2. Update the `[HttpPost]` overload of the `Show` method:
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Show(string venue, LocalDate date, Dictionary<Guid, int> tickets) {
       var show = await FindShow(venue, date);
       if (show == default) return NotFound();
       var ticketOrder = show.CreateOrder(tickets, clock.GetCurrentInstant());
       db.TicketOrders.Add(ticketOrder);
       await db.SaveChangesAsync();
       return Ok($"Order {ticketOrder.Reference} created. We should probably capture some customer details next.");
   }
   ```

## Adding an admin controller and views

It's easier to test our code if we can see what's happening in the database, so we'll use the `aspnet-codegenerator` tool to create a `TicketOrdersController`:

```
dotnet aspnet-codegenerator controller -name TicketOrdersController -m TicketOrder -dc RockawayDbContext --relativeFolderPath Areas/Admin/Controllers --useDefaultLayout --referenceScriptLibraries

dotnet format
```

Then we need to move all the views. The `aspnet-codegenerator` tool is not particularly clever, and specifying `--relativeFoldePath` only appears to affect the controller, regardless of where we run the tool from, so move the folder `/Views/TicketOrders` into `/Areas/Admin/Views/`

We need to make a few changes to `Areas/Admin/Controllers/TicketOrdersController`:

1. Add the [Area("Admin")] attribute to the class

Once you add in the formatting improvements and fix the namespaces, there's not a whole lot of the original controller left; here's what the code looks like by the time we're done:

```csharp
// Rockaway.WebApp/Areas/Admin/Controllers/TicketOrdersController.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Areas/Admin/Controllers/TicketOrdersController.cs %}
```

## Test Coverage

`Rockaway.WebApp.Tests/TicketTests.cs` has examples of testing the new ticket order logic, both using the `WebApplicationFactory` and by instantiating a `TicketsController` directly using a test database and a fake clock.

```csharp
// Rockaway.WebApp.Tests/TicketTests.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp.Tests/TicketTests.cs %}
```

## Create the database migrations

Once we're done, tested, and working, don't forget to add a database migration:

```dotnetcli
dotnet ef migrations add CreateTicketOrders -- --environment=Staging
```







