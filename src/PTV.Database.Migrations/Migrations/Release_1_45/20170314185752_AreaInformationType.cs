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

namespace PTV.Database.Migrations.Migrations.Release_1_45
{
    public partial class AreaInformationType : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AreaInformationType()
        {
            migrateHelper = new MigrateHelper();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {
			migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "2AreaInformationType.sql"));

            migrationBuilder.CreateTable(
                name: "AreaInformationType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaInformationType", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                defaultValueSql: @"GetOrCreateDefaultAreaInformationTypeId('WholeCountry')"
            );

            migrationBuilder.CreateTable(
                name: "AreaInformationTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaInformationTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaInformationTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaInformationTypeName_AreaInformationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AreaInformationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgVer_AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "AreaInformationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AreInfTyp_Id",
                schema: "public",
                table: "AreaInformationType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AreInfTypNam_Id",
                schema: "public",
                table: "AreaInformationTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AreInfTypNam_LocalizationId",
                schema: "public",
                table: "AreaInformationTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AreInfTypNam_TypeId",
                schema: "public",
                table: "AreaInformationTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "AreaInformationTypeId",
                principalSchema: "public",
                principalTable: "AreaInformationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropTable(
                name: "AreaInformationTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AreaInformationType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_OrgVer_AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropColumn(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "OrganizationVersioned");
        }
    }
}
