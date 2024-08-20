using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Models;

public class ShowViewData(Show show) {

	public LocalDate ShowDate { get; } = show.Date;

	public string VenueName { get; } = show.Venue.Name;

	public string VenueAddress { get; } = show.Venue.FullAddress;

	public string HeadlineArtistName { get; } = show.HeadlineArtist.Name;

	public string CountryCode { get; } = show.Venue.CountryCode;

	public List<string> SupportActs { get; } = show.SupportSlots
			.OrderBy(s => s.SlotNumber)
			.Select(s => s.Artist.Name).ToList();

	public List<TicketType> TicketTypes { get; } = show.TicketTypes;

	public Dictionary<string, string> RouteData { get; } = show.RouteData;
}