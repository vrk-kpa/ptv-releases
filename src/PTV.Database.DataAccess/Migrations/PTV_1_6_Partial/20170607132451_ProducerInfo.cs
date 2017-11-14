/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_6_Partial
{
    public partial class ProducerInfo : IPartialMigration
    {

        private readonly DataUtils dataUtils;
        public ProducerInfo()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationServiceWebPage",
                schema: "public");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationServiceAdditionalInformation_OrganizationService",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationService_ProvisionType_ProvisionTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationService_RoleType_RoleTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationService",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropIndex(
                name: "IX_OrgSer_Id",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropIndex(
                name: "IX_OrgSer_ProvisionTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropIndex(
                name: "IX_OrgSer_RoleTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.CreateTable(
                name: "ServiceProducer",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ProvisionTypeId = table.Column<Guid>(nullable: false),
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProducer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProducer_ProvisionType_ProvisionTypeId",
                        column: x => x.ProvisionTypeId,
                        principalSchema: "public",
                        principalTable: "ProvisionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProducer_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProducer_Id",
                schema: "public",
                table: "ServiceProducer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProducer_ProvisionTypeId",
                schema: "public",
                table: "ServiceProducer",
                column: "ProvisionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProducer_ServiceVersionedId",
                schema: "public",
                table: "ServiceProducer",
                column: "ServiceVersionedId");

            migrationBuilder.CreateTable(
                name: "ServiceProducerOrganization",
                schema: "public",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ServiceProducerId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProducerOrganization", x => new { x.OrganizationId, x.ServiceProducerId });
                    table.ForeignKey(
                        name: "FK_ServiceProducerOrganization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProducerOrganization_ServiceProducer_ServiceProducerId",
                        column: x => x.ServiceProducerId,
                        principalSchema: "public",
                        principalTable: "ServiceProducer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerProOrg_OrganizationId",
                schema: "public",
                table: "ServiceProducerOrganization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerProOrg_ServiceProducerId",
                schema: "public",
                table: "ServiceProducerOrganization",
                column: "ServiceProducerId");

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_6", "1ServiceProducer.sql"));

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropColumn(
                name: "ProvisionTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropColumn(
                name: "RoleTypeId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationService",
                schema: "public",
                table: "OrganizationService",
                columns: new[] { "OrganizationId", "ServiceVersionedId" });

            migrationBuilder.DropTable(
                name: "RoleTypeName",
                schema: "public");
            
            migrationBuilder.DropTable(
                name: "RoleType",
                schema: "public");

            migrationBuilder.RenameTable(
                name: "OrganizationServiceAdditionalInformation",
                newName: "ServiceProducerAdditionalInformation",
                schema: "public"
                );

            migrationBuilder.RenameColumn(
                name: "OrganizationServiceId",
                newName: "ServiceProducerId",
                table: "ServiceProducerAdditionalInformation",
                schema: "public");

            migrationBuilder.CreateIndex(
                name: "IX_SerProAddInf_ServiceProducerId",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                column: "ServiceProducerId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
