using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Rockaway.WebApp.Data.Entities;

public class Venue {

	public Venue() { }

	public Venue(Guid id, string name, string slug, string address, string city, string countryCode, string? postalCode, string? telephone,
		string? websiteUrl) {
		Id = id;
		Name = name;
		Slug = slug;
		Address = address;
		City = city;
		CountryCode = countryCode;
		PostalCode = postalCode;
		Telephone = telephone;
		WebsiteUrl = websiteUrl;
	}

	public Guid Id { get; set; }

	[MaxLength(100)]
	public string Name { get; set; } = String.Empty;

	[MaxLength(100)]
	[Unicode(false)]
	[RegularExpression("^[a-z0-9-]{2,100}$",
		ErrorMessage = "Slug must be 2-100 characters and can only contain a-z, 0-9 and the hyphen - character")]
	public string Slug { get; set; } = String.Empty;

	[MaxLength(500)]
	public string Address { get; set; } = String.Empty;

	public string City { get; set; } = String.Empty;

	[Unicode(false)]
	[MaxLength(2)]
	public string CountryCode { get; set; } = String.Empty;

	public string? PostalCode { get; set; }

	[Phone]
	public string? Telephone { get; set; }

	[Url]
	public string? WebsiteUrl { get; set; }
}