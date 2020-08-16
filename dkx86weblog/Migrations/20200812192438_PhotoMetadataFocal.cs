using Microsoft.EntityFrameworkCore.Migrations;

namespace dkx86weblog.Migrations
{
    public partial class PhotoMetadataFocal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Aperture",
                table: "Photo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "FocalLength",
                table: "Photo",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FocalLength",
                table: "Photo");

            migrationBuilder.AlterColumn<int>(
                name: "Aperture",
                table: "Photo",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
