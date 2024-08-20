using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rockaway.WebApp.Migrations {
	/// <inheritdoc />
	public partial class ShowsAndSupportSlots : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "Show",
				columns: table => new {
					Date = table.Column<DateOnly>(type: "date", nullable: false),
					VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					HeadlineArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Show", x => new { x.VenueId, x.Date });
					table.ForeignKey(
						name: "FK_Show_Artist_HeadlineArtistId",
						column: x => x.HeadlineArtistId,
						principalTable: "Artist",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Show_Venue_VenueId",
						column: x => x.VenueId,
						principalTable: "Venue",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "SupportSlot",
				columns: table => new {
					SlotNumber = table.Column<int>(type: "int", nullable: false),
					ShowVenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ShowDate = table.Column<DateOnly>(type: "date", nullable: false),
					ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_SupportSlot", x => new { x.ShowVenueId, x.ShowDate, x.SlotNumber });
					table.ForeignKey(
						name: "FK_SupportSlot_Artist_ArtistId",
						column: x => x.ArtistId,
						principalTable: "Artist",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_SupportSlot_Show_ShowVenueId_ShowDate",
						columns: x => new { x.ShowVenueId, x.ShowDate },
						principalTable: "Show",
						principalColumns: new[] { "VenueId", "Date" },
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.InsertData(
				table: "Artist",
				columns: new[] { "Id", "Description", "Name", "Slug" },
				values: new object[,]
				{
					{ new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa28"), "dot-dot-dot to their friends (and fans), ... are a Canadian drone jazz combo formed in Vancouver in 1998, known for their thunderous horn section and innovative visuals.", "...", "dot-dot-dot" },
					{ new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa29"), "Known as the \"Silver Mountain Strings\" if you're into the whole brevity thing, Kentucky's answer to the Leningrad Cowboys has gone from strength to strength, from the Superbowl half time show to their sold-out Vegas residency in 2023.", "The Silver Mountain String Band featuring Timber J. MacCorkindale and the Hill Valley Hornswogglers", "silver-mountain-string-band" }
				});

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "rockaway-sample-admin-user",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
				values: new object[] { "fdee0023-b7b2-4bc2-821f-4a33a918ad20", "AQAAAAIAAYagAAAAEJy162WBBVFf6WWsYVLxm1cssO/2mphuAvMszK8qMpVSNkp35mQxSTzXa7CyTtcoOQ==", "f6108175-432a-48c5-b0dc-f23d5a2ea489" });

			migrationBuilder.InsertData(
				table: "Show",
				columns: new[] { "Date", "VenueId", "HeadlineArtistId" },
				values: new object[,]
				{
					{ new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 25), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") },
					{ new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3") }
				});

			migrationBuilder.InsertData(
				table: "SupportSlot",
				columns: new[] { "ShowDate", "ShowVenueId", "SlotNumber", "ArtistId" },
				values: new object[,]
				{
					{ new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11") },
					{ new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), 2, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa15") },
					{ new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), 3, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10") },
					{ new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11") },
					{ new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), 2, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa15") },
					{ new DateOnly(2024, 5, 25), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10") },
					{ new DateOnly(2024, 5, 25), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), 2, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa29") },
					{ new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10") },
					{ new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11") },
					{ new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7"), 2, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa15") },
					{ new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10") },
					{ new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9"), 1, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10") }
				});

			migrationBuilder.CreateIndex(
				name: "IX_Show_HeadlineArtistId",
				table: "Show",
				column: "HeadlineArtistId");

			migrationBuilder.CreateIndex(
				name: "IX_SupportSlot_ArtistId",
				table: "SupportSlot",
				column: "ArtistId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "SupportSlot");

			migrationBuilder.DropTable(
				name: "Show");

			migrationBuilder.DeleteData(
				table: "Artist",
				keyColumn: "Id",
				keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa28"));

			migrationBuilder.DeleteData(
				table: "Artist",
				keyColumn: "Id",
				keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa29"));

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "rockaway-sample-admin-user",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
				values: new object[] { "ec9d7c74-d3f3-4b0e-bac1-9c2a393cc55c", "AQAAAAIAAYagAAAAELzmg82ZfBDgbYxzXE7Ew/hIVsDQ6kXEQE9TAJ3Kz12S/L5Ay9lHXAstreED/up5uw==", "abe08569-b6f1-411d-aa87-fa529538a7c7" });
		}
	}
}