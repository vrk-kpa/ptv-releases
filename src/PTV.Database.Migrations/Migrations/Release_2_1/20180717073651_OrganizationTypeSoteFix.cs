using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class OrganizationTypeSoteFix : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        
        public OrganizationTypeSoteFix()
        {
            migrateHelper = new MigrateHelper();
        }
        
        public void Up(MigrationBuilder migrationBuilder)
        { 
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_2_1", "1OrganizationTypeSoteFix.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
