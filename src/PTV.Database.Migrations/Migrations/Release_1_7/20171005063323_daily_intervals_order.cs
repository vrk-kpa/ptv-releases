/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
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
