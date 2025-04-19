using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DriverCRUDsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "vehicleInfos");

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductionYear",
                table: "vehicleInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleColor",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehiclePicturePath",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleRegistrationBackPath",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleRegistrationFrontPath",
                table: "vehicleInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CriminalRecordPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdBackPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdFrontPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseBackPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseFrontPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseSelfiePath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehiclePicturePath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleRegistrationBackPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleRegistrationFrontPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "ProductionYear",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleColor",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehiclePicturePath",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleRegistrationBackPath",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "VehicleRegistrationFrontPath",
                table: "vehicleInfos");

            migrationBuilder.DropColumn(
                name: "CriminalRecordPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdBackPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdFrontPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseBackPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseFrontPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicenseSelfiePath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VehiclePicturePath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VehicleRegistrationBackPath",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VehicleRegistrationFrontPath",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "vehicleInfos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "vehicleInfos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
