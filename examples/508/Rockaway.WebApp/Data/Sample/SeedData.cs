using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static class SeedData {
	public static IEnumerable<object> For(IEnumerable<Artist> artists)
		=> artists.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<Venue> venues)
		=> venues.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<Show> shows)
		=> shows.Select(ToSeedData);

	public static IEnumerable<object> For(IEnumerable<SupportSlot> supportSlots)
		=> supportSlots.Select(ToSeedData);

	static object ToSeedData(Artist artist) => new {
		artist.Id,
		artist.Name,
		artist.Description,
		artist.Slug
	};

	static object ToSeedData(Venue venue) => new {
		venue.Id,
		venue.Name,
		venue.Slug,
		venue.Address,
		venue.City,
		venue.PostalCode,
		venue.CultureName,
		venue.Telephone,
		venue.WebsiteUrl
	};

	static object ToSeedData(Show show) => new {
		VenueId = show.Venue.Id,
		show.Date,
		HeadlineArtistId = show.HeadlineArtist.Id,
	};

	static object ToSeedData(SupportSlot slot) => new {
		ShowVenueId = slot.Show.Venue.Id,
		ShowDate = slot.Show.Date,
		slot.SlotNumber,
		ArtistId = slot.Artist.Id
	};

	public static IEnumerable<object> For(IEnumerable<TicketType> ticketTypes)
		=> ticketTypes.Select(ToSeedData);

	static object ToSeedData(TicketType tt) => new {
		tt.Id,
		ShowVenueId = tt.Show.Venue.Id,
		ShowDate = tt.Show.Date,
		tt.Price,
		tt.Name
	};
}