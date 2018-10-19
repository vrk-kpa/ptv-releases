using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class SoftHyphensRemoval : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        public SoftHyphensRemoval()
        {
            migrateHelper = new MigrateHelper();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_2_0", "5RemoveSoftHyphens.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
