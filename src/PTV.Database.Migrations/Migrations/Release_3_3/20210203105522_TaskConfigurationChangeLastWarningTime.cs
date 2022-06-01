using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_3
{
    public partial class TaskConfigurationChangeLastWarningTime : Migration
    {
        private readonly MigrateHelper migrateHelper = new MigrateHelper();
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Changing last warning times for drafts in TaskConfiguration table.");
            
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_3_3", "2UpdateTaskConfigurationLastWarningTimes.sql")
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        
        }
    }
}
