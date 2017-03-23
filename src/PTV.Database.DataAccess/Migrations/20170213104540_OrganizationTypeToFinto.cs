using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class OrganizationTypeToFinto : Migration
    {
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

            migrationBuilder.Sql(File.ReadAllText(@"SqlMigrations\PTV_1_4\Fixed\4OrganizationTypeUriSetting.sql"));            
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
