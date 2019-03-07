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

using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.Migrations.Migrations.Base
{
    public partial class OrganizationTypeToFinto : Migration
    {
        private readonly MigrateHelper migrateHelper;

        public OrganizationTypeToFinto()
        {
            migrateHelper = new MigrateHelper();
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_OrganizationType_OrganizationType_ParentTypeId",
               schema: "public",
               table: "OrganizationType");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "OrganizationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgTypNam_TypeId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "IX_OrgTypNam_OrganizationTypeId");

            migrationBuilder.RenameColumn(
                name: "ParentTypeId",
                schema: "public",
                table: "OrganizationType",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgTyp_ParentTypeId",
                schema: "public",
                table: "OrganizationType",
                newName: "IX_OrgTyp_ParentId");

            migrationBuilder.AddColumn<string>(
                name: "Label",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OntologyType",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentUri",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uri",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationType_OrganizationType_ParentId",
                schema: "public",
                table: "OrganizationType",
                column: "ParentId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "OrganizationTypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

			migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4", "Fixed", "4OrganizationTypeUriSetting.sql"));            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationType_OrganizationType_ParentId",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropColumn(
                name: "Label",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "OntologyType",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "ParentUri",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "Uri",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.RenameColumn(
                name: "OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgTypNam_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "IX_OrgTypNam_TypeId");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                schema: "public",
                table: "OrganizationType",
                newName: "ParentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgTyp_ParentId",
                schema: "public",
                table: "OrganizationType",
                newName: "IX_OrgTyp_ParentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationType_OrganizationType_ParentTypeId",
                schema: "public",
                table: "OrganizationType",
                column: "ParentTypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_TypeId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
