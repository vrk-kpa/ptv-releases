using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;
using System;
using System.Collections.Generic;

namespace PTV.Database.Migrations.Migrations.Release_2_0_5
{
    public partial class ARIsValid : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: false,
                defaultValue: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "AccessibilityRegister");
        }
    }
}
