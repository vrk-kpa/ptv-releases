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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_6_Partial
{
    public partial class UserMultiOrg : IPartialMigration
    {
        private DataUtils dataUtils;

        public UserMultiOrg()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""UserOrganization"" WHERE (""OrganizationId"" IS NULL) OR (""OrganizationId"" = '00000000-0000-0000-0000-000000000000') OR (""OrganizationId"" NOT IN (SELECT ""Id"" FROM ""Organization""))");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "RelationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5_5", "FK_ServiceServiceChannel.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "IsMain",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                schema: "public",
                table: "UserOrganization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "RelationId",
                schema: "public",
                table: "UserOrganization",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "public",
                table: "UserOrganization",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
