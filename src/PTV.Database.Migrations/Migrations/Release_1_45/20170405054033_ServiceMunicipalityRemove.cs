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
    internal partial class ServiceMunicipalityRemove : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public ServiceMunicipalityRemove()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
			migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "6ServiceMunicipality.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceCoverageType_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropTable(
                name: "ServiceCoverageTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCoverageType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Ser_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCoverageType",
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
                    table.PrimaryKey("PK_ServiceCoverageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMunicipality",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMunicipality", x => new { x.ServiceVersionedId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCoverageTypeName",
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
                    table.PrimaryKey("PK_ServiceCoverageTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCoverageTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCoverageTypeName_ServiceCoverageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceCoverageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ServiceCoverageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTyp_Id",
                schema: "public",
                table: "ServiceCoverageType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_Id",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_LocalizationId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_TypeId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerMun_MunicipalityId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerMun_ServiceVersionedId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "ServiceVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceCoverageType_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ServiceCoverageTypeId",
                principalSchema: "public",
                principalTable: "ServiceCoverageType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
