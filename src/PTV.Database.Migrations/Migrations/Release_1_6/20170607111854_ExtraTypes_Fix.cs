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

using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_6
{
    public partial class ExtraTypes_Fix : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
