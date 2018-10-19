using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class daily_intervals_order : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        public daily_intervals_order()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_IsExtra",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                defaultValue: 0);

            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_7", "3OpenningHoursPKchange.sql"));

            migrationBuilder.DropColumn(
                name: "IsExtra",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_Order",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "Order" });
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_Order",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsExtra",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "IsExtra" });

            migrationBuilder.CreateIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_IsExtra",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "IsExtra" });
        }
    }
}
