using Rockaway.WebApp.Data;

namespace Rockaway.WebApp.Tests.Data;

public class EntityTypeBuilderExtensionTests {
	public class Country {
		public string Code { get; set; } = String.Empty;
	}

	public class Address {
		public Country Country { get; set; } = new();
		public int Id { get; set; }
	}

	public class Customer {
		public string Number { get; set; } = String.Empty;
		public Address Address { get; set; } = new();
	}

	public class Invoice {
		public string? Number { get; set; }
		public Customer Customer { get; set; } = new();
	}

	[Fact]
	public void HasKey_Translates_Simple_Properties_To_Strings() {
		var result = EntityTypeBuilderExtensions.ColumnName<Invoice>(i => i.Number);
		result.ShouldBe("Number");
	}

	[Fact]
	public void HasKey_Translates_2_Properties_To_Strings() {
		var result = EntityTypeBuilderExtensions.ColumnName<Invoice>(i => i.Customer.Number);
		result.ShouldBe("CustomerNumber");
	}

	[Fact]
	public void HasKey_Translates_3_Properties_To_Strings() {
		var result = EntityTypeBuilderExtensions.ColumnName<Invoice>(i => i.Customer.Address.Id);
		result.ShouldBe("CustomerAddressId");
	}

	[Fact]
	public void HasKey_Translates_4_Properties_To_Strings() {
		var result = EntityTypeBuilderExtensions.ColumnName<Invoice>(i => i.Customer.Address.Country.Code);
		result.ShouldBe("CustomerAddressCountryCode");
	}

	[Fact]
	public void HasKey_Throws_For_Invalid_Expression() {

		Should.Throw<Exception>(() =>
			EntityTypeBuilderExtensions.ColumnName<Invoice>(i
				=> i.ToString()));

		Should.Throw<Exception>(() =>
			EntityTypeBuilderExtensions.ColumnName<Invoice>(i
				=> i.Customer.ToString()));

		Should.Throw<Exception>(() =>
			EntityTypeBuilderExtensions.ColumnName<Invoice>(i
				=> i.Customer.Address.ToString()));

		Should.Throw<Exception>(() =>
			EntityTypeBuilderExtensions.ColumnName<Invoice>(i
				=> i.Customer.Address.Country.ToString()));
	}
}