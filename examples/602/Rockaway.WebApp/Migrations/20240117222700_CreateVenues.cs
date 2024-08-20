using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rockaway.WebApp.Migrations {
	/// <inheritdoc />
	public partial class CreateVenues : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "Venue",
				columns: table => new {
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Slug = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
					Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					City = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CountryCode = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
					PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
					WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Venue", x => x.Id);
				});

			migrationBuilder.InsertData(
				table: "Venue",
				columns: new[] { "Id", "Address", "City", "CountryCode", "Name", "PostalCode", "Slug", "Telephone", "WebsiteUrl" },
				values: new object[,]
				{
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), "Town Hall Parade", "London", "GB", "Electric Brixton", "SW2 1RJ", "electric-brixton", "020 7274 2290", "https://www.electricbrixton.uk.com/" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), "50 Boulevard Voltaire", "Paris", "FR", "Bataclan", "75011", "bataclan-paris", "+33 1 43 14 00 30", "https://www.bataclan.fr/" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), "Columbiadamm 9 - 11", "Berlin", "DE", "Columbia Theatre", "10965", "columbia-berlin", "+49 30 69817584", "https://columbia-theater.de/" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), "Liosion 205", "Athens", "GR", "Gagarin 205", "104 45", "gagarin-athens", "+45 35 35 50 69", "" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), "Torggata 16", "Oslo", "NO", "John Dee Live Club & Pub", "0181", "john-dee-oslo", "+47 22 20 32 32", "https://www.rockefeller.no/" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), "Stengade 18", "Copenhagen", "DK", "Stengade", "2200", "stengade-copenhagen", "+45 35355069", "https://www.stengade.dk" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7"), "R da Madeira 186", "Porto", "PT", "Barracuda", "4000-433", "barracuda-porto", null, null },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8"), "Sveavägen 90", "Stockholm", "SE", "Pub Anchor", "113 59", "pub-anchor-stockholm", "+46 8 15 20 00", "https://www.instagram.com/pubanchor/?hl=en" },
					{ new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9"), "323 New Cross Road", "London", "GB", "New Cross Inn", "SE14 6AS", "new-cross-inn-london", "+44 20 8469 4382", "https://www.newcrossinn.com/" }
				});

			migrationBuilder.CreateIndex(
				name: "IX_Venue_Slug",
				table: "Venue",
				column: "Slug",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "Venue");
		}
	}
}