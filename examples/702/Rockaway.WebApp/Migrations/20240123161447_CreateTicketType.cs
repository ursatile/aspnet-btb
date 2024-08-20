using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rockaway.WebApp.Migrations {
	/// <inheritdoc />
	public partial class CreateTicketType : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "TicketType",
				columns: table => new {
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ShowVenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ShowDate = table.Column<DateOnly>(type: "date", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Price = table.Column<decimal>(type: "money", nullable: false),
					Limit = table.Column<int>(type: "int", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_TicketType", x => x.Id);
					table.ForeignKey(
						name: "FK_TicketType_Show_ShowVenueId_ShowDate",
						columns: x => new { x.ShowVenueId, x.ShowDate },
						principalTable: "Show",
						principalColumns: new[] { "VenueId", "Date" },
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "rockaway-sample-admin-user",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
				values: new object[] { "12fc68df-895b-4b0f-8ed5-267cb5025ddf", "AQAAAAIAAYagAAAAEOXs6BkR3fturyCz71tJvH84Hv2oqOpmiztTntqNeTaITRrn6pyX3W006kOV6VdVaw==", "11d3521e-d2bd-4589-a226-5ad3b423293b" });

			migrationBuilder.InsertData(
				table: "TicketType",
				columns: new[] { "Id", "Limit", "Name", "Price", "ShowDate", "ShowVenueId" },
				values: new object[,]
				{
					{ new Guid("07247117-d897-47ed-a957-c6b3210ee1fc"), null, "VIP Meet & Greet", 75m, new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3") },
					{ new Guid("0ddd32c6-43d5-492d-8069-967b0d0665f2"), null, "Upstairs unallocated seating", 25m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("1c539681-db0c-4d30-b5ae-a87980250719"), null, "Cabaret table (4 people)", 120m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("334cc0ff-f1f9-4a6b-827c-c8ca75a849dc"), null, "General Admission", 300m, new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8") },
					{ new Guid("47d2ed52-e0ba-424f-9284-a7ed60cc8124"), null, "General Admission", 35m, new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2") },
					{ new Guid("55a4491c-0574-4595-b63f-c0cd466125dd"), null, "VIP Meet & Greet", 75m, new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2") },
					{ new Guid("7763c787-c721-421f-80f8-a4bfe1c41314"), null, "VIP Meet & Greet", 55m, new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9") },
					{ new Guid("9900cff2-89be-43d2-8f92-25f0f48967d9"), null, "General Admission", 25m, new DateOnly(2024, 5, 25), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4") },
					{ new Guid("b328f089-e465-4d0b-befa-61d3b472a9d7"), null, "Downstairs standing", 25m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("c398bc43-0181-4bdd-8820-adcf59a17828"), null, "General Admission", 25m, new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9") },
					{ new Guid("ed8cd99e-e4c2-4aa3-ab2d-4a8cb6605b66"), null, "General Admission", 350m, new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5") },
					{ new Guid("f7225ae7-7ed1-44d7-80f6-ad913595f565"), null, "General Admission", 35m, new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3") },
					{ new Guid("fb5e2d51-8511-41d2-98c1-4e7d6eb7bf63"), null, "VIP Meet & Greet", 720m, new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8") },
					{ new Guid("feaa8b63-8d17-4215-bea3-11f043b25f67"), null, "VIP Meet & Greet", 750m, new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5") }
				});

			migrationBuilder.CreateIndex(
				name: "IX_TicketType_ShowVenueId_ShowDate",
				table: "TicketType",
				columns: new[] { "ShowVenueId", "ShowDate" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "TicketType");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "rockaway-sample-admin-user",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
				values: new object[] { "2e7dd6d8-1ab6-47c0-99c1-eb92f9fd8c2b", "AQAAAAIAAYagAAAAEL6pXWpBCqEip/BNAfLsU2ybnkbkc7M9uU4PSIi3mkzH5U89bX+fa/2XL1JwlH+2yw==", "3fdeb9f6-2b48-4307-bbff-50083306f913" });
		}
	}
}