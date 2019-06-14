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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_45
{
    public partial class OrganizationDisplayNameType : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public OrganizationDisplayNameType()
        {
            migrateHelper = new MigrateHelper();

        }

        public void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateTable(
                name: "OrganizationDisplayNameType",
                schema: "public",
                columns: table => new
                {
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DisplayNameTypeId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDisplayNameType", x => new { x.OrganizationVersionedId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_NameType_DisplayNameTypeId",
                        column: x => x.DisplayNameTypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_OrganizationVersioned_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "DisplayNameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_LocalizationId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "OrganizationVersionedId");

			migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "5OrganizationDisplayNameType.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_NameType_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropIndex(
                name: "IX_Org_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropColumn(
                name: "DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationDisplayNameType",
                schema: "public");

            migrationBuilder.AddColumn<Guid>(
                name: "DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrgVer_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "DisplayNameTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_NameType_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "DisplayNameTypeId",
                principalSchema: "public",
                principalTable: "NameType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
