using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class Is_open_247_to_service_hours : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public Is_open_247_to_service_hours()
        {
            migrateHelper = new MigrateHelper();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNonStop",
                schema: "public",
                table: "ServiceHours",
                nullable: false,
                defaultValue: false);

           migrateHelper.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_1_8", "4IsOpenNonStop.sql")
           );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNonStop",
                schema: "public",
                table: "ServiceHours");
        }
    }
}
