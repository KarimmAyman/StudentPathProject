using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "LocationId",
                table: "Locations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_LocationId",
                table: "Locations",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_AspNetUsers_LocationId",
                table: "Locations",
                column: "LocationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_AspNetUsers_LocationId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_LocationId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Locations");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
