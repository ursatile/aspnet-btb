---
title: "5.4 The Artist Gallery"
layout: module
nav_order: 10504
typora-root-url: ./
typora-copy-images-to: ./images
summary: "Let's add some photographs, and turn our artists page into something that might inspire customers to actually buy a ticket."
previous: rockaway503
complete: rockaway504
---

In this module, we're going to overhaul the design of the `/artists` page, and then add a new artist information page at `/artists/{slug}` that will show us details of a specific artist.

We can do this using Razor Pages:

* Modify the existing `/Pages/Artists.cshtml` page to include hyperlinks for each artist
* Add a new page at `/Pages/Artist.cshtml` that includes a route parameter for the artist `slug` property, used to look up the artist in the database.

### ArtistViewData

So far, we've been able to map our entities -- `Artist` and `Venue` -- directly onto our web pages and views. Now, we want to introduce some new elements, like photo URLs and CSS classes, which need to be available when we render the page but which really don't belong in our database.

To accomplish this, we'll introduce a new class, `ArtistViewData`, which is designed around the data which the **page** needs, not around our database structure or our business processes.

```csharp
// Rockaway.WebApp/Models/ArtistViewData.cs

{% include_relative examples/504/Rockaway/Rockaway.WebApp/Models/ArtistViewData.cs %}
```

> There's a method here which will take a width and a height and return a Cloudinary URL. Cloudinary ([cloudinary.com](https://cloudinary.com)) is a cloud image hosting service that provides all sorts of cool features, like dynamic image resizing and format conversion. I've uploaded free images from Unsplash to a Cloudinary bucket for use in this workshop, but they have a free tier if you want to upload your own images and play around with their settings.

### Display Templates

We're going to create a standard template that's used when we render an instance of `ArtistViewData` anywhere in a Razor page or view.

Create a new file at `/Pages/Shared/DisplayTemplates/ArtistViewData.cshtml`:

```html
{% include_relative
examples/504/Rockaway/Rockaway.WebApp/Pages/Shared/DisplayTemplates/ArtistViewData.cshtml %}
```

Now we'll modify `/Pages/Artists.cshtml` to use our display template:

```html
{% include_relative examples/504/Rockaway/Rockaway.WebApp/Pages/Artists.cshtml %}
```

We'll also create some new CSS rules to control the layout of our artist gallery.

* On smartphones, the gallery should render as a single column
* On small devices, it should be two columns
* Medium screens: three columns
* Large screens: four columns
* Very large screens: six columns

We could do this inline using the Bootstrap grid system:

```html
<article class="row">
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
	<section class="col-12 col-sm-6 col-md-4 col-lg-3 col-xxl-2"></section>
</article>
```

Bootstrap grid is "mobile first" -- i.e. you define your defaults for small devices, and then override for larger ones where necessary, so here each div will occupy 12 columns (full width) by default, 6 columns on "small" devices, and so on.

Instead, though, we'll do this using a SASS mixin:

```scss
@mixin flex-grid-column {
	@include make-col-ready();

	@include media-breakpoint-up(sm) {
		@include make-col(6);
	}

	@include media-breakpoint-up(md) {
		@include make-col(4);
	}

	@include media-breakpoint-up(lg) {
		@include make-col(3);
	}

	@include media-breakpoint-up(xxl) {
		@include make-col(2);
	}
}

.artist-gallery {
	@include make-row();

	section {
		@include flex-grid-column;
		margin-bottom: 16px;
	}
}
```

and then define a set of CSS rules for the `artist-card` class:

```scss
div.artist-card {
	border-radius: $border-radius;
	background-position: center center;
	background-size: cover;
	background-repeat: no-repeat;
	height: 240px;
	display: flex;
	flex-direction: column;
	justify-content: flex-end;

	&.long-name h4 {
		font-size: 1.2em;
	}

	h4 {
		background-color: rgba(0,0,0,0.8);
		padding: calc($container-padding-x / 2);
		margin: 0;
		border-radius: 0 0 $border-radius $border-radius;

		a {
			color: #fff;
			text-decoration: none;
		}
	}
}
```

### The Artist Detail Page

In the arist card, we added a hyperlink using this syntax:

```html
<a asp-page="/Artist" asp-route-slug="@Model.Slug">@Model.Name</a>
```

At the moment, that doesn't go anywhere - so let's create the page it will link to:

```html
{% include_relative examples/504/Rockaway/Rockaway.WebApp/Pages/Artist.cshtml %}
```

and the corresponding code-behind file:

```csharp
{% include_relative examples/504/Rockaway/Rockaway.WebApp/Pages/Artist.cshtml.cs %}
```





