using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class OrgDescPrimaryKeyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationDescription",
                schema: "public",
                table: "OrganizationDescription");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationDescription",
                schema: "public",
                table: "OrganizationDescription",
                columns: new[] { "OrganizationId", "TypeId", "LocalizationId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationDescription",
                schema: "public",
                table: "OrganizationDescription");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationDescription",
                schema: "public",
                table: "OrganizationDescription",
                columns: new[] { "OrganizationId", "TypeId" });
        }
    }
}
