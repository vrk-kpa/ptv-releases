using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class SoftHyphenRemoval : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        public SoftHyphenRemoval()
        {
            migrateHelper = new MigrateHelper();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_2_1", "2SoftHyphenRemoval.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
