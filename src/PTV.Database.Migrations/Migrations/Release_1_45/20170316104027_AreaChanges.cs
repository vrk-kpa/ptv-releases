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
    internal partial class AreaChanges : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AreaChanges()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
			migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "2AreaInformationType.sql"));

            migrationBuilder.AddColumn<Guid>(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValueSql: @"GetOrCreateDefaultAreaInformationTypeId('WholeCountry')");

            migrationBuilder.AddColumn<Guid>(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Area",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AreaTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Area_AreaType_AreaTypeId",
                        column: x => x.AreaTypeId,
                        principalSchema: "public",
                        principalTable: "AreaType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAreaMunicipality",
                schema: "public",
                columns: table => new
                {
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAreaMunicipality", x => new { x.OrganizationVersionedId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_OrganizationAreaMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationAreaMunicipality_OrganizationVersioned_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAreaMunicipality",
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
                    table.PrimaryKey("PK_ServiceAreaMunicipality", x => new { x.ServiceVersionedId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_ServiceAreaMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAreaMunicipality_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelAreaMunicipality",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelAreaMunicipality", x => new { x.ServiceChannelVersionedId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_ServiceChannelAreaMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelAreaMunicipality_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaMunicipality",
                schema: "public",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaMunicipality", x => new { x.AreaId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_AreaMunicipality_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "public",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaName",
                schema: "public",
                columns: table => new
                {
                    AreaId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaName", x => new { x.AreaId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AreaName_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "public",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationArea",
                schema: "public",
                columns: table => new
                {
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    AreaId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationArea", x => new { x.OrganizationVersionedId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_OrganizationArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "public",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationArea_OrganizationVersioned_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceArea",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    AreaId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceArea", x => new { x.ServiceVersionedId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_ServiceArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "public",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceArea_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelArea",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    AreaId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelArea", x => new { x.ServiceChannelVersionedId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_ServiceChannelArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "public",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelArea_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "AreaInformationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AreaInformationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Are_AreaTypeId",
                schema: "public",
                table: "Area",
                column: "AreaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Are_Id",
                schema: "public",
                table: "Area",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AreMun_AreaId",
                schema: "public",
                table: "AreaMunicipality",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AreMun_MunicipalityId",
                schema: "public",
                table: "AreaMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AreNam_AreaId",
                schema: "public",
                table: "AreaName",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AreNam_LocalizationId",
                schema: "public",
                table: "AreaName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAre_AreaId",
                schema: "public",
                table: "OrganizationArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAre_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationArea",
                column: "OrganizationVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAreMun_MunicipalityId",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAreMun_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                column: "OrganizationVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerAre_AreaId",
                schema: "public",
                table: "ServiceArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SerAre_ServiceVersionedId",
                schema: "public",
                table: "ServiceArea",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerAreMun_MunicipalityId",
                schema: "public",
                table: "ServiceAreaMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerAreMun_ServiceVersionedId",
                schema: "public",
                table: "ServiceAreaMunicipality",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAre_AreaId",
                schema: "public",
                table: "ServiceChannelArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAre_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelArea",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAreMun_MunicipalityId",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAreMun_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                column: "ServiceChannelVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AreaInformationTypeId",
                principalSchema: "public",
                principalTable: "AreaInformationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "AreaInformationTypeId",
                principalSchema: "public",
                principalTable: "AreaInformationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_AreaInformationType_AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropTable(
                name: "AreaMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AreaName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationArea",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationAreaMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceArea",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceAreaMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelArea",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelAreaMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Area",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_SerVer_AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "AreaInformationTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
