using Rockaway.WebApp.Data.Entities;

namespace Rockaway.WebApp.Data.Sample;

public static partial class SampleData {

	public static class TicketOrders {

		public static TicketOrder Order001 =
			Shows.Coda_Barracuda_20240517.CreateTestOrder("Ace Frehley", "ace@example.com");

		public static TicketOrder Order002 =
			Shows.Coda_NewCrossInn_20240520.CreateTestOrder("Brian Johnson", "brian@example.com");

		public static TicketOrder Order003 =
			Shows.Coda_PubAnchor_20240523.CreateTestOrder("Joey Tempest", "joey.tempest@example.com");

		public static IEnumerable<TicketOrder> AllTicketOrders = [Order001, Order002, Order003];

		public static IEnumerable<TicketOrderItem> AllTicketOrderItems
			=> AllTicketOrders.SelectMany(o => o.Contents);
	}

	public static TicketOrder CreateTestOrder(this Show show, string name, string email) {
		// To generate random-but-stable data, we use numbers based on
		// the modulo of various string properties.
		var quantities = show.TicketTypes.ToDictionary(tt => tt.Id, tt => 1 + tt.Name.Length % 5);
		var createdAt = show.Date.AtMidnight().InUtc().Minus(Duration.FromDays(42))
			.PlusHours(show.Venue.Name.Length)
			.PlusMinutes(show.HeadlineArtist.Name.Length)
			.PlusSeconds(show.Venue.Address.Length)
			.ToInstant();
		var o = show.CreateOrder(quantities, createdAt);
		o.CustomerEmail = email;
		o.CustomerName = name;
		o.CompletedAt = createdAt.Plus(Duration.FromMinutes(show.Venue.FullAddress.Length));
		o.Id = NextId;
		return o;
	}

	// We want real GUIDs, but we need them to be stable
	// otherwise we get noisy DB migrations, so we generate
	// a bunch of real GUIDs ahead of time and hardcode them.
	private static readonly string[] ticketOrderGuids = [
		"ac824d10-367f-494c-ad32-f221420c7c3c",
		"560ed55e-c635-4f0e-a433-a23ab6fa7bb6",
		"f584739d-2ec0-4de8-8de2-140333516b4f",
		"6d97121f-7e5b-4318-ac78-85c4b3ce920e",
		"c556bd74-2d2f-4dd5-b4e0-f88c95a41958",
		"0f7c7eab-a870-4654-8c07-36be6ac8866a",
		"1a1c46d8-5322-488f-910a-6a4e7a13704e",
		"0e617535-3aa6-4ef8-8705-82e941a5c2d8",
		"889a6277-8506-45e4-ac8c-f38db75ae886",
		"a4782161-4745-4b3f-92cc-ec8def894b4f",
		"b790f475-1781-4b6c-aaf5-00cc52d1f720",
		"742257aa-174c-490f-ae19-f9dd8cadf0f1",
		"015a0651-adca-4156-a4f1-7db85ee52920",
		"343b4199-b161-41a7-ad27-9e0a6786bf7e",
		"209b9419-29d2-4246-ad9a-350a94fc308f",
		"dde9b193-ee60-464e-a16d-a9dc3c4915ef",
		"d007ece8-9721-4ac5-b5f2-0b4c8cb14633",
		"0340290a-5b67-4c33-ae4b-fa007c77d6a6",
		"0fd7ce0d-74f2-44db-9796-e134c433170a",
		"e98e6206-f7d6-4aa9-9a81-1ce6b374904f",
		"ec743cea-6a67-43c4-b330-8dff12fbd1cd",
		"64a4cc01-89d2-4b2f-bd19-6d78fa5ad2a9",
		"8a67bd1b-aa53-46e6-bac6-a719256580b7",
		"571ca9f0-0e67-421f-87f4-1e108a2d5c84",
		"52e1659c-b46a-422b-99f9-c3b4ede9c160",
		"f4e30182-6d70-4282-bd7f-1a43760fc326",
		"b8cbf7f0-0378-4e67-880e-ca4711843cf9",
		"fbb57888-1bb2-4e5e-9a07-b69af08ade5a",
		"71a8ed05-079d-46f3-90ed-de962dbc2d30",
		"e87c2689-2e23-4947-89f8-45c86c74a06e",
		"31999597-75eb-4100-a84a-f229d64a7694",
		"8acbdd1d-dd30-4b11-83d7-9738086faec7",
		"b00afaba-d103-491c-8b70-9363f809879c",
		"89339c59-2550-470f-a10b-193c246163a7",
		"519a4e52-47c5-4919-bb19-47ba7ff2a058",
		"f39dff57-69b9-4d0b-9e69-05b6e2112e50",
		"03cdf083-ad2f-426c-82d3-de04e5662190",
		"3cc93ab0-811c-4e1e-8c04-8488525b195f",
		"00a54b58-da12-48aa-87f4-614f2bb56458",
		"082bc88b-b0be-4e8f-9f30-b456b307cac5",
	];
	private static int index;
	private static Guid NextId => Guid.Parse(ticketOrderGuids[index++]);

}