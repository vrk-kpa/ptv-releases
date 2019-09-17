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

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class AddedEntityOriginIdColumn : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "OriginalId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SerColVer_ServiceCollectionVersioned_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceCollectionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceVersioned_OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_SerColVer_ServiceCollectionVersioned_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_ServiceVersioned_OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerVer_OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerColVer_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
