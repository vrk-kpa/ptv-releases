using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class FintoDescription : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OntologyTermDescription",
                schema: "public",
                columns: table => new
                {
                    OntologyTermId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTermDescription", x => new { x.OntologyTermId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_OntologyTermDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OntologyTermDescription_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceClassDescription",
                schema: "public",
                columns: table => new
                {
                    ServiceClassId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceClassDescription", x => new { x.ServiceClassId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ServiceClassDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceClassDescription_ServiceClass_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OntTerDes_LocalizationId",
                schema: "public",
                table: "OntologyTermDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerDes_OntologyTermId",
                schema: "public",
                table: "OntologyTermDescription",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_SerClaDes_LocalizationId",
                schema: "public",
                table: "ServiceClassDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerClaDes_ServiceClassId",
                schema: "public",
                table: "ServiceClassDescription",
                column: "ServiceClassId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OntologyTermDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceClassDescription",
                schema: "public");
        }
    }
}
