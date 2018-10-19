using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class OrgRestrictionFilter : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestrictionFilter",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    FilterName = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    TypeValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationFilter",
                schema: "public",
                columns: table => new
                {
                    FilterId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationFilter", x => new { x.FilterId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_OrganizationFilter_RestrictionFilter_FilterId",
                        column: x => x.FilterId,
                        principalSchema: "public",
                        principalTable: "RestrictionFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationFilter_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgFil_FilterId",
                schema: "public",
                table: "OrganizationFilter",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgFil_OrganizationId",
                schema: "public",
                table: "OrganizationFilter",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_Id",
                schema: "public",
                table: "RestrictionFilter",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationFilter",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RestrictionFilter",
                schema: "public");
        }
    }
}
