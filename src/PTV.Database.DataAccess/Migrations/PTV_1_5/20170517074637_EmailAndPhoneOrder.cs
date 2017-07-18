using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class EmailAndPhoneOrder : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "Phone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "Email",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "Phone");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "Email");
        }
    }
}
