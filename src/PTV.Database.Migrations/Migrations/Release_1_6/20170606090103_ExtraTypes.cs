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

namespace PTV.Database.Migrations.Migrations.Release_1_6
{
    public partial class ExtraTypes : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtraType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtraSubType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ExtraTypeId = table.Column<Guid>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraSubType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraSubType_ExtraType_ExtraTypeId",
                        column: x => x.ExtraTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraTypeName_ExtraType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraSubTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraSubTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraSubTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraSubTypeName_ExtraSubType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ExtraSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelExtraType",
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
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceChannelExtraType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelExtraType_ExtraSubType_ExtraSubTypeId",
                        column: x => x.ExtraSubTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelExtraType_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelExtraTypeDescription",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    ServiceServiceChannelExtraTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceChannelExtraTypeDescription", x => new { x.LocalizationId, x.ServiceServiceChannelExtraTypeId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelExtraTypeDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelExtraTypeDescription_ServiceServiceChannelExtraType_ServiceServiceChannelExtraTypeId",
                        column: x => x.ServiceServiceChannelExtraTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceServiceChannelExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTyp_ExtraTypeId",
                schema: "public",
                table: "ExtraSubType",
                column: "ExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTyp_Id",
                schema: "public",
                table: "ExtraSubType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTypNam_Id",
                schema: "public",
                table: "ExtraSubTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTypNam_LocalizationId",
                schema: "public",
                table: "ExtraSubTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTypNam_TypeId",
                schema: "public",
                table: "ExtraSubTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtTyp_Id",
                schema: "public",
                table: "ExtraType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtTypNam_Id",
                schema: "public",
                table: "ExtraTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtTypNam_LocalizationId",
                schema: "public",
                table: "ExtraTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtTypNam_TypeId",
                schema: "public",
                table: "ExtraTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ExtraSubTypeId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ExtraSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_Id",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTypDes_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTypDes_ServiceServiceChannelExtraTypeId",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                column: "ServiceServiceChannelExtraTypeId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraSubTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ExtraTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelExtraTypeDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelExtraType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ExtraSubType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ExtraType",
                schema: "public");
        }
    }
}
