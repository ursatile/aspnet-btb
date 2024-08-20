using NodaTime;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Tests.Data.Entities;

public class PriceFormatTests {
	[Theory]
	[InlineData(123.45, "en-GB", "£123.45")]
	[InlineData(123.45, "fr-FR", "123,45 €")]
	[InlineData(123.45, "el-GR", "123,45 €")]
	[InlineData(123.45, "da-DK", "123,45 kr.")]
	[InlineData(123.45, "nn-NO", "123,45 kr")]
	[InlineData(123.45, "sv-SE", "123,45 kr")]
	[InlineData(123.45, "en-US", "$123.45")]
	[InlineData(123.45, "en-AU", "$123.45")]

	// Edge cases for non-geographic cultures.
	[InlineData(123.45, "", "¤123.45")]
	[InlineData(123.45, "INVALID-CULTURE-NAME", "¤123.45")]
	[InlineData(123.45, "en-001", "¤123.45")]
	[InlineData(123.45, "ar-001", "123٫45 ¤")]
	public static void Venue_Formats_Price_Correctly(decimal price, string cultureName, string expected) {
		var v = new Venue { CultureName = cultureName };
		var show = v.BookShow(new(), LocalDate.MaxIsoValue).WithTicketType(Guid.NewGuid(), "test", price);
		var tt = show.TicketTypes.First();
		tt.FormattedPrice.ShouldBe(expected);
	}
}