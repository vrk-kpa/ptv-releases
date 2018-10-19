using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;
using System;
using System.Collections.Generic;

namespace PTV.Database.Migrations.Migrations.Release_2_0_5
{
    public partial class UiAccessRightsDb : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BlockOtherTypes",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FilterType",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccessRightsOperationsUI",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AllowedAllOrganizations = table.Column<bool>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: true),
                    Permission = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    RulesAll = table.Column<long>(nullable: false),
                    RulesOwn = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRightsOperationsUI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessRightsOperationsUI_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccRigOpeUI_Id",
                schema: "public",
                table: "AccessRightsOperationsUI",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRigOpeUI_OrganizationId",
                schema: "public",
                table: "AccessRightsOperationsUI",
                column: "OrganizationId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRightsOperationsUI",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "BlockOtherTypes",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.DropColumn(
                name: "FilterType",
                schema: "public",
                table: "RestrictionFilter");
        }
    }
}
