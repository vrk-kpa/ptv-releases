using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;
using System.IO;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class daily_intervals_order : IPartialMigration
    {
        private readonly DataUtils dataUtils;
        public daily_intervals_order()
        {
            dataUtils = new DataUtils();
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

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_7", "3OpenningHoursPKchange.sql"));

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
