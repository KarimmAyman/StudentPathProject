using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTripAndTripLocationsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromLocation",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ToLocation",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Trips",
                newName: "DriverNotes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FromLocationId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasAirConditioning",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFreeWater",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMusic",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPhoneCharger",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWiFi",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ToLocationId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TripLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripLocations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_FromLocationId",
                table: "Trips",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ToLocationId",
                table: "Trips",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripLocations_Latitude_Longitude",
                table: "TripLocations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TripLocations_FromLocationId",
                table: "Trips",
                column: "FromLocationId",
                principalTable: "TripLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TripLocations_ToLocationId",
                table: "Trips",
                column: "ToLocationId",
                principalTable: "TripLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TripLocations_FromLocationId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TripLocations_ToLocationId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "TripLocations");

            migrationBuilder.DropIndex(
                name: "IX_Trips_FromLocationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ToLocationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "HasAirConditioning",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "HasFreeWater",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "HasMusic",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "HasPhoneCharger",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "HasWiFi",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "DriverNotes",
                table: "Trips",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "FromLocation",
                table: "Trips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToLocation",
                table: "Trips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
