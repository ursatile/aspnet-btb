using Rockaway.WebApp.Data.Entities;
// ReSharper disable InconsistentNaming

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {

	public static Show WithSupportActs(this Show show, params Artist[] artists) {
		show.SupportSlots.AddRange(artists.Select(artist => new SupportSlot() {
			Show = show,
			Artist = artist,
			SlotNumber = show.NextSupportSlotNumber
		}));
		return show;
	}
	public static class Shows {
		public static readonly Show Coda_Barracuda_20260517 = Venues.Barracuda
			.BookShow(Artists.Coda, new(2026, 8, 17))
			.WithSupportActs(Artists.KillerBite, Artists.Overflow);

		public static readonly Show Coda_Columbia_20260518 = Venues.Columbia
			.BookShow(Artists.Coda, new(2026, 8, 18))
			.WithSupportActs(Artists.KillerBite, Artists.Overflow);

		public static readonly Show Coda_Bataclan_20260519 = Venues.Bataclan
			.BookShow(Artists.Coda, new(2026, 8, 19))
			.WithSupportActs(Artists.KillerBite, Artists.Overflow, Artists.JavasCrypt);


		public static readonly Show Coda_NewCrossInn_20260520 = Venues.NewCrossInn
			.BookShow(Artists.Coda, new(2026, 8, 20))
			.WithSupportActs(Artists.JavasCrypt);

		public static readonly Show Coda_JohnDee_20260522 = Venues.JohnDee
			.BookShow(Artists.Coda, new(2026, 8, 22))
			.WithSupportActs(Artists.JavasCrypt);

		public static readonly Show Coda_PubAnchor_20260523 = Venues.PubAnchor
			.BookShow(Artists.Coda, new(2026, 8, 23))
			.WithSupportActs(Artists.JavasCrypt);

		public static readonly Show Coda_Gagarin_20260525 =
			Venues.Gagarin.BookShow(Artists.Coda, new(2026, 8, 25))
			.WithSupportActs(Artists.JavasCrypt, Artists.SilverMountainStringBand);

		public static IEnumerable<Show> AllShows = [
			Coda_Barracuda_20260517,
			Coda_Columbia_20260518,
			Coda_Bataclan_20260519,
			Coda_NewCrossInn_20260520,
			Coda_JohnDee_20260522,
			Coda_PubAnchor_20260523,
			Coda_Gagarin_20260525
		];

		public static IEnumerable<SupportSlot> AllSupportSlots
			=> AllShows.SelectMany(show => show.SupportSlots);
	}
}