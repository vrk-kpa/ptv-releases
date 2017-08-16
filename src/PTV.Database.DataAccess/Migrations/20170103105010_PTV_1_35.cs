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

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PTV_1_35 : Migration
    {
        private DataUtils dataUtils;
        public PTV_1_35()
        {
            this.dataUtils = new DataUtils();
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScripts(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_35"));
		
            migrationBuilder.DropForeignKey(
                name: "FK_IndustrialClassName_IndustrialClass_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropForeignKey(
                name: "FK_LifeEventName_LifeEvent_LifeEventId",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropForeignKey(
                name: "FK_OntologyTermName_OntologyTerm_OntologyTermId",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceClassName_ServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetGroupName_TargetGroup_TargetGroupId",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "Municipality");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                schema: "public",
                table: "ServiceRequirement",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceChannelDescription",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceDescription",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LifeEventId",
                schema: "public",
                table: "LifeEventName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "public",
                table: "Language",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DigitalAuthorization",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OntologyType = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentUri = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalAuthorization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalAuthorization_DigitalAuthorization_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "DigitalAuthorization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityName",
                schema: "public",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityName", x => new { x.MunicipalityId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_MunicipalityName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MunicipalityName_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DigitalAuthorizationName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DigitalAuthorizationId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalAuthorizationName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalAuthorizationName_DigitalAuthorization_DigitalAuthorizationId",
                        column: x => x.DigitalAuthorizationId,
                        principalSchema: "public",
                        principalTable: "DigitalAuthorization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DigitalAuthorizationName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelDigitalAuthorization",
                schema: "public",
                columns: table => new
                {
                    DigitalAuthorizationId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceChannelDigitalAuthorization", x => new { x.DigitalAuthorizationId, x.ServiceId, x.ServiceChannelId });
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelDigitalAuthorization_DigitalAuthorization_DigitalAuthorizationId",
                        column: x => x.DigitalAuthorizationId,
                        principalSchema: "public",
                        principalTable: "DigitalAuthorization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelDigitalAuthorization_ServiceServiceChannel_ServiceId_ServiceChannelId",
                        columns: x => new { x.ServiceId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "ServiceServiceChannel",
                        principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalAuthorization_Id",
                schema: "public",
                table: "DigitalAuthorization",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalAuthorization_ParentId",
                schema: "public",
                table: "DigitalAuthorization",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalAuthorizationName_DigitalAuthorizationId",
                schema: "public",
                table: "DigitalAuthorizationName",
                column: "DigitalAuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalAuthorizationName_Id",
                schema: "public",
                table: "DigitalAuthorizationName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalAuthorizationName_LocalizationId",
                schema: "public",
                table: "DigitalAuthorizationName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_LocalizationId",
                schema: "public",
                table: "MunicipalityName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_MunicipalityId",
                schema: "public",
                table: "MunicipalityName",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannelDigitalAuthorization_DigitalAuthorizationId",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                column: "DigitalAuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannelDigitalAuthorization_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_IndustrialClassName_IndustrialClass_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                column: "IndustrialClassId",
                principalSchema: "public",
                principalTable: "IndustrialClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LifeEventName_LifeEvent_LifeEventId",
                schema: "public",
                table: "LifeEventName",
                column: "LifeEventId",
                principalSchema: "public",
                principalTable: "LifeEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OntologyTermName_OntologyTerm_OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                column: "OntologyTermId",
                principalSchema: "public",
                principalTable: "OntologyTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceClassName_ServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                column: "ServiceClassId",
                principalSchema: "public",
                principalTable: "ServiceClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetGroupName_TargetGroup_TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                column: "TargetGroupId",
                principalSchema: "public",
                principalTable: "TargetGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndustrialClassName_IndustrialClass_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropForeignKey(
                name: "FK_LifeEventName_LifeEvent_LifeEventId",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropForeignKey(
                name: "FK_OntologyTermName_OntologyTerm_OntologyTermId",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceClassName_ServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetGroupName_TargetGroup_TargetGroupId",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropTable(
                name: "DigitalAuthorizationName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MunicipalityName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceServiceChannelDigitalAuthorization",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DigitalAuthorization",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "public",
                table: "Language");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "Requirement",
                schema: "public",
                table: "ServiceRequirement",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceDescription",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LifeEventId",
                schema: "public",
                table: "LifeEventName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_IndustrialClassName_IndustrialClass_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                column: "IndustrialClassId",
                principalSchema: "public",
                principalTable: "IndustrialClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LifeEventName_LifeEvent_LifeEventId",
                schema: "public",
                table: "LifeEventName",
                column: "LifeEventId",
                principalSchema: "public",
                principalTable: "LifeEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OntologyTermName_OntologyTerm_OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                column: "OntologyTermId",
                principalSchema: "public",
                principalTable: "OntologyTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceClassName_ServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                column: "ServiceClassId",
                principalSchema: "public",
                principalTable: "ServiceClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetGroupName_TargetGroup_TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                column: "TargetGroupId",
                principalSchema: "public",
                principalTable: "TargetGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
