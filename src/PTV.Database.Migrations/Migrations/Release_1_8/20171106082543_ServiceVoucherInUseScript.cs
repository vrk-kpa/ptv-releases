using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    /// <summary>
    /// Manualy created migration to rerun script which was missing in 1.75 release
    /// </summary>
    public partial class ServiceVoucherInUseScript : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public ServiceVoucherInUseScript()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "2ServiceVoucherInUse.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
