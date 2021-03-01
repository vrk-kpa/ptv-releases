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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_75
{
    public partial class AddressPostalCodeNotMandatory : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AddressPostalCodeNotMandatory()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

           migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "1AddressPostalCode.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
