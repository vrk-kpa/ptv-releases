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
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_3
{
    public partial class ClsAddressesExtendedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralDescriptionBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ChannelBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionBlockedAccessRight",
                schema: "public");

            migrationBuilder.AddColumn<bool>(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreetNumber");

            migrationBuilder.DropColumn(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreetNumber");

            migrationBuilder.DropColumn(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreetName");

            migrationBuilder.DropColumn(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreetName");

            migrationBuilder.DropColumn(
                name: "NonCls",
                schema: "public",
                table: "ClsAddressStreet");

            migrationBuilder.DropColumn(
                name: "UpdateIdentifier",
                schema: "public",
                table: "ClsAddressStreet");

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_GenDesBloAccRig_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesBloAccRig_StatutoryServiceGeneralDescription_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                });

            migrationBuilder.CreateTable(
                name: "OrganizationBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_OrgBloAccRig_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationBlockedAccessRight_Organization_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_ServiceBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceBlockedAccessRight_Service_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_SerColBloAccRig_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SerColBloAccRig_ServiceCollection_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "ServiceCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesBloAccRig_AccessBlockedId",
                schema: "public",
                table: "GeneralDescriptionBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesBloAccRig_EntityId",
                schema: "public",
                table: "GeneralDescriptionBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_EntityId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgBloAccRig_AccessBlockedId",
                schema: "public",
                table: "OrganizationBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgBloAccRig_EntityId",
                schema: "public",
                table: "OrganizationBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ServiceBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerBloAccRig_EntityId",
                schema: "public",
                table: "ServiceBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ServiceCollectionBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColBloAccRig_EntityId",
                schema: "public",
                table: "ServiceCollectionBlockedAccessRight",
                column: "EntityId");
        }
    }
}
