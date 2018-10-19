using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class TranslationOrderLanguageStateCulture : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public TranslationOrderLanguageStateCulture()
        {
            migrateHelper = new MigrateHelper();
        }
        
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Last",
                schema: "public",
                table: "TranslationOrderState",
                type: "bool",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LanguageStateCulture",
                schema: "public",
                table: "Language",
                type: "text",
                nullable: true);

            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_1_8_5", "1TranslationOrderStateLast.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Last",
                schema: "public",
                table: "TranslationOrderState");

            migrationBuilder.DropColumn(
                name: "LanguageStateCulture",
                schema: "public",
                table: "Language");
        }
    }
}
