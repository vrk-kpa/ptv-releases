using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class ResponsibleOrganization : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgVer_ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "ResponsibleOrganizationRegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_Organization_ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "ResponsibleOrganizationRegionId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationVersioned_Organization_ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropIndex(
                name: "IX_OrgVer_ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropColumn(
                name: "ResponsibleOrganizationRegionId",
                schema: "public",
                table: "OrganizationVersioned");
        }
    }
}
