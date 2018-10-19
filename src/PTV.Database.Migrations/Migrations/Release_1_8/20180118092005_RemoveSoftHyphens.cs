using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class RemoveSoftHyphens : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        public RemoveSoftHyphens()
        {
            migrateHelper = new MigrateHelper();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {

            migrateHelper.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_1_8", "6RemoveSoftHyphens.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
