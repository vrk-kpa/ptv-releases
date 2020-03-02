/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class GDConnectionDetails_Remove : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelAddress",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelEmail",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelPhone",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelServiceHours",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelWebPage",
                schema: "public");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelAddress",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelAddress", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelAddress_AddressCharacter_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "public",
                        principalTable: "AddressCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelAddress_GeneralDescriptionServiceChannel_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelEmail",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    EmailId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelEmail", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId, x.EmailId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelEmail_Email_EmailId",
                        column: x => x.EmailId,
                        principalSchema: "public",
                        principalTable: "Email",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelEmail_GeneralDescriptionServiceChannel_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelPhone",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    PhoneId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelPhone", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId, x.PhoneId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelPhone_Phone_PhoneId",
                        column: x => x.PhoneId,
                        principalSchema: "public",
                        principalTable: "Phone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelPhone_GeneralDescriptionServiceChannel_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelServiceHours",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ServiceHoursId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelServiceHours", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId, x.ServiceHoursId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelServiceHours_ServiceHours_ServiceHoursId",
                        column: x => x.ServiceHoursId,
                        principalSchema: "public",
                        principalTable: "ServiceHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelServiceHours_GeneralDescriptionServiceChannel_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelWebPage",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelWebPage", x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId, x.WebPageId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionServiceChannelWebPage_GeneralDescriptionServiceChannel_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaAdd_AddressId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaAdd_CharacterId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelAddress",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaAdd_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelAddress",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaEma_EmailId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelEmail",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaEma_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelEmail",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaPho_PhoneId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelPhone",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaPho_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelPhone",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaSerHou_ServiceHoursId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelServiceHours",
                column: "ServiceHoursId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaSerHou_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelServiceHours",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaWebPag_WebPageId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaWebPag_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelWebPage",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });
        }
    }
}
