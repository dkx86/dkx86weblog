using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dkx86weblog.Migrations
{
    public partial class DigitalPackages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DigitalPackage",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    PackageFileName = table.Column<string>(maxLength: 64, nullable: true),
                    PreviewFileName = table.Column<string>(maxLength: 72, nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    FileSize = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalPackage", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalPackage");
        }
    }
}
