---
title: Introduction
layout: home
nav_order: 10000
typora-copy-images-to: ./images
summary: "Introduction - why .NET, why C#, choosing a .NET version. The scenario: selling tickets for rock concerts."
---

## Introduction

In this workshop, we're going to use .NET 8, C# and ASP.NET Core to build a web application for selling tickets to rock concerts.

I chose this scenario for several reasons. First, it's relatively simple to understand. Bands go on tour, they play shows in various different venues, maybe there's a support band or two on the same bill, but the business rules are fairly consistent from one show to the next.

Second, dealing with stage times and ticket prices at venues in different countries means we can't ignore real-world concerns like time zones and currency localization.

Third: I like rock bands.🤘🏼

![shutterstock_676097989_1080p](images/shutterstock_676097989_1080p.jpg)

## Choosing a .NET Version

In the beginning, there was just .NET -- I've been using .NET since 2002, when I went to a launch event at Microsoft in the UK and got a beta release of "Visual Studio .NET" on DVD.

In 2014, Microsoft announced .NET Core -- an open-source, cross-platform version of .NET -- and so the older, Windows-only version of .NET became known as .NET Framework.

.NET Core shipped versions 1.0, 2.0 and 3.1, alongside .NET Framework 4.6, 4.7 and 4.8.

Since the release of .NET 5 in 2020, there's been a release of .NET every November. Even-numbered versions are "long term support" releases, with support and updates guaranteed for at least three years. Odd-numbered versions are short term support, with updates for 18 months.

At the time of writing, the latest LTS release is .NET 8, which will be supported until the end of 2027.

### Upgrading projects to .NET 8

The good news is that .NET has phenomenally good backwards compatibility. If you've got code written for .NET 5, 6, or 7, upgrading it to .NET 8 is usually pretty straightforward:

1. Replace `net60` with `net80` in all your `.csproj` files. If you have a `global.json` file, you'll need to change the .NET version in there too.
2. Upgrade any NuGet package whose name starts `Microsoft.` to the latest versions.
   1. If you're using third party NuGet packages, you might need to upgrade those too, but watch out for breaking changes or new license versions.
3. `dotnet clean`
4. `dotnet build`
5. `dotnet run`

You can also upgrade your projects using the [.NET Upgrade Assistant](https://dotnet.microsoft.com/en-us/platform/upgrade-assistant).

## Client, Servers, Requests and Responses

The web is the most successful distributed system ever built. Billions of servers running dozens of different operating systems, languages, databases, from hobby servers running on low-cost hardware like Raspberry Pis to serverless functions running in the cloud, all forming part of a giant global network of interconnected documents and services.

Behind every single one of those sites and services is the same fundamental architecture: clients, servers, requests and responses.

## Fundamental Principles of Web Applications

### Servers don't do anything until they receive a request

Think of a web server like a bartender, and the client as a customer ordering drinks. When a customer orders a gin & tonic -- the *request* -- the bartender springs into action and makes them a gin & tonic -- the *response*.

What if there are no customers? Well, there's odd bits of housekeeping and maintenance to do, sure, but one of the key principles of web application architecture is that if there are no requests coming in, your server shouldn't be doing anything. 

And if there are too many customers? The bartender can't keep up; customers have to wait longer for their drinks. And some of them get fed up of waiting -- we say their *request timed out*. Maybe they order again - "hey, where's my gin & tonic?" -- or maybe they go to another bar.

Either way, timeouts are bad.

### Structure of an HTTP Request

![image-20230827122347473](D:\Projects\github\ursatile\mwnet\images\image-20230827122347473-1697208781995-1.png)

### Structure of an HTTP Response

![image-20230827122419914](D:\Projects\github\ursatile\mwnet\images\image-20230827122419914-1697208781996-2.png)

### Getting started with ASP.NET Core Web Apps

We're going to use ASP.NET Core to explore what we can do using HTTP requests and responses.

First, make sure you've got the right version of .NET installed. We're using .NET 8 for these examples, so run:

```
D:\Projects> dotnet --version
8.0.201
```

As long as it starts with a 8, you should be good to go.

