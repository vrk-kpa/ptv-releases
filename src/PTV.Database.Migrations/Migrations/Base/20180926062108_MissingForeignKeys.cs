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
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Base
{
    public partial class MissingForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TasCon_PublishingStatusId",
                schema: "public",
                table: "TasksConfiguration",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AccReg_AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "AddressLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessibilityRegister_Language_AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "AddressLanguageId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TasksConfiguration_PublishingStatusType_PublishingStatusId",
                schema: "public",
                table: "TasksConfiguration",
                column: "PublishingStatusId",
                principalSchema: "public",
                principalTable: "PublishingStatusType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessibilityRegister_Language_AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_TasksConfiguration_PublishingStatusType_PublishingStatusId",
                schema: "public",
                table: "TasksConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_TasCon_PublishingStatusId",
                schema: "public",
                table: "TasksConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_AccReg_AddressLanguageId",
                schema: "public",
                table: "AccessibilityRegister");
        }
    }
}
