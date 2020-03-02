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

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class Added_UserAccessRightsGroup_Table : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAccessRightsGroup",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRightsGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessRightsGroupName",
                schema: "public",
                columns: table => new
                {
                    UserAccessRightsGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRightsGroupName", x => new { x.UserAccessRightsGroupId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_UserAccessRightsGroupName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccessRightsGroupName_UserAccessRightsGroup_UserAccessRightsGroupId",
                        column: x => x.UserAccessRightsGroupId,
                        principalSchema: "public",
                        principalTable: "UserAccessRightsGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UseAccRigGro_Id",
                schema: "public",
                table: "UserAccessRightsGroup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UseAccRigGroNam_LocalizationId",
                schema: "public",
                table: "UserAccessRightsGroupName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UseAccRigGroNam_UserAccessRightsGroupId",
                schema: "public",
                table: "UserAccessRightsGroupName",
                column: "UserAccessRightsGroupId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccessRightsGroupName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserAccessRightsGroup",
                schema: "public");
        }
    }
}
