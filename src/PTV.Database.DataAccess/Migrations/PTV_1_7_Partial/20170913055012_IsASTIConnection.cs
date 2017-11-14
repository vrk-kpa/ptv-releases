using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class IsASTIConnection : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsASTIConnection",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                defaultValue: false);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsASTIConnection",
                schema: "public",
                table: "ServiceServiceChannel");
        }
    }
}
