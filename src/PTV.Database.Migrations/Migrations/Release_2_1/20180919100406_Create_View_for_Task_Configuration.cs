using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class Create_View_for_Task_Configuration : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        
        public Create_View_for_Task_Configuration()
        {
            migrateHelper = new MigrateHelper();
        }
        
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_2_1", "3TaskConfigurationView.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
