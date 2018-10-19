using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class TranslationCompanyOrderIdentificator : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslationCompanyOrderIdentifier",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslationCompanyOrderIdentifier",
                schema: "public",
                table: "TranslationOrder");
        }
    }
}
