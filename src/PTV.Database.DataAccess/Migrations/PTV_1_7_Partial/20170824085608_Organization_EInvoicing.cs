using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class Organization_EInvoicing : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationEInvoicing",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ElectronicInvoicingAddress = table.Column<string>(maxLength: 110, nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OperatorCode = table.Column<string>(maxLength: 110, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationEInvoicing", x => new { x.Id, x.OrganizationVersionedId });
                    table.UniqueConstraint("AK_OrganizationEInvoicing_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationEInvoicing_OrganizationVersioned_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationEInvoicingAdditionalInformation",
                schema: "public",
                columns: table => new
                {
                    OrganizationEInvoicingId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Text = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationEInvoicingAdditionalInformation", x => new { x.OrganizationEInvoicingId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_OrganizationEInvoicingAdditionalInformation_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationEInvoicingAdditionalInformation_OrganizationEInvoicing_OrganizationEInvoicingId",
                        column: x => x.OrganizationEInvoicingId,
                        principalSchema: "public",
                        principalTable: "OrganizationEInvoicing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgEInv_Id",
                schema: "public",
                table: "OrganizationEInvoicing",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgEInv_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationEInvoicing",
                column: "OrganizationVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgEInvAddInf_LocalizationId",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgEInvAddInf_OrganizationEInvoicingId",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                column: "OrganizationEInvoicingId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationEInvoicingAdditionalInformation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationEInvoicing",
                schema: "public");
        }
    }
}
