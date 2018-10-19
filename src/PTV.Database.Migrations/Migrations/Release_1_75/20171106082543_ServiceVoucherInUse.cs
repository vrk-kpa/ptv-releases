using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_75
{
    public partial class ServiceVoucherInUse : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public ServiceVoucherInUse()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WebPageInUse",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValue: false);

            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "2ServiceVoucherInUse.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebPageInUse",
                schema: "public",
                table: "ServiceVersioned");
        }
    }
}
