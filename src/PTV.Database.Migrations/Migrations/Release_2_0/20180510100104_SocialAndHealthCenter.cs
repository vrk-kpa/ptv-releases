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

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class SocialAndHealthCenter : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceChannelDisplayNameType",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DisplayNameTypeId = table.Column<Guid>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelDisplayNameType", x => new { x.ServiceChannelVersionedId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ServiceChannelDisplayNameType_NameType_DisplayNameTypeId",
                        column: x => x.DisplayNameTypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelDisplayNameType_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelDisplayNameType_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelSocialHealthCenter",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Oid = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelSocialHealthCenter", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_ServiceChannelSocialHealthCenter_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerChaDisNamTyp_DisplayNameTypeId",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                column: "DisplayNameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaDisNamTyp_LocalizationId",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaDisNamTyp_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaSocHeaCen_Oid",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaSocHeaCen_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                column: "ServiceChannelId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
