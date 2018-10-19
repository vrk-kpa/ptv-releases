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

namespace PTV.Database.Migrations.Migrations.Release_1_6
{
    public partial class mandatoryFields : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organization_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_ServiceFundingType_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.AlterColumn<Guid>(
                name: "FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceFundingType_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "FundingTypeId",
                principalSchema: "public",
                principalTable: "ServiceFundingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organization_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_ServiceFundingType_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.AlterColumn<Guid>(
                name: "FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceFundingType_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "FundingTypeId",
                principalSchema: "public",
                principalTable: "ServiceFundingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
