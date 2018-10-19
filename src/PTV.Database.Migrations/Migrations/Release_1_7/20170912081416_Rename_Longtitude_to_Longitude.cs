using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class Rename_Longtitude_to_Longitude : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longtitude",
                schema: "public",
                table: "Coordinate",
                newName: "Longitude");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                schema: "public",
                table: "Coordinate",
                newName: "Longtitude");
        }
    }
}
