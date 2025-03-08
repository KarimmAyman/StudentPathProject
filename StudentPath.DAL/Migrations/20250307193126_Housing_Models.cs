using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Housing_Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisingStatus = table.Column<int>(type: "int", nullable: false),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false),
                    HousingType = table.Column<int>(type: "int", nullable: false),
                    Rooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    GrossArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WarmingType = table.Column<int>(type: "int", nullable: true),
                    BuildingAge = table.Column<int>(type: "int", nullable: true),
                    FloorLocation = table.Column<int>(type: "int", nullable: true),
                    IsFurnished = table.Column<bool>(type: "bit", nullable: true),
                    IsAvailableForLoan = table.Column<bool>(type: "bit", nullable: true),
                    Dues = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Front = table.Column<int>(type: "int", nullable: true),
                    RentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Properties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationProperties_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeatures",
                columns: table => new
                {
                    PropertyFeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    FeatureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFeatures", x => x.PropertyFeatureId);
                    table.ForeignKey(
                        name: "FK_PropertyFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyFeatures_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyImages",
                columns: table => new
                {
                    PropertyImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyImages", x => x.PropertyImageId);
                    table.ForeignKey(
                        name: "FK_PropertyImages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Category", "Name" },
                values: new object[,]
                {
                    { 1, 0, "ADSL" },
                    { 2, 0, "Alarm" },
                    { 3, 0, "Balcony" },
                    { 4, 0, "Built-in Kitchen" },
                    { 5, 0, "Barbecue" },
                    { 6, 0, "Furnished" },
                    { 7, 0, "Laundry Room" },
                    { 8, 0, "Air Conditioning" },
                    { 9, 0, "Wallpaper" },
                    { 10, 0, "Dressing Room" },
                    { 11, 0, "Video Intercom" },
                    { 12, 0, "Jacuzzi" },
                    { 13, 0, "Shower" },
                    { 14, 0, "TV Satellite" },
                    { 15, 0, "Laminate" },
                    { 16, 0, "Panel Door" },
                    { 17, 0, "Marble Floor" },
                    { 18, 0, "Blinds" },
                    { 19, 0, "Sauna" },
                    { 20, 0, "Parent Bathroom" },
                    { 21, 0, "Parquet" },
                    { 22, 0, "Satin Plaster" },
                    { 23, 0, "Satin Color" },
                    { 24, 0, "Ceramic Floor" },
                    { 25, 0, "Spotlight" },
                    { 26, 0, "Fireplace" },
                    { 27, 0, "Terrace" },
                    { 28, 0, "Cloakroom" },
                    { 29, 0, "Underfloor Heating" },
                    { 30, 0, "Double Glazing" },
                    { 31, 1, "Elevator" },
                    { 32, 1, "Gardened" },
                    { 33, 1, "Fitness" },
                    { 34, 1, "Security" },
                    { 35, 1, "Thermal Insulation" },
                    { 36, 1, "Generator" },
                    { 37, 1, "Doorman" },
                    { 38, 1, "Car Park" },
                    { 39, 1, "Playground" },
                    { 40, 1, "PVC" },
                    { 41, 1, "Siding" },
                    { 42, 1, "Water Tank" },
                    { 43, 1, "Tennis Court" },
                    { 44, 1, "Fire Escape" },
                    { 45, 1, "Swimming Pool" },
                    { 46, 1, "Football Field" },
                    { 47, 1, "Basketball Field" },
                    { 48, 1, "Market" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationProperties_PropertyId",
                table: "LocationProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFeatures_FeatureId",
                table: "PropertyFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFeatures_PropertyId",
                table: "PropertyFeatures",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_PropertyId",
                table: "PropertyImages",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationProperties");

            migrationBuilder.DropTable(
                name: "PropertyFeatures");

            migrationBuilder.DropTable(
                name: "PropertyImages");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
