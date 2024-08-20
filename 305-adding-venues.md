---
title: "3.5 Adding Venues"
layout: module
nav_order: 10305
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this exercise, you'll add the Venue entity to our model, scaffold the controllers, create sample data, and set up the database migrations for it."
previous: mwnet304
complete: mwnet305
migration: CreateVenues
---

## Exercise: Adding Venues

In this section, you'll add the `Venue` entity to our model.

The code for a Venue looks like this:

```
// Rockaway.WebApp/Data/Entities/Venue.cs

{% include_relative examples/305/Rockaway/Rockaway.WebApp/Data/Entities/Venue.cs %}
```

You'll find sample data venue here: **[SampleData.Venues.cs](examples/305/Rockaway/Rockaway.WebApp/Data/Sample/SampleData.Venues.cs)**

The command to scaffold the `VenuesController` is:

```dotnetcli
dotnet aspnet-codegenerator controller -name VenuesController -m Venue -dc RockawayDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
```

#### Checklist:

1. Add the `Venue` entity to your project
2. Add a `DbSet<Venue>` to the `RockawayDbContext`
3. Add an index for `Venue.Slug` with a unique constraint
4. Add the `SampleData.Venues.cs` to your `SampleData` namespace
5. Add the `HasData` to seed venues
6. Scaffold the VenuesController as described above
7. Verify that browsing to `/Venues/Index` displays a list of venues
8. Verify that you can add, edit, and delete venues using the various controller actions
9. Generate a database migration which adds venues to the database schema



