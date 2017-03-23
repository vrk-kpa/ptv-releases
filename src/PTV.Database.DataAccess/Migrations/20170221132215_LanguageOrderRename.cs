using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class LanguageOrderRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Order",
                schema: "public",
                table: "Language",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: false);

            migrationBuilder.RenameColumn(
                name: "Order",
                newName: "OrderNumber",
                schema: "public",
                table: "Language");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
