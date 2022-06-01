using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_3_12
{
    public partial class GeoserverUpdates8 : Migration
    {
        private readonly MigrateHelper migrateHelper = new MigrateHelper();
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Updating GeoServer objects.");

            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_3_3_12", "1GeoServerUpdate.sql"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
