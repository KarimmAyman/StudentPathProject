using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class creationtriprequestmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false),
                    IsLookingForTrip = table.Column<bool>(type: "bit", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripRequests_TripLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "TripLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripRequests_TripLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "TripLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_FromLocationId",
                table: "TripRequests",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_ToLocationId",
                table: "TripRequests",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripRequests_UserId",
                table: "TripRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripRequests");
        }
    }
}
