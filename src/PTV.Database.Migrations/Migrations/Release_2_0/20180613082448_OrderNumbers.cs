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

using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class OrderNumbers : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                schema: "public",
                table: "ServiceLanguage",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "Order",
                schema: "public",
                table: "ServiceChannelLanguage",
                newName: "OrderNumber");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceLaw",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationPhone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationEmail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationAddress",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: true,
                oldClrType: typeof(int));

            // Handwriten DailyOpeningTime migration to prevent data loss //
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime"
            );
            migrationBuilder.DropIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_Order",
                schema: "public",
                table: "DailyOpeningTime"
            );
            migrationBuilder.AlterColumn<int>(
                name: "Order",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: true,
                oldClrType: typeof(int)
            );
            migrationBuilder.RenameColumn(
                name: "Order",
                schema: "public",
                table: "DailyOpeningTime",
                newName: "OrderNumber"
            );
            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "OrderNumber" }
            );
            migrationBuilder.CreateIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_OrderNumber",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] { "OpeningHourId", "DayFrom", "OrderNumber" }
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyOpeningTime",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropIndex(
                name: "IX_DaiOpeTim_OpeningHourId_DayFrom_OrderNumber",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "StatutoryServiceLaw");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceLaw");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationAddress");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceLanguage",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelLanguage",
                newName: "Order");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
    }
}
