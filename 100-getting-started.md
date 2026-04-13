---
title: Getting Started
layout: module
nav_order: 10100
typora-root-url: ./
typora-copy-images-to: ./images
summary: Let's meet Rockaway, the sample app we'll be using to explore the wonderful world of advanced web app development using ASP.NET 10 and C#
complete: rockaway100
---
Let's get up and running with Rockaway.

You can find the source code to Rockaway in the GitHub repository for this workshop:

[https://github.com/ursatile/rockaway2026](https://github.com/ursatile/rockaway2026)

Clone the repo, or download the ZIP file from [examples/rockaway-100.zip](examples/rockaway-100.zip)

Open the project folder, `dotnet run`

You should get the Rockaway sample app.

## Introducing C# 14 Extension Members

The code in the starter project was written for .NET 8. It's been upgraded to the .NET 10 runtime, but hasn't been rewritten to take advantage of C# 14's new language features, so let's introduce some examples of this.

C# 14 introduces a new `extension` block syntax that replaces the static class + `this` parameter pattern. Instead of a static wrapper class, you declare an `extension(T receiver)` block and write members directly.

### `Hosting/HostEnvironmentExtensions.cs`

**Before:**

```csharp
public static class HostEnvironmentExtensions {
    private static readonly string[] sqliteEnvironments = ["UnitTest", Environments.Development];

    public static bool UseSqlite(this IHostEnvironment env)
        => sqliteEnvironments.Contains(env.EnvironmentName);
}
```

**After:**

```csharp
public static class HostEnvironmentExtensions {
    private static readonly string[] sqliteEnvironments = ["UnitTest", Environments.Development];

    extension(IHostEnvironment env) {
        public bool UseSqlite()
            => sqliteEnvironments.Contains(env.EnvironmentName);
    }
}
```

---

### `Data/NodaTimeConverters.cs`

**Before:**

```csharp
public static class NodaTimeConverters {
    public static ModelConfigurationBuilder AddNodaTimeConverters(this ModelConfigurationBuilder builder) {
        builder.Properties<Instant>().HaveConversion<InstantToDateTimeOffsetConverter>();
        builder.Properties<LocalDate>().HaveConversion<LocalDateConverter>();
        builder.Properties<LocalTime>().HaveConversion<LocalTimeConverter>();
        builder.Properties<LocalDateTime>().HaveConversion<LocalDateTimeConverter>();
        return builder;
    }

    private class InstantToDateTimeOffsetConverter()
        : ValueConverter<Instant, DateTimeOffset>(i => i.ToDateTimeOffset(),
            dto => Instant.FromDateTimeOffset(dto));
    // ... other converters
}
```

**After:**

```csharp
public static class NodaTimeConverters {
    extension(ModelConfigurationBuilder builder) {
        public ModelConfigurationBuilder AddNodaTimeConverters() {
            builder.Properties<Instant>().HaveConversion<InstantToDateTimeOffsetConverter>();
            builder.Properties<LocalDate>().HaveConversion<LocalDateConverter>();
            builder.Properties<LocalTime>().HaveConversion<LocalTimeConverter>();
            builder.Properties<LocalDateTime>().HaveConversion<LocalDateTimeConverter>();
            return builder;
        }
    }

    private class InstantToDateTimeOffsetConverter()
        : ValueConverter<Instant, DateTimeOffset>(i => i.ToDateTimeOffset(),
            dto => Instant.FromDateTimeOffset(dto));
    // ... other converters
}
```

---

### `Data/EntityTypeBuilderExtensions.cs`

**Before:**

```csharp
public static class EntityTypeBuilderExtensions {
    public static KeyBuilder<TEntity> HasKey<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        params Expression<Func<TEntity, object?>>[] keyExpressions
    ) where TEntity : class =>
        builder.HasKey(keyExpressions.Select(ColumnName).ToArray());

    public static string ColumnName<TEntity>(Expression<Func<TEntity, object?>> expr) =>
        expr.Body switch {
            MemberExpression mx => ColumnName(mx),
            UnaryExpression { Operand: MemberExpression mx } => ColumnName(mx),
            _ => throw new("Only member expressions are supported")
        };

    private static string ColumnName(Expression? expr) {
        if (expr is not MemberExpression mx) return String.Empty;
        return ColumnName(mx.Expression) + mx.Member.Name;
    }
}
```

**After:**

```csharp
public static class EntityTypeBuilderExtensions {
    extension<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class {
        public KeyBuilder<TEntity> HasKey(
            params Expression<Func<TEntity, object?>>[] keyExpressions
        ) => builder.HasKey(keyExpressions.Select(ColumnName).ToArray());
    }

    public static string ColumnName<TEntity>(Expression<Func<TEntity, object?>> expr) =>
        expr.Body switch {
            MemberExpression mx => ColumnName(mx),
            UnaryExpression { Operand: MemberExpression mx } => ColumnName(mx),
            _ => throw new("Only member expressions are supported")
        };

    private static string ColumnName(Expression? expr) {
        if (expr is not MemberExpression mx) return String.Empty;
        return ColumnName(mx.Expression) + mx.Member.Name;
    }
}
```

> `ColumnName` is not an extension method, so it stays as a regular static method outside the block.

---

### `Data/Sample/SampleData.Shows.cs`

**Before:**

```csharp
public static partial class SampleData {
    public static Show WithTicketType(this Show show, Guid id, string name, decimal price, int? limit = null) {
        show.TicketTypes.Add(new(id, show, name, price, limit));
        return show;
    }

    public static Show WithSupportActs(this Show show, params Artist[] artists) {
        show.SupportSlots.AddRange(artists.Select(artist => new SupportSlot() {
            Show = show,
            Artist = artist,
            SlotNumber = show.NextSupportSlotNumber
        }));
        return show;
    }
    // ...
}
```

**After:**

```csharp
public static partial class SampleData {
    extension(Show show) {
        public Show WithTicketType(Guid id, string name, decimal price, int? limit = null) {
            show.TicketTypes.Add(new(id, show, name, price, limit));
            return show;
        }

        public Show WithSupportActs(params Artist[] artists) {
            show.SupportSlots.AddRange(artists.Select(artist => new SupportSlot() {
                Show = show,
                Artist = artist,
                SlotNumber = show.NextSupportSlotNumber
            }));
            return show;
        }
    }
    // ...
}
```
