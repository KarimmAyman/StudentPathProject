using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class editinmettingpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingPoint",
                table: "Bookings");

            migrationBuilder.AddColumn<double>(
                name: "MeetingPoint_Latitude",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MeetingPoint_Longitude",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingPoint_Latitude",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MeetingPoint_Longitude",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "MeetingPoint",
                table: "Bookings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
