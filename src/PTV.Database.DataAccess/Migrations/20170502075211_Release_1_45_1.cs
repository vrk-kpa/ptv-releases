using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class Release_1_45_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityFallback",
                schema: "public",
                table: "PublishingStatusType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UseOrg_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropIndex(
                name: "IX_UseOrg_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "PriorityFallback",
                schema: "public",
                table: "PublishingStatusType");
        }
    }
}
