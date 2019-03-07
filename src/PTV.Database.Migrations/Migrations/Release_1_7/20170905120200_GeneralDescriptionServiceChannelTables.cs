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
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class GeneralDescriptionServiceChannelTables : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannel",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ChargeTypeId = table.Column<Guid>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenDesSerChan", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChan_ChaTypId",
                        column: x => x.ChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenDesSerChan_SerChaId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChan_GenDesId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelExtraType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ExtraSubTypeId = table.Column<Guid>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenDesSerChaExtTyp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_ExtSubTypId",
                        column: x => x.ExtraSubTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_SerChaId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_GenDesId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelDescription",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenDesSerChaDes", x => new { x.TypeId, x.LocalizationId, x.ServiceChannelId, x.StatutoryServiceGeneralDescriptionId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_LocId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_TypId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_SerChaId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelDigitalAuthorization",
                schema: "public",
                columns: table => new
                {
                    DigitalAuthorizationId = table.Column<Guid>(nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenDesSerChaDesDigAut", x => new { x.DigitalAuthorizationId, x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDesDigAut_DigAutId",
                        column: x => x.DigitalAuthorizationId,
                        principalSchema: "public",
                        principalTable: "DigitalAuthorization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDesDigAut_SerChaId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelExtraTypeDescription",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    GeneralDescriptionServiceChannelExtraTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenDesSerChaExtTypDes", x => new { x.LocalizationId, x.GeneralDescriptionServiceChannelExtraTypeId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTypDes_GenDesSerChaExtTypId",
                        column: x => x.GeneralDescriptionServiceChannelExtraTypeId,
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannelExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTypDes_LocId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerCha_ChargeTypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerCha_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerCha_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_LocalizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_TypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDigAut_DigitalAuthorizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                column: "DigitalAuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_ExtraSubTypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "ExtraSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_Id",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                column: "GeneralDescriptionServiceChannelExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTypDes_LocalizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                column: "LocalizationId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelDigitalAuthorization",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelExtraTypeDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannel",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelExtraType",
                schema: "public");
        }
    }
}
