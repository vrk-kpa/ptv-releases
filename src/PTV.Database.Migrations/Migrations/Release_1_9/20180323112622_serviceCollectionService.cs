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

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class serviceCollectionService : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.RenameColumn(
                   name: "ServiceCollectionVersionedId",
                   schema: "public",
                   table: "ServiceCollectionService",
                   newName: "ServiceCollectionId");           

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService",
                columns: new[] { "ServiceCollectionId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerColSer_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollection_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionId",
                principalSchema: "public",
                principalTable: "ServiceCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollection_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropIndex(
                name: "IX_SerColSer_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropColumn(
                name: "ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService",
                columns: new[] { "ServiceCollectionVersionedId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionVersionedId",
                principalSchema: "public",
                principalTable: "ServiceCollectionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
