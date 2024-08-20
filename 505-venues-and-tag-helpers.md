---
title: "5.5 Venues, Countries, and Tag Helpers"
layout: module
nav_order: 10505
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we learn about ASP.NET TagHelpers and use them to enhance our venue listing page by drawing country flags for each venue."
previous: mwnet504
complete: mwnet505
examples: "examples/505/Rockaway"
---

## Introducing ASP.NET TagHelpers

ASP.NET provides all kinds of ways to create reusable components, from partial views to display templates. In this module, we're going to meet **tag helpers**, and use them to render country flags on our list of concert venues.

In my experience, tag helpers work best for rendering one specific piece of data, like using an icon to show whether a product is available, low stock, or sold out. Templates and partials work better for rendering more complex data, like a customer record, order contents.

We're going to create a TagHelper that will take a 2-digit ISO3166 country code (`GB`, `FR`, `DK`, `NO`) and render it as a small image of the associated country's flag, with `title` and `alt` attributes explaining which country it is.

### Create the Country class

Countries are an interesting outlier when it comes to managing business data. I've worked on multiple systems which had a `Country` table in their database, usually with one of the ISO country codes used as a primary key -- and quite a few where the client insisted we build a screen to add, remove, and edit countries.

Don't build this. If you let your customers add countries, you'll end up with a mess. I've seen apps that listed England, Great Britain *and* the United Kingdom as separate countries in the same drop-down list -- and you really don't want to get into arguments with your customers about whether Kosovo should be included in your list of countries. No, we'll outsource that to the International Standards Organisation, and use their public standard ISO3166 list as our list of countries.

The fun thing about this is, since it's a public standard, we can do something a little unusual. The country list doesn't change very often -- rarely enough that a new country easily justifies a new release of our software -- and so we can bake that standard directly into our code rather than looking it up in our database every time.

I've used C#'s `partial` keyword to split the Country class across two files. `Country.cs` contains the "logic" for working with countries:

```csharp
{% include_relative {{page.examples}}/Rockaway.WebApp/Data/Country.cs %}
```

and `Country.Iso3166.cs` contains the list of countries:

* [Rockaway.WebApp/Data/Country.Iso3166.cs]({{page.examples}}/Rockaway.WebApp/Data/Country.Iso3166.cs)

### Download the flag images

There are various sets of free flag images and icons available online (and country flags are in the public domain, so copyright isn't a problem).

We're going to use a set of flag images from [flagpedia.net](https://flagpedia.net/download/icons) - specifically, the 24x18 set of "mini waving icon" flags.

#### Image density and high-DPI displays

Modern smartphones and 4K displays use something called "pixel density". A 27" 1920x1080 HD monitor has about 80 pixels per inch (PPI) - so if we make an image 40 pixels wide, that's half of an inch, or about 12 millimetres. About as big as your thumbnail.

The iPhone Pro has a display density of 460 pixels per inch, so if it used a true 1:1 pixel mapping, our 40 pixel image would be 1/12 of an inch wide - about 2mm. That's stupidly tiny - and so devices like iPhone don't use true pixels; they use virtual pixels: a single virtual pixel is actually a grid of 3x3 or 4x4 physical pixels.

We can take advantage of this in our web pages by providing multiple versions of each image, so that devices with high-density displays use a higher-resolution version of the same image: it'll render at the same size, but with more detail.

1x density: ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/1x/ke.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/1x/gb.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/1x/pt.png)

2x density: ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/2x/ke.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/2x/gb.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/2x/pt.png)

3x density: ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/3x/ke.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/3x/gb.png) ![](examples/505/Rockaway/Rockaway.WebApp/wwwroot/img/flags/3x/pt.png)

Flagpedia offers downloads of each image set at 1x, 2x and 3x density, so we'll grab all three versions.

Unzip:

* [https://flagcdn.com/20x15.zip](https://flagcdn.com/20x15.zip) => `Rockaway.WebApp/wwwroot/img/flags/1x/` 
* [https://flagcdn.com/40x30.zip](https://flagcdn.com/40x30.zip) => `Rockaway.WebApp/wwwroot/img/flags/2x/`
* [https://flagcdn.com/60x45.zip](https://flagcdn.com/60x45.zip) => `Rockaway.WebApp/wwwroot/img/flags/3x/`

We'll also add an `_unknown.png` for unrecognised countries: ![unknown]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/1x/unknown.png) ![unknown]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/2x/unknown.png) ![unknown]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/3x/unknown.png)

* [1x/unknown.png]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/1x/unknown.png)
* [2x/unknown.png]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/2x/unknown.png)
* [3x/unknown.png]({{ page.examples}}/Rockaway.WebApp/wwwroot/img/flags/3x/unknown.png)

### The Tag Helper

The actual tag helper code looks like this:

```csharp
{% include_relative {{page.examples}}/Rockaway.WebApp/TagHelpers/CountryFlagTagHelper.cs %}
```

To use it in our pages, we need to register it in `_ViewImports.cshtml`:

```html
{% include_relative {{page.examples}}/Rockaway.WebApp/_ViewImports.cshtml %}
```

> Note that you need to specify the **assembly** here, not the **namespace**. If you get this wrong -- or if you mistype the assembly name, or the assembly isn't loaded -- you won't get any kind of error message or useful diagnostic information; it just won't work, and you'll have no idea why.

### Testing the Tag Helpers

We'll test our tag helper by plugging a chunk of code into our `/Elements` page:

```html
<section>
	<h2>Country Tag Helper</h2>
	@foreach (var country in Country.Iso3166List) {
		<country-flag iso-code="@country.Code" />
	}
	<country-flag iso-code="??" />
</section>
```

### Updating the Venues Page

Finally, we can use our new tag helper to update the layout of the `/venues` page:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Pages/Venues.cshtml %}
```

Notice that because the flag is drawn by a tag helper, we can add more HTML attributes when we actually use it - just like any other HTML tag.
