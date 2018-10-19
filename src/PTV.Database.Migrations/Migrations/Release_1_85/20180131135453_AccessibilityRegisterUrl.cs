using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class AccessibilityRegisterUrl : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccReg_ServiceChannelId",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Url",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccReg_ServiceChannelId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "ServiceChannelId",
                unique: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccReg_ServiceChannelId",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.DropColumn(
                name: "AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.DropColumn(
                name: "Url",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.CreateIndex(
                name: "IX_AccReg_ServiceChannelId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "ServiceChannelId");
        }
    }
}
