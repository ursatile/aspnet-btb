---
title: "5.6 Shows and Tour Dates"
layout: module
nav_order: 10506
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll see how to add shows to our application data model."
previous: rockaway505
complete: rockaway506
examples: examples/506/Rockaway
migration: AddShowsAndSupportSlots
---

So far, our application's data model is pretty simple - artists and venues, and a bunch of string properties describing each one.

Here's what we're going to add in the next few modules.

1. Tour dates. A way to advertise the fact that a particular band will be performing at a certain venue, on a certain date.
2. Support slots. Which other artists will be appearing at the show, besides the headline act?
3. Tickets. What kinds of ticket are available for each show, how much do they cost, and how many of that type of ticket are available?
4. Checkout process. A customer can add tickets to an order and submit the order.
5. Email: when a customer submits an order, we'll send them an email confirming it.

### Shows

A show is defined by an **artist** performing at a **venue** on a specific **date**. The featured artist is known as the **headliner**.

* The same venue cannot host two different shows on the same date, so we can use the combination of `(venue, date)` as a unique identifier for a show. This is known as a **composite key**.
* A show might feature additional artists (known as **support acts**). Each support act is assigned a numbered **support slot**. By convention, these are in reverse running order: if there are three support acts, the band who are "fourth on the bill" will play first, then the band who are in slot #3, then the band in slot #2, and then the headliner.

The data schema for shows looks like this:

![data schema diagram showing Venue, Show, Artist and SupportSlot](/images/image-20240122212658197.png)

### Dates, Times and DateTimes, Oh My!

If a show's on March 25th, in Los Angeles, and the doors open at 7pm, it's already March 26th for most of the rest of the world. So what date is that concert actually happening?

The correct answer is, of course, "March 25th, don't be an idiot." (see also [relevant XKCD](https://xkcd.com/2867/))

![XKCD DateTime](/images/506-shows-and-tour-dates/datetime.png)

But timezones are tricky.

One common strategy for dealing with this kind of scenario is to insist that all date/time data is stored in the database as UTC, and the convert it to/from local time whenever you're displaying it. This can work well, but it does introduce an element of risk -- if somebody forgets to implement the conversion when adding a new UI feature, we could end up with shows on the wrong day.

Since all our events take place at a venue, and a venue is a physical location that doesn't move, what we really want is a **local date**; a way of saying "look, this event has a location, and it happens when the date **in that location** is March 25th".

.NET's DateTime classes have never provided great support for this kind of scenario: a .NET `DateTime` has a `Kind` property, which can be `Local`, `Utc` or `Unspecified`, but it's still way too easy to get the conversions wrong.

Instead, we're going to install a library called **NodaTime**, created by Jon Skeet and originally inspired by the JodaTime libraries for Java.

> Noda Time is an alternative date and time API for .NET. It helps you to think about your data more clearly, and express operations on that data more precisely.
>
> - [Project web site](http://nodatime.org/) - for documentation, installation, downloads etc
> - [Group/mailing list](https://groups.google.com/group/noda-time) - for discussion of potential features
> - [Project source and issue site](https://github.com/nodatime/nodatime)
> - [Stack Overflow tag](http://stackoverflow.com/questions/tagged/nodatime) - for specific "How do I do X?" questions

```
dotnet add package NodaTime
```

NodaTime gives us access to a whole bunch of new classes, including one called `LocalDate`, which works perfectly for our scenario.

Here's the code for a `Show`, including the `LocalDate` storing the show date:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Entities/Show.cs %}
```

We're also going to add classes for `SupportSlot`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Entities/SupportSlot.cs %}
```

We'll also add a new property to `Artist`:

```csharp
public List<Show> HeadlineShows { get; set; } = [];
```

and to `Venue`, along with a utility method that'll help us set up sample data:

```csharp
public List<Show> Shows { get; set; } = [];

public Show BookShow(Artist artist, LocalDate date) {
    var show = new Show {
        Venue = this,
        HeadlineArtist = artist,
        Date = date,
    };
    Shows.Add(show);
    return show;
}
```

### Mapping Composite Keys with EF Core

Some of our entities - `Artist`, `Venue` -- use a GUID as a key. This is a completely meaningless identifier, sometimes known as a **synthetic key** -- all that matters is that it's unique.

> If you've read on the internet that GUIDs make bad keys because they're slow to insert: yes, that's technically correct. Sometimes.
>
> If you're working on the kind of applications where you're inserting so many records so fast that GUIDs vs integers actually makes a difference, you should be benchmarking different key strategies on your own platform to identify the solution that works for you.
>
> If you're not? Chill. GUIDs are just fine. Relax.
>
> *Remember the first rule of software architecture: **it depends**.*

Shows don't use a GUID id. A show is identified by a combination of the **date** and the **venue** - known as a **composite key**.

In database terms, this means every show is related to a venue, identified by a venue ID, and every show has a date, and that combination of `(venueId, date)` forms a unique key. EF Core can handle composite keys just fine, but out of the box there's a limitation I don't like.

For simple keys, we can say:

```csharp
// define key using an expression
modelBuilder.Entity<Customer>().HasKey(customer => customer.AccountNumber));
```

OR we can provide a string column name:

```csharp
// define key using a string column name
modelBuilder.Entity<Customer>().HasKey("AccountNumber"));
```

Expressions are a much nicer way to refer to other parts of our code, because they're strongly typed - if we make a mistake, our code won't build. If we make a typo in a string property, the code will build just fine and then fail when we run it.

For composite keys, EF Core allows you to define your key using expressions:

```
modelBuilder.Entity<Show>().HasKey(entity => { entity.Property1, entity.Property2 });
```

but this only works for simple properties.

In our model, a `SupportSlot` is identified by a unique combination of `(Show, SlotNumber)`, but the `Show` in turn is identified by `(Venue,Date)` -- so in the database, the primary key for the `SupportSlot` table will be `(ShowVenueId, ShowDate, SlotNumber)`, and EF Core doesn't have expression-based support for declaring `SupportSlot.Show.Venue.Id` as a key property.

We could do this using strings:

```csharp
modelBuilder.Entity<SupportSlot>().HasKey("ShowVenueId", "ShowDate", "SlotNumber");
```

However, using C# extension methods, we can extend EF Core to support expressions for defining composite keys.

This is some pretty gnarly code == if you've not worked with `Expression` types in .NET before, it may take a moment to figure out what it's doing.

```csharp
// Rockaway.WebApp/Data/EntityTypeBuilderExtensions.cs

{% include_relative {{ page.examples}}/Rockaway.WebApp/Data/EntityTypeBuilderExtensions.cs %}
```

### Extending EF Core with Type Converters

We also have some types in our model now which EF Core has never seen before, specifically the NodaTime `LocalDate` used for the show date property.

To use these types with EF Core, we need to provide **type converters**, which can translate our custom types into types which EF Core knows about.

There's a NuGet package [NodaTime.EntityFrameworkCore.Conversions](https://www.nuget.org/packages/NodaTime.EntityFrameworkCore.Conversions), which provides type converters for NodaTime, but it's simple enough to implement them directly:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/NodaTimeConverters.cs %}
```

### Sample Data for Complex Types

Another limitation of EF Core is that the `HasData()` method expects a simple flat object with property names matching the column names on the target table. This worked fine for `Artist` and `Venue`, because we weren't relying on any complex navigation properties, but if we pass a `SupportSlot` to `HasData`, it's gonna try to find `SupportSlot.ShowVenueId`-- and when it can't, it'll throw an exception.

First, we're going to add ticket types and shows to the sample venue data:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Sample/SampleData.Shows.cs %}
```

So we need to map our complex model types into flat "seed data" objects to be able to use them with `HasData`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/Sample/SeedData.cs %}
```

Now we can update `RockawayDbContext`:

1. Use `override ConfigureConventions` to add our NodaTime type converters
2. Add property mappings for `Venue.Shows` and `Artist.HeadlineShows`
3. Add entity mappings for `Show`, `SupportSlot` and `TicketType`
4. Update the `HasData` calls to use `SeedData.For()`

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Data/RockawayDbContext.cs %}
```

Create `Models/ShowViewData.cs`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Models/ShowViewData.cs %}
```

and update `ArtistViewData` to include show dates:

```csharp
public IEnumerable<ShowViewData> Shows => artist.HeadlineShows
	.OrderBy(show => show.Date).Select(show => new ShowViewData(show));
```

Update the codebehind in `Artist.cshtml.cs`:

```csharp
{% include_relative {{ page.examples }}/Rockaway.WebApp/Pages/Artist.cshtml.cs %}
```

and the Razor page code:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Pages/Artist.cshtml %}
```

### ...and don't forget the SQL migration

```dotnetcli
dotnet ef migrations add ShowsAndSupportSlots -- --environment Staging
dotnet format
dotnet ef database update -- --environment Staging
```











