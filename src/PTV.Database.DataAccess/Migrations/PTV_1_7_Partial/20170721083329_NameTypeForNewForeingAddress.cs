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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class NameTypeForNewForeingAddress : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressCharacterName",
                schema: "public",
                table: "AddressCharacterName");

            migrationBuilder.DropIndex(
                name: "IX_AddChaNam_Id",
                schema: "public",
                table: "AddressCharacterName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AddressCharacterName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressCharacterName",
                schema: "public",
                table: "AddressCharacterName",
                columns: new[] { "TypeId", "LocalizationId" });
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressCharacterName",
                schema: "public",
                table: "AddressCharacterName");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AddressCharacterName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressCharacterName",
                schema: "public",
                table: "AddressCharacterName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddChaNam_Id",
                schema: "public",
                table: "AddressCharacterName",
                column: "Id");
        }
    }
}
