using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedDistanceAndDurationToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DriverNotes",
                table: "Trips",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrivalTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedDistance",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedDuration",
                table: "Trips",
                type: "time",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripLocations_FullAddress",
                table: "TripLocations",
                column: "FullAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TripLocations_FullAddress",
                table: "TripLocations");

            migrationBuilder.DropColumn(
                name: "EstimatedArrivalTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EstimatedDistance",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "Trips");

            migrationBuilder.AlterColumn<string>(
                name: "DriverNotes",
                table: "Trips",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
