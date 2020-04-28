using Microsoft.EntityFrameworkCore.Migrations;

namespace dkx86weblog.Data.Migrations
{
    public partial class PhotoSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Photo",
                newName: "ID");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Photo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Photo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Photo");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Photo",
                newName: "Id");
        }
    }
}
