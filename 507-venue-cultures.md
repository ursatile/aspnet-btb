---
title: "5.7 Venue Cultures"
layout: module
nav_order: 10507
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we learn how to use .NET's built-in localisation support to format prices in local currency."
previous: mwnet506
complete: mwnet507
examples: examples/507/Rockaway
migration: ReplaceCountryCodeWithCultureName
---

In the next section, we're going to add tickets to our database. Tickets have prices, and that's where things get interesting.

To display prices correctly, we need to use the currency convention of the venue's region - for venues in the UK, prices are displayed as pounds, with a dot (.) as a decimal separator -- £25.00. In France, decimals are separated by a comma, with the currency symbol € after the amount: 25,00 €. 

### Goodbye Country, Hello CultureInfo

To work with currencies, time zones, and other localization concerns, we need to store the **culture** associated with the venue, not just the **country**.

The `CultureInfo` class in .NET is the basis of almost all localization - timezones, currency, date formatting. A `CultureInfo` is defined by the combination of a language and a region:

* `en-US` English, United States
* `en-GB` English, Great Britain
* `pt-BR` Portuguese, Brazil
* `pt-PT` Portuguese, Portugal
* `en-CA` English, Canada
* `fr-CA` French, Canada

The good news is: if we know the culture name, we can extract the country code, so we can replace the `CountryCode` property with `CultureName`, and then use a read-only property to get the country code back. This gives us what we need to be able to localize timezones and currencies, but won't break any of the work we did in the last module to render country flags on the venue list.

> To get a list of all the supported specific cultures in .NET, try this:
>
> ```csharp
> using System.Globalization;
> 
> var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).OrderBy(ci => ci.Name.Length);
> foreach (var ci in cultures) {
>     Console.Write("{0,-20}", ci.Name);
>     Console.Write("{0,-6}", ci.Name.Length);
>     Console.Write(" {0,-3}", ci.TwoLetterISOLanguageName);
>     Console.Write(" {0,-3}", ci.ThreeLetterISOLanguageName);
>     Console.Write(" {0,-3}", ci.ThreeLetterWindowsLanguageName);
>     Console.Write(" {0,-40}", ci.DisplayName);
>     Console.WriteLine(" {0,-40}", ci.EnglishName);
> }
> ```

The longest culture name in the .NET runtime is `ca-ES-VALENCIA` (14 characters), so we'll use 16 to store the culture name:

```csharp
// replace Venue.CountryCode with this:
[Unicode(false)]
[MaxLength(16)]
public string CultureName { get; set; } = String.Empty;

public string CountryCode => CultureName.Split("-").Last();
```

> Yes, this won't work for some unusual cultures like `en-001` (English - World) -- so when we update the UI that's used to edit venues, we'll filter out any cultures whose identifier doesn't contain a country code.

We'll also update the `Venue` constructor, and the `Venue.ToSeedData()` extension method, to refer to the culture name instead of the country code.

### Migrating data with EF Migrations

If we did this on a real system, we'd have a problem, because when we create and apply the migration, EF Core will drop the CountryCode column and create a new, empty CultureName column -- and so we'll lose data.

To avoid this, we could break this change into three steps:

1. Create and apply the migration to add the `CultureName` column.
2. Manually populate the column based on each venue's `CountryCode`
3. Create and apply the migration to drop the `CountryCode` column

Alternatively, we can create the migration and edit it before applying it, to add an intermediate step that'll populate the new `CultureName` column based on the existing `CountryCode` values.

First, create the migration:

```csharp
dotnet ef migrations add ReplaceCountryCodeWithCultureName -- --environment Staging
dotnet format
```

That'll create a migration file which includes these steps:

```csharp
migrationBuilder.DropColumn(
    name: "CountryCode",
    table: "Venue");

migrationBuilder.AddColumn<string>(
    name: "CultureName",
    table: "Venue",
    type: "varchar(16)",
    unicode: false,
    maxLength: 16,
    nullable: false,
    defaultValue: "");
```

We're going to add a block of mappings to the migration code:

```csharp
private readonly Dictionary<string, string> countryCodesToCultureNames = new() {
    { "GB", "en-GB" }, // English (Great Britain)
    { "FR", "fr-FR" }, // French (France)
    { "DE", "de-DE" }, // Germany (Germany)
    { "PT", "pt-PT" }, // Portuguese (Portugal)
    { "GR", "el-GR" }, // Greek (Greece)
    { "NO", "nn-NO" }, // Norwegian (Norway)
    { "SE", "sv-SE" }, // Swedish (Sweden)
    { "DK", "dk-DK" } // Danish (Denmark)
};
```

> Yep, you'll need to see what country codes are in use, and pick a culture -- and if you've got venues in Canada, you're gonna have to choose whether they're all English-speaking, or all French-speaking, or run the migration and then fix any that didn't fit the pattern. Real life is messy.

Then we'll replace the generated code with this:

```csharp
migrationBuilder.AddColumn<string>(
    name: "CultureName",
    table: "Venue",
    type: "varchar(16)",
    unicode: false,
    maxLength: 16,
    nullable: false,
    defaultValue: "");

foreach (var (countryCode, cultureName) in countryCodesToCultureNames) {
    var sql = $@"
        UPDATE Venue
        SET CultureName = '{cultureName}'
        WHERE CountryCode = '{countryCode}''";
    migrationBuilder.Sql(sql);
}

migrationBuilder.DropColumn(name: "CountryCode", table: "Venue");
```

and make the corresponding changes to the `Down()` method in case the migration needs to be reversed.

> I've never, in my career, actually used the `Down()` method to roll back a migration on a production database; on the occasions a release or a migration hasn't done quite the right thing, it's always made more sense to create a new migration that fixes the problem, and roll forwards. But your scenario might be different to mine.

Apply the migration, and then check our `Venue` page to make sure it worked -- if we see the right country flags, we're all good:

![image-20240122230803623](/images/image-20240122230803623.png)

