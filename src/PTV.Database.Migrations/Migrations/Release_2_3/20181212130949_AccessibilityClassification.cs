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
    public partial class AccessibilityClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerChaVer_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_WcagLevelType_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.CreateTable(
                name: "AccessibilityClassification",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Url = table.Column<string>(maxLength: 500, nullable: true),
                    AccessibilityClassificationLevelTypeId = table.Column<Guid>(nullable: false),
                    WcagLevelTypeId = table.Column<Guid>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityClassification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccCla_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId",
                        column: x => x.AccessibilityClassificationLevelTypeId,
                        principalSchema: "public",
                        principalTable: "AccessibilityClassificationLevelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccessibilityClassification_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessibilityClassification_WcagLevelType_WcagLevelTypeId",
                        column: x => x.WcagLevelTypeId,
                        principalSchema: "public",
                        principalTable: "WcagLevelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelAccessibilityClassification",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    AccessibilityClassificationId = table.Column<Guid>(nullable: false),
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelAccessibilityClassification", x => new { x.ServiceChannelVersionedId, x.AccessibilityClassificationId });
                    table.ForeignKey(
                        name: "FK_SerChaAccCla_AccessibilityClassificationId",
                        column: x => x.AccessibilityClassificationId,
                        principalSchema: "public",
                        principalTable: "AccessibilityClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SerChaAccCla_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccCla_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "AccessibilityClassification",
                column: "AccessibilityClassificationLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccCla_Id",
                schema: "public",
                table: "AccessibilityClassification",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccCla_LocalizationId",
                schema: "public",
                table: "AccessibilityClassification",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccCla_WcagLevelTypeId",
                schema: "public",
                table: "AccessibilityClassification",
                column: "WcagLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAccCla_AccessibilityClassificationId",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                column: "AccessibilityClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAccCla_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                column: "ServiceChannelVersionedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceChannelAccessibilityClassification",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessibilityClassification",
                schema: "public");

            migrationBuilder.AddColumn<Guid>(
                name: "AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AccessibilityClassificationLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "WcagLevelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SerChaVer_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AccessibilityClassificationLevelTypeId",
                principalSchema: "public",
                principalTable: "AccessibilityClassificationLevelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_WcagLevelType_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "WcagLevelTypeId",
                principalSchema: "public",
                principalTable: "WcagLevelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
