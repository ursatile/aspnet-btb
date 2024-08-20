To pass Unicode strings without encoding:

```csharp
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
```



The code for those files is below:

**Rockaway.WebApp/Areas/Admin/Views/Admin/Index.cshtml**

```html
{% include_relative {{ page.examples }}Rockaway.WebApp/Areas/Admin/Views/Admin/Index.cshtml %}
```

**Rockaway.WebApp/Areas/Admin/Views/Admin/Artist.cshtml:**

```csharp
{% include_relative {{ page.examples }}Rockaway.WebApp/Areas/Admin/Views/Admin/Artist.cshtml %}
```

**Rockaway.WebApp/Areas/Admin/Views/Admin/Artists.cshtml:**

```csharp
{% include_relative {{ page.examples }}Rockaway.WebApp/Areas/Admin/Views/Admin/Artists.cshtml %}
```

**Rockaway.WebApp/Areas/Admin/Controllers/AdminController.cs:**

```csharp
{% include_relative {{ page.examples }}Rockaway.WebApp/Areas/Admin/Controllers/AdminController.cs %}
```

> You'll see code samples online which use a pattern like this:
>
> ```csharp
> var found = db.Artists.Find(id);
> var entry = db.Entry(found);
> found.CurrentValues.SetValues(post);
> db.SaveChanges();
> ```



## A Note about Key Generation Strategies

We're using a GUID as the primary key on our `Artist` entity, which means when we add a new artist to the database, we need to get a GUID value from *somewhere*.

By default, EF Core will generate a new GUID value in code -- this happens when we call `db.Artists.Add(artist)` -- and then persist this to the database when inserting a new record. This, mostly, works. If for any reason we don't want to use the default behaviour, we have two choices.

* Generate it in our application code. This means it's available as soon as we've generated it, and that we can rely on IDs which were generated but haven't been sent to the database yet. The drawback of generating our own GUIDs is that if we're inserting a *lot* of records, it can cause database performance problems. Look up index fragmentation if you want to know more.
* Generate it in our database. SQL Server has a function called `NEWSEQUENTIALID` which generates GUIDs that avoid the index fragmentation problem -- but this means we don't know what our ID is until we've done a round trip to the database.









































