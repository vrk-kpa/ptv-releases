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

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class RestrictionFilterExtended : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"RestrictionFilter\";");

            migrationBuilder.DropIndex(
                name: "IX_ResFil_TypeName",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.DropColumn(
                name: "TypeValue",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.AddColumn<Guid>(
                name: "RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false);

            migrationBuilder.RenameColumn(
                name: "TypeName",
                schema: "public",
                table: "RestrictionFilter",
                newName: "ColumnName");

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_ColumnName",
                schema: "public",
                table: "RestrictionFilter",
                column: "ColumnName");

            migrationBuilder.CreateTable(
                name: "RestrictedType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TypeName = table.Column<string>(nullable: false),
                    Value = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter",
                column: "RestrictedTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResTyp_Id",
                schema: "public",
                table: "RestrictedType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestrictionFilter_RestrictedType_RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter",
                column: "RestrictedTypeId",
                principalSchema: "public",
                principalTable: "RestrictedType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestrictionFilter_RestrictedType_RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.DropTable(
                name: "RestrictedType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_ResFil_RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.RenameColumn(
                name: "RestrictedTypeId",
                schema: "public",
                table: "RestrictionFilter",
                newName: "TypeValue");

            migrationBuilder.RenameColumn(
                name: "ColumnName",
                schema: "public",
                table: "RestrictionFilter",
                newName: "TypeName");

            migrationBuilder.RenameIndex(
                name: "IX_ResFil_ColumnName",
                schema: "public",
                table: "RestrictionFilter",
                newName: "IX_ResFil_TypeName");
        }
    }
}
