using Microsoft.EntityFrameworkCore.Migrations;

namespace dkx86weblog.Migrations
{
    public partial class PhotoMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Aperture",
                table: "Photo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CameraName",
                table: "Photo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExposureTime",
                table: "Photo",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ISO",
                table: "Photo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PreviewFileName",
                table: "DigitalPackage",
                maxLength: 72,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PackageFileName",
                table: "DigitalPackage",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aperture",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "CameraName",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ExposureTime",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "ISO",
                table: "Photo");

            migrationBuilder.AlterColumn<string>(
                name: "PreviewFileName",
                table: "DigitalPackage",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 72,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PackageFileName",
                table: "DigitalPackage",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);
        }
    }
}
