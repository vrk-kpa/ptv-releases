using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public class OpeningHoursDateFix : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public OpeningHoursDateFix()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_9", "3ServiceHoursUtcTimeFix.sql"));
            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_9", "4EntityArchiveDateMove.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }

        public void BuildTargetModel(ModelBuilder modelBuilder)
        {
            
        }
    }
}