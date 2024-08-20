---
title: "5.10 Customer Details"
layout: module
nav_order: 10510
typora-root-url: ./
typora-copy-images-to: ./images
summary: "In this module, we'll build the second part of the customer checkout process, confirming a customer's order and asking them to enter their name and email address."
previous: rockaway509
complete: rockaway510
examples: examples/510
---

In the last module, we created a simple ticket picker, and when it was submitted, added a `TicketOrder` to the database.

In this module, we'll capture the customer's name and email address, and ask them to accept our payment terms.

> Integrating with a real payment system is outside the scope of this workshop, so for now, we just ask the customer to promise that they'll pay on the door. Chill. It'll be fine.

We're going to create a new controller, `CheckoutController`.

`GET /checkout/confirm/{id}` will display the order contents and ask the customer to fill in their contact details.

`POST /checkout/confirm` will confirm their order (and, in the next module, send them an email)

```csharp
// Rockaway.WebApp/Controllers/CheckoutController.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Controllers/CheckoutController.cs %}
```

Update `TicketsController` so that after the order is saved to the database, we redirect to the new order page:

```csharp
[HttpPost]
public async Task<IActionResult> Show(string venue, LocalDate date, Dictionary<Guid, int> tickets) {
    var show = await FindShow(venue, date);
    if (show == default) return NotFound();
    var ticketOrder = show.CreateOrder(tickets, clock.GetCurrentInstant());
    db.TicketOrders.Add(ticketOrder);
    await db.SaveChangesAsync();
    return RedirectToAction("Confirm", "Checkout", new { id = ticketOrder.Id });
}
```

To show the order summary, and handle the form submission, create a new class. We'll use annotations from `System.ComponentModel` to control the format, display and validation of the data provided via the form:

```csharp
// Rockaway.WebApp/Models/OrderConfirmationPostData.cs

{% include_relative {{ page.examples}}/Rockaway.WebApp/Models/OrderConfirmationPostData.cs %}
```

## Validation and Non-Nullable Reference Types

`OrderConfirmationPostData` includes a `TicketOrderViewData` property, which is a reference type -- and in .NET 8, reference types can't be null unless you make them optional. Which means the built-in validation is going to misbehave, 'cos if `OrderConfirmationPostData.TicketOrder` null, the model state isn't valid.

To work around this, find the line in `Program.cs`:

 ```csharp
 builder.Services.AddControllersWithViews();
 ```

and replace it with:

```csharp
builder.Services.AddControllersWithViews(options => {
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
```

## Custom Validation with MustBeTrue

ASP.NET's validation support is pretty powerful, but one thing it can't do out of the box is require the user to check a checkbox, so to ensure the user accepts our payment terms before submitting the form, we can create a custom validation attribute.

```csharp
// Rockaway.WebApp/Models/MustBeTrueAttribute.cs

{% include_relative {{ page.examples }}/Rockaway.WebApp/Models/MustBeTrueAttribute.cs %}
```

To provide client-side validation, we need to provide the snippet of JavaScript that extends the jQuery validation code to support our custom validation.

Add this to the end of `Rockaway.WebApp/Views/Shared/_ValidationScriptsPartial.cshtml`:

```html
<script>
	jQuery.validator.addMethod('must-be-true', (_, element) => element.checked);
	jQuery.validator.unobtrusive.adapters.addBool('must-be-true');
</script>
```

## Partial Views

We'll use **partial views** to break our confirmation page into manageable sections.

Create `Rockaway.WebApp/Views/Shared/_TicketOrderSummary.cshtml`:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Shared/_TicketOrderSummary.cshtml %}
```

Create `Rockaway.WebApp/Views/Shared/_CustomerDetailsForm.cshtml`:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Shared/_CustomerDetailsForm.cshtml %}
```

The actual confirmation page is `Rockaway.WebApp/Views/Checkout/Confirm.cshtml`:

```html
{% include_relative {{ page.examples }}/Rockaway.WebApp/Views/Checkout/Confirm.cshtml %}
```

