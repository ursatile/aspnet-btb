using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rockaway.WebApp.Migrations {
	/// <inheritdoc />
	public partial class CreateTicketOrders : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("07247117-d897-47ed-a957-c6b3210ee1fc"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("0ddd32c6-43d5-492d-8069-967b0d0665f2"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("1c539681-db0c-4d30-b5ae-a87980250719"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("334cc0ff-f1f9-4a6b-827c-c8ca75a849dc"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("47d2ed52-e0ba-424f-9284-a7ed60cc8124"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("55a4491c-0574-4595-b63f-c0cd466125dd"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("7763c787-c721-421f-80f8-a4bfe1c41314"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("9900cff2-89be-43d2-8f92-25f0f48967d9"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("b328f089-e465-4d0b-befa-61d3b472a9d7"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("c398bc43-0181-4bdd-8820-adcf59a17828"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("ed8cd99e-e4c2-4aa3-ab2d-4a8cb6605b66"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("f7225ae7-7ed1-44d7-80f6-ad913595f565"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("fb5e2d51-8511-41d2-98c1-4e7d6eb7bf63"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("feaa8b63-8d17-4215-bea3-11f043b25f67"));

			migrationBuilder.CreateTable(
				name: "TicketOrder",
				columns: table => new {
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					ShowDate = table.Column<DateOnly>(type: "date", nullable: true),
					ShowVenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_TicketOrder", x => x.Id);
					table.ForeignKey(
						name: "FK_TicketOrder_Show_ShowVenueId_ShowDate",
						columns: x => new { x.ShowVenueId, x.ShowDate },
						principalTable: "Show",
						principalColumns: new[] { "VenueId", "Date" });
				});

			migrationBuilder.CreateTable(
				name: "TicketOrderItem",
				columns: table => new {
					TicketOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_TicketOrderItem", x => new { x.TicketOrderId, x.TicketTypeId });
					table.ForeignKey(
						name: "FK_TicketOrderItem_TicketOrder_TicketOrderId",
						column: x => x.TicketOrderId,
						principalTable: "TicketOrder",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_TicketOrderItem_TicketType_TicketTypeId",
						column: x => x.TicketTypeId,
						principalTable: "TicketType",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "rockaway-sample-admin-user",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
				values: new object[] { "7b9a44f7-cc16-4c5f-86ec-ea392abc27ab", "AQAAAAIAAYagAAAAEImvuoFcqmHCvxKJtFmmPR73NapKhSf8PjDWbz3lC2lp7WKm12nwCSsDupwh9zpFsg==", "f77dc522-8f80-4fd2-8253-705df167330a" });

			migrationBuilder.InsertData(
				table: "TicketType",
				columns: new[] { "Id", "Limit", "Name", "Price", "ShowDate", "ShowVenueId" },
				values: new object[,]
				{
					{ new Guid("2fc6d145-6075-4c10-8326-ca7adb5b5727"), null, "VIP Meet & Greet", 75m, new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2") },
					{ new Guid("3352b1ab-605f-49bc-b938-141d68eb7bc7"), null, "VIP Meet & Greet", 750m, new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5") },
					{ new Guid("4ca6cab5-0e04-4abb-a310-83b9b9e34a52"), null, "Downstairs standing", 25m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("55ff6e9d-e091-461a-ab30-b9359900a4c1"), null, "General Admission", 35m, new DateOnly(2024, 5, 19), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2") },
					{ new Guid("7a97fa4c-ae29-4180-abac-6777f61657c4"), null, "General Admission", 300m, new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8") },
					{ new Guid("89a3141b-3191-4cd6-b4a4-dc5fed127259"), null, "VIP Meet & Greet", 75m, new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3") },
					{ new Guid("8e92740d-c283-4de6-a2ec-c8a850212d45"), null, "General Admission", 25m, new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9") },
					{ new Guid("ade83857-69ff-4ee5-a0a6-18a7f23ddfd3"), null, "VIP Meet & Greet", 720m, new DateOnly(2024, 5, 23), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8") },
					{ new Guid("bf355665-288e-4c4a-8075-bfeea2f10c73"), null, "Upstairs unallocated seating", 25m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("c435a52f-e4df-4921-abf7-55b9e5688e77"), null, "General Admission", 25m, new DateOnly(2024, 5, 25), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4") },
					{ new Guid("e01be398-c103-4d18-b085-e0dd38df0e38"), null, "General Admission", 35m, new DateOnly(2024, 5, 18), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3") },
					{ new Guid("f1a3e169-995d-41eb-97c8-0a00e1838248"), null, "Cabaret table (4 people)", 120m, new DateOnly(2024, 5, 17), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7") },
					{ new Guid("f25b372e-1506-4737-a72d-44a0eef445fa"), null, "VIP Meet & Greet", 55m, new DateOnly(2024, 5, 20), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9") },
					{ new Guid("fd6b57d6-8404-4034-96ba-1af87baca65b"), null, "General Admission", 350m, new DateOnly(2024, 5, 22), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5") }
				});

			migrationBuilder.CreateIndex(
				name: "IX_TicketOrder_ShowVenueId_ShowDate",
				table: "TicketOrder",
				columns: new[] { "ShowVenueId", "ShowDate" });

			migrationBuilder.CreateIndex(
				name: "IX_TicketOrderItem_TicketTypeId",
				table: "TicketOrderItem",
				column: "TicketTypeId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "TicketOrderItem");

			migrationBuilder.DropTable(
				name: "TicketOrder");

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("2fc6d145-6075-4c10-8326-ca7adb5b5727"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("3352b1ab-605f-49bc-b938-141d68eb7bc7"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("4ca6cab5-0e04-4abb-a310-83b9b9e34a52"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("55ff6e9d-e091-461a-ab30-b9359900a4c1"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("7a97fa4c-ae29-4180-abac-6777f61657c4"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("89a3141b-3191-4cd6-b4a4-dc5fed127259"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("8e92740d-c283-4de6-a2ec-c8a850212d45"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("ade83857-69ff-4ee5-a0a6-18a7f23ddfd3"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("bf355665-288e-4c4a-8075-bfeea2f10c73"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("c435a52f-e4df-4921-abf7-55b9e5688e77"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("e01be398-c103-4d18-b085-e0dd38df0e38"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("f1a3e169-995d-41eb-97c8-0a00e1838248"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("f25b372e-1506-4737-a72d-44a0eef445fa"));

			migrationBuilder.DeleteData(
				table: "TicketType",
				keyColumn: "Id",
				keyValue: new Guid("fd6b57d6-8404-4034-96ba-1af87baca65b"));

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
		}
	}
}