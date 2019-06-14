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

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class chargeType_rename : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phone_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "Phone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.RenameColumn(
                name: "ServiceChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Ser_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_SerVer_ChargeTypeId");

            migrationBuilder.RenameColumn(
                name: "ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SerSerCha_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "IX_SerSerCha_ChargeTypeId");

            migrationBuilder.RenameColumn(
                name: "ServiceChargeTypeId",
                schema: "public",
                table: "Phone",
                newName: "ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pho_ServiceChargeTypeId",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_ChargeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Phone_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "Phone",
                column: "ChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phone_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "Phone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.RenameColumn(
                name: "ChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SerVer_ChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_SerVer_ServiceChargeTypeId");

            migrationBuilder.RenameColumn(
                name: "ChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_SerSerCha_ChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "IX_SerSerCha_ServiceChargeTypeId");

            migrationBuilder.RenameColumn(
                name: "ChargeTypeId",
                schema: "public",
                table: "Phone",
                newName: "ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pho_ChargeTypeId",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_ServiceChargeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Phone_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "Phone",
                column: "ServiceChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ServiceChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
