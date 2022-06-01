using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_4
{
    public partial class SetServiceVoucherTypeByOldValues : Migration
    {
        private readonly MigrateHelper migrateHelper = new MigrateHelper();
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Set service voucher types by old existing data.");
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_3_4", "2SetServiceVoucherType.sql"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
