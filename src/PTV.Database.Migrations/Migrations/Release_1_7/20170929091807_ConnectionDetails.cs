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
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class ConnectionDetails : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public ConnectionDetails()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceHours",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IsClosed = table.Column<bool>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OpeningHoursFrom = table.Column<DateTime>(nullable: true),
                    OpeningHoursTo = table.Column<DateTime>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true),
                    ServiceHourTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHours_ServiceHourType_ServiceHourTypeId",
                        column: x => x.ServiceHourTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceHourType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_7", "2ServiceHours.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_DailyOpeningTime_ServiceChannelServiceHours_OpeningHourId",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelServiceHours_ServiceHourType_ServiceHourTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHoursAdditionalInformation_ServiceChannelServiceHours_ServiceChannelServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelServiceHours",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropIndex(
                name: "IX_SerChaSerHou_Id",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "ServiceHourTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "OpeningHoursFrom",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "OpeningHoursTo",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                newName: "ServiceHoursId");

            migrationBuilder.RenameIndex(
                name: "IX_SerHouAddInf_ServiceChannelServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                newName: "IX_SerHouAddInf_ServiceHoursId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "ServiceHoursId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelServiceHours",
                schema: "public",
                table: "ServiceChannelServiceHours",
                columns: new[] { "ServiceChannelVersionedId", "ServiceHoursId" });
            
            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelAddress",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ServiceServiceChannelAddress", x => new { x.ServiceId, x.ServiceChannelId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelAddress_AddressCharacter_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "public",
                        principalTable: "AddressCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelAddress_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelAddress_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelEmail",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ServiceServiceChannelEmail", x => new { x.ServiceId, x.ServiceChannelId, x.EmailId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelEmail_Email_EmailId",
                        column: x => x.EmailId,
                        principalSchema: "public",
                        principalTable: "Email",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelEmail_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelEmail_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelPhone",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ServiceServiceChannelPhone", x => new { x.ServiceId, x.ServiceChannelId, x.PhoneId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelPhone_Phone_PhoneId",
                        column: x => x.PhoneId,
                        principalSchema: "public",
                        principalTable: "Phone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelPhone_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelPhone_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelWebPage",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ServiceServiceChannelWebPage", x => new { x.ServiceId, x.ServiceChannelId, x.WebPageId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelWebPage_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelWebPage_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelServiceHours",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_ServiceServiceChannelServiceHours", x => new { x.ServiceId, x.ServiceChannelId, x.ServiceHoursId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelServiceHours_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelServiceHours_ServiceHours_ServiceHoursId",
                        column: x => x.ServiceHoursId,
                        principalSchema: "public",
                        principalTable: "ServiceHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelServiceHours_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerHou_Id",
                schema: "public",
                table: "ServiceHours",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerHou_ServiceHourTypeId",
                schema: "public",
                table: "ServiceHours",
                column: "ServiceHourTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_AddressId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_CharacterId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_EmailId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_PhoneId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceHoursId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceHoursId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_WebPageId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "WebPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyOpeningTime_ServiceHours_OpeningHourId",
                schema: "public",
                table: "DailyOpeningTime",
                column: "OpeningHourId",
                principalSchema: "public",
                principalTable: "ServiceHours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelServiceHours_ServiceHours_ServiceHoursId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "ServiceHoursId",
                principalSchema: "public",
                principalTable: "ServiceHours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHoursAdditionalInformation_ServiceHours_ServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                column: "ServiceHoursId",
                principalSchema: "public",
                principalTable: "ServiceHours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceServiceChannelAddress",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelEmail",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelPhone",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelServiceHours",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelWebPage",
                schema: "public");
            
        }
    }
}
