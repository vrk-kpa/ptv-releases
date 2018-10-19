using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0_5
{
    public partial class TranslationsInfoDetails : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InfoDetails",
                schema: "public",
                table: "TranslationOrderState",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfoDetails",
                schema: "public",
                table: "TranslationOrderState");
        }
    }
}
