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
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PTV_1_40 : Migration
    {
        private void AddSqlScripts140Pre(MigrationBuilder migrationBuilder)
        {
            var directory = new DirectoryInfo(@"SqlMigrations\PTV_1_4\Pre");
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles("*.sql"))
                {
                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
                }

            }
        }

        private void AddSqlScripts140Post(MigrationBuilder migrationBuilder)
        {
            var directory = new DirectoryInfo(@"SqlMigrations\PTV_1_4\Post");
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles("*.sql"))
                {
                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
                }
            }
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddSqlScripts140Pre(migrationBuilder);

            migrationBuilder.RenameTable(
               name: "Service",
               newName: "ServiceVersioned"
           );
            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "OrganizationVersioned"
            );
            migrationBuilder.RenameTable(
               name: "ServiceChannel",
               newName: "ServiceChannelVersioned"
           );
            migrationBuilder.RenameTable(
                name: "StatutoryServiceGeneralDescription",
                newName: "StatutoryServiceGeneralDescriptionVersioned"
            );
            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRoot", x => x.Id);
                });

            migrationBuilder.CreateTable(
               name: "ServiceChannel",
               schema: "public",
               columns: table => new
               {
                   Id = table.Column<Guid>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_ServiceChannelRoot", x => x.Id);
               });
            migrationBuilder.CreateTable(
                name: "Service",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRoot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceGeneralDescription",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceGeneralDescriptionRoot", x => x.Id);
                });

            migrationBuilder.Sql(File.ReadAllText(@"SqlMigrations\PTV_1_4\Fixed\1FillInRootNodeTables.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_ElectronicChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ElectronicChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Organization_ParentId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddress_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationDescription_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationEmail_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationName_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationName");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPhone_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationService_Service_ServiceId",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationWebPage_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceType_TypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDescription_Service_ServiceId",
                schema: "public",
                table: "ServiceDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceElectronicCommunicationChannel_Service_ServiceId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceElectronicNotificationChannel_Service_ServiceId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannel_Organization_OrganizationId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelAttachment_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelDescription_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelEmail_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelKeyword_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelKeyword");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelLanguage_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelName_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelOntologyTerm_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelOntologyTerm");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelPhone_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelServiceClass_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelServiceHours_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelTargetGroup_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTargetGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelWebPage_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceIndustrialClass_Service_ServiceId",
                schema: "public",
                table: "ServiceIndustrialClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceKeyword_Service_ServiceId",
                schema: "public",
                table: "ServiceKeyword");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLanguage_Service_ServiceId",
                schema: "public",
                table: "ServiceLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLifeEvent_Service_ServiceId",
                schema: "public",
                table: "ServiceLifeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLocationChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceLocationChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceMunicipality_Service_ServiceId",
                schema: "public",
                table: "ServiceMunicipality");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceName_Service_ServiceId",
                schema: "public",
                table: "ServiceName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOntologyTerm_Service_ServiceId",
                schema: "public",
                table: "ServiceOntologyTerm");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequirement_Service_ServiceId",
                schema: "public",
                table: "ServiceRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceClass_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannel_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTargetGroup_Service_ServiceId",
                schema: "public",
                table: "ServiceTargetGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceDescription_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceIndustrialClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceLanguage_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceLaw_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLaw");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceLifeEvent_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLifeEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceName_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceName");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceOntologyTerm_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceRequirement_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceRequirement");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceServiceClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceServiceClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceTargetGroup_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceTargetGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_WebpageChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "WebpageChannel");

            migrationBuilder.DropTable(
                name: "ServiceWebPage",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceRequirement");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceName");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceChannelName");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceChannelDescription");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceElectronicNotificationChannel");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "ServiceDescription");


            migrationBuilder.CreateTable(
                name: "PrintableFormChannelIdentifier",
                schema: "public",
                columns: table => new
                {
                    PrintableFormChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    FormIdentifier = table.Column<string>(maxLength: 100, nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintableFormChannelIdentifier", x => new { x.PrintableFormChannelId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelIdentifier_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelIdentifier_PrintableFormChannel_PrintableFormChannelId",
                        column: x => x.PrintableFormChannelId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannelReceiver",
                schema: "public",
                columns: table => new
                {
                    PrintableFormChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    FormReceiver = table.Column<string>(maxLength: 100, nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintableFormChannelReceiver", x => new { x.PrintableFormChannelId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelReceiver_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelReceiver_PrintableFormChannel_PrintableFormChannelId",
                        column: x => x.PrintableFormChannelId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(File.ReadAllText(@"SqlMigrations\PTV_1_4\Fixed\3PrintableFormIdentifierReciever.sql"));

            migrationBuilder.DropColumn(
                name: "FormIdentifier",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropColumn(
                name: "FormReceiver",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropColumn(
                name: "PostOffice",
                schema: "public",
                table: "PostalCode");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "Phone");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "OrganizationName");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "OrganizationDescription");

            migrationBuilder.DropColumn(
                name: "Visible",
                schema: "public",
                table: "Email");

            migrationBuilder.DropColumn(
                name: "CoordinateState",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Longtitude",
                schema: "public",
                table: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_WebPageTypeName_TypeId",
                schema: "public",
                table: "WebPageTypeName",
                newName: "IX_WebPagTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_WebPageTypeName_LocalizationId",
                schema: "public",
                table: "WebPageTypeName",
                newName: "IX_WebPagTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_WebPageTypeName_Id",
                schema: "public",
                table: "WebPageTypeName",
                newName: "IX_WebPagTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WebPageType_Id",
                schema: "public",
                table: "WebPageType",
                newName: "IX_WebPagTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WebpageChannelUrl_WebpageChannelId",
                schema: "public",
                table: "WebpageChannelUrl",
                newName: "IX_WebChaUrl_WebpageChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_WebpageChannelUrl_LocalizationId",
                schema: "public",
                table: "WebpageChannelUrl",
                newName: "IX_WebChaUrl_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "WebpageChannel",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_WebpageChannel_ServiceChannelId",
                schema: "public",
                table: "WebpageChannel",
                newName: "IX_WebCha_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_WebpageChannel_Id",
                schema: "public",
                table: "WebpageChannel",
                newName: "IX_WebCha_Id");

            migrationBuilder.RenameIndex(
                name: "IX_WebPage_LocalizationId",
                schema: "public",
                table: "WebPage",
                newName: "IX_WebPag_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_WebPage_Id",
                schema: "public",
                table: "WebPage",
                newName: "IX_WebPag_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Versioning_PreviousVersionId",
                schema: "public",
                table: "Versioning",
                newName: "IX_Ver_PreviousVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Versioning_Id",
                schema: "public",
                table: "Versioning",
                newName: "IX_Ver_Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserOrganization_Id",
                schema: "public",
                table: "UserOrganization",
                newName: "IX_UseOrg_Id");

            migrationBuilder.RenameIndex(
                name: "IX_TargetGroupName_TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                newName: "IX_TarGroNam_TargetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetGroupName_LocalizationId",
                schema: "public",
                table: "TargetGroupName",
                newName: "IX_TarGroNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetGroupName_Id",
                schema: "public",
                table: "TargetGroupName",
                newName: "IX_TarGroNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_TargetGroup_ParentId",
                schema: "public",
                table: "TargetGroup",
                newName: "IX_TarGro_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetGroup_Id",
                schema: "public",
                table: "TargetGroup",
                newName: "IX_TarGro_Id");

            migrationBuilder.RenameIndex(
                name: "IX_StreetName_LocalizationId",
                schema: "public",
                table: "StreetName",
                newName: "IX_StrNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_StreetName_Id",
                schema: "public",
                table: "StreetName",
                newName: "IX_StrNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_StreetName_AddressId",
                schema: "public",
                table: "StreetName",
                newName: "IX_StrNam_AddressId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceTargetGroup_TargetGroupId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                newName: "IX_StaSerTarGro_TargetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceTargetGroup_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                newName: "IX_StaSerTarGro_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceServiceClass_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                newName: "IX_StaSerSerCla_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceServiceClass_ServiceClassId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                newName: "IX_StaSerSerCla_ServiceClassId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceRequirement_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                newName: "IX_StaSerReq_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceRequirement_LocalizationId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                newName: "IX_StaSerReq_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceRequirement_Id",
                schema: "public",
                table: "StatutoryServiceRequirement",
                newName: "IX_StaSerReq_Id");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceOntologyTerm_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                newName: "IX_StaSerOntTer_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceOntologyTerm_OntologyTermId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                newName: "IX_StaSerOntTer_OntologyTermId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceName",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceName_TypeId",
                schema: "public",
                table: "StatutoryServiceName",
                newName: "IX_StaSerNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceName_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceName",
                newName: "IX_StaSerNam_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceName_LocalizationId",
                schema: "public",
                table: "StatutoryServiceName",
                newName: "IX_StaSerNam_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLifeEvent_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                newName: "IX_StaSerLifEve_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLifeEvent_LifeEventId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                newName: "IX_StaSerLifEve_LifeEventId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLaw",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLaw_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLaw",
                newName: "IX_StaSerLaw_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLaw_LawId",
                schema: "public",
                table: "StatutoryServiceLaw",
                newName: "IX_StaSerLaw_LawId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLanguage_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                newName: "IX_StaSerLan_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceLanguage_LanguageId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                newName: "IX_StaSerLan_LanguageId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceIndustrialClass_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                newName: "IX_StaSerIndCla_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceIndustrialClass_IndustrialClassId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                newName: "IX_StaSerIndCla_IndustrialClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceGeneralDescription_TypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                newName: "IX_StaSerGenDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceGeneralDescription_Id",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                newName: "IX_StaSerGenDes_Id");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceGeneralDescription_ChargeTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                newName: "IX_StaSerGenDes_ChargeTypeId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceDescription",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceDescription_TypeId",
                schema: "public",
                table: "StatutoryServiceDescription",
                newName: "IX_StaSerDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceDescription",
                newName: "IX_StaSerDes_StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_StatutoryServiceDescription_LocalizationId",
                schema: "public",
                table: "StatutoryServiceDescription",
                newName: "IX_StaSerDes_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceTypeName_TypeId",
                schema: "public",
                table: "ServiceTypeName",
                newName: "IX_SerTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceTypeName_LocalizationId",
                schema: "public",
                table: "ServiceTypeName",
                newName: "IX_SerTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceTypeName_Id",
                schema: "public",
                table: "ServiceTypeName",
                newName: "IX_SerTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceType_Id",
                schema: "public",
                table: "ServiceType",
                newName: "IX_SerTyp_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceTargetGroup",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceTargetGroup_TargetGroupId",
                schema: "public",
                table: "ServiceTargetGroup",
                newName: "IX_SerTarGro_TargetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceTargetGroup_ServiceId",
                schema: "public",
                table: "ServiceTargetGroup",
                newName: "IX_SerTarGro_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannelDigitalAuthorization_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                newName: "IX_SerSerChaDigAut_ServiceId_ServiceChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannelDigitalAuthorization_DigitalAuthorizationId",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                newName: "IX_SerSerChaDigAut_DigitalAuthorizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannelDescription_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                newName: "IX_SerSerChaDes_ServiceId_ServiceChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannelDescription_TypeId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                newName: "IX_SerSerChaDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannelDescription_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                newName: "IX_SerSerChaDes_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannel_ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "IX_SerSerCha_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannel_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "IX_SerSerCha_ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannel",
                newName: "IX_SerSerCha_ServiceChannelId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceServiceClass",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceClass_ServiceId",
                schema: "public",
                table: "ServiceServiceClass",
                newName: "IX_SerSerCla_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceServiceClass",
                newName: "IX_SerSerCla_ServiceClassId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceRequirement",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceRequirement_ServiceId",
                schema: "public",
                table: "ServiceRequirement",
                newName: "IX_SerReq_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceRequirement_LocalizationId",
                schema: "public",
                table: "ServiceRequirement",
                newName: "IX_SerReq_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceRequirement_Id",
                schema: "public",
                table: "ServiceRequirement",
                newName: "IX_SerReq_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceOntologyTerm",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOntologyTerm_ServiceId",
                schema: "public",
                table: "ServiceOntologyTerm",
                newName: "IX_SerOntTer_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOntologyTerm_OntologyTermId",
                schema: "public",
                table: "ServiceOntologyTerm",
                newName: "IX_SerOntTer_OntologyTermId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceName",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceName_TypeId",
                schema: "public",
                table: "ServiceName",
                newName: "IX_SerNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceName_ServiceId",
                schema: "public",
                table: "ServiceName",
                newName: "IX_SerNam_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceName_LocalizationId",
                schema: "public",
                table: "ServiceName",
                newName: "IX_SerNam_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceMunicipality",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceMunicipality_ServiceId",
                schema: "public",
                table: "ServiceMunicipality",
                newName: "IX_SerMun_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceMunicipality_MunicipalityId",
                schema: "public",
                table: "ServiceMunicipality",
                newName: "IX_SerMun_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannelServiceArea_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                newName: "IX_SerLocChaSerAre_ServiceLocationChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannelServiceArea_MunicipalityId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                newName: "IX_SerLocChaSerAre_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannelAddress_TypeId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                newName: "IX_SerLocChaAdd_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannelAddress_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                newName: "IX_SerLocChaAdd_ServiceLocationChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannelAddress_AddressId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                newName: "IX_SerLocChaAdd_AddressId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceLocationChannel",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceLocationChannel",
                newName: "IX_SerLocCha_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLocationChannel_Id",
                schema: "public",
                table: "ServiceLocationChannel",
                newName: "IX_SerLocCha_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceLifeEvent",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLifeEvent_ServiceId",
                schema: "public",
                table: "ServiceLifeEvent",
                newName: "IX_SerLifEve_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLifeEvent_LifeEventId",
                schema: "public",
                table: "ServiceLifeEvent",
                newName: "IX_SerLifEve_LifeEventId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceLanguage",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLanguage_ServiceId",
                schema: "public",
                table: "ServiceLanguage",
                newName: "IX_SerLan_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceLanguage_LanguageId",
                schema: "public",
                table: "ServiceLanguage",
                newName: "IX_SerLan_LanguageId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceKeyword",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceKeyword_ServiceId",
                schema: "public",
                table: "ServiceKeyword",
                newName: "IX_SerKey_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceKeyword_KeywordId",
                schema: "public",
                table: "ServiceKeyword",
                newName: "IX_SerKey_KeywordId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceIndustrialClass",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceIndustrialClass_ServiceId",
                schema: "public",
                table: "ServiceIndustrialClass",
                newName: "IX_SerIndCla_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceIndustrialClass_IndustrialClassId",
                schema: "public",
                table: "ServiceIndustrialClass",
                newName: "IX_SerIndCla_IndustrialClassId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChargeTypeName_TypeId",
                schema: "public",
                table: "ServiceChargeTypeName",
                newName: "IX_ServCharTypeName_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChargeTypeName_LocalizationId",
                schema: "public",
                table: "ServiceChargeTypeName",
                newName: "IX_ServCharTypeName_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChargeTypeName_Id",
                schema: "public",
                table: "ServiceChargeTypeName",
                newName: "IX_ServCharTypeName_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChargeType_Id",
                schema: "public",
                table: "ServiceChargeType",
                newName: "IX_ServCharType_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelWebPage",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelWebPage_WebPageId",
                schema: "public",
                table: "ServiceChannelWebPage",
                newName: "IX_SerChaWebPag_WebPageId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelWebPage_TypeId",
                schema: "public",
                table: "ServiceChannelWebPage",
                newName: "IX_SerChaWebPag_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelWebPage_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelWebPage",
                newName: "IX_SerChaWebPag_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelTypeName_TypeId",
                schema: "public",
                table: "ServiceChannelTypeName",
                newName: "IX_SerChaTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelTypeName_LocalizationId",
                schema: "public",
                table: "ServiceChannelTypeName",
                newName: "IX_SerChaTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelTypeName_Id",
                schema: "public",
                table: "ServiceChannelTypeName",
                newName: "IX_SerChaTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelType_Id",
                schema: "public",
                table: "ServiceChannelType",
                newName: "IX_SerChaTyp_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelTargetGroup_TargetGroupId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                newName: "IX_SerChaTarGro_TargetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelTargetGroup_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                newName: "IX_SerChaTarGro_ServiceChannelVersionedId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelServiceHours_ServiceHourTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "IX_SerChaSerHou_ServiceHourTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelServiceHours_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "IX_SerChaSerHou_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelServiceHours_Id",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "IX_SerChaSerHou_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                newName: "IX_SerChaSerCla_ServiceClassId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelServiceClass_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                newName: "IX_SerChaSerCla_ServiceChannelVersionedId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelPhone",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelPhone_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelPhone",
                newName: "IX_SerChaPho_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelPhone_PhoneId",
                schema: "public",
                table: "ServiceChannelPhone",
                newName: "IX_SerChaPho_PhoneId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelOntologyTerm_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                newName: "IX_SerChaOntTer_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelOntologyTerm_OntologyTermId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                newName: "IX_SerChaOntTer_OntologyTermId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelName",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelName_TypeId",
                schema: "public",
                table: "ServiceChannelName",
                newName: "IX_SerChaNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelName_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelName",
                newName: "IX_SerChaNam_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelName_LocalizationId",
                schema: "public",
                table: "ServiceChannelName",
                newName: "IX_SerChaNam_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelLanguage",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelLanguage_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelLanguage",
                newName: "IX_SerChaLan_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelLanguage_LanguageId",
                schema: "public",
                table: "ServiceChannelLanguage",
                newName: "IX_SerChaLan_LanguageId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelKeyword",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelKeyword_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelKeyword",
                newName: "IX_SerChaKey_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelKeyword_KeywordId",
                schema: "public",
                table: "ServiceChannelKeyword",
                newName: "IX_SerChaKey_KeywordId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelEmail",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelEmail_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelEmail",
                newName: "IX_SerChaEma_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelEmail_EmailId",
                schema: "public",
                table: "ServiceChannelEmail",
                newName: "IX_SerChaEma_EmailId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelDescription",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelDescription_TypeId",
                schema: "public",
                table: "ServiceChannelDescription",
                newName: "IX_SerChaDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelDescription_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelDescription",
                newName: "IX_SerChaDes_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelDescription_LocalizationId",
                schema: "public",
                table: "ServiceChannelDescription",
                newName: "IX_SerChaDes_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelAttachment",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelAttachment_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelAttachment",
                newName: "IX_SerChaAtt_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannelAttachment_AttachmentId",
                schema: "public",
                table: "ServiceChannelAttachment",
                newName: "IX_SerChaAtt_AttachmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannel_TypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                newName: "IX_SerCha_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannel_PublishingStatusId",
                schema: "public",
                table: "ServiceChannelVersioned",
                newName: "IX_SerCha_PublishingStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannel_OrganizationId",
                schema: "public",
                table: "ServiceChannelVersioned",
                newName: "IX_SerCha_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceChannel_Id",
                schema: "public",
                table: "ServiceChannelVersioned",
                newName: "IX_SerCha_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHourTypeName_TypeId",
                schema: "public",
                table: "ServiceHourTypeName",
                newName: "IX_SerHouTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHourTypeName_LocalizationId",
                schema: "public",
                table: "ServiceHourTypeName",
                newName: "IX_SerHouTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHourTypeName_Id",
                schema: "public",
                table: "ServiceHourTypeName",
                newName: "IX_SerHouTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHourType_Id",
                schema: "public",
                table: "ServiceHourType",
                newName: "IX_SerHouTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHoursAdditionalInformation_ServiceChannelServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                newName: "IX_SerHouAddInf_ServiceChannelServiceHoursId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceHoursAdditionalInformation_LocalizationId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                newName: "IX_SerHouAddInf_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicNotificationChannel_ServiceId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                newName: "IX_SerEleNotCha_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicNotificationChannel_LocalizationId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                newName: "IX_SerEleNotCha_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicNotificationChannel_Id",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                newName: "IX_SerEleNotCha_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicCommunicationChannel_ServiceId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                newName: "IX_SerEleComCha_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicCommunicationChannel_LocalizationId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                newName: "IX_SerEleComCha_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceElectronicCommunicationChannel_Id",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                newName: "IX_SerEleComCha_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceDescription",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceDescription_TypeId",
                schema: "public",
                table: "ServiceDescription",
                newName: "IX_SerDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceDescription_ServiceId",
                schema: "public",
                table: "ServiceDescription",
                newName: "IX_SerDes_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceDescription_LocalizationId",
                schema: "public",
                table: "ServiceDescription",
                newName: "IX_SerDes_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCoverageTypeName_TypeId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                newName: "IX_SerCovTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCoverageTypeName_LocalizationId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                newName: "IX_SerCovTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCoverageTypeName_Id",
                schema: "public",
                table: "ServiceCoverageTypeName",
                newName: "IX_SerCovTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCoverageType_Id",
                schema: "public",
                table: "ServiceCoverageType",
                newName: "IX_SerCovTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceClassName_ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                newName: "IX_SerClaNam_ServiceClassId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceClassName_LocalizationId",
                schema: "public",
                table: "ServiceClassName",
                newName: "IX_SerClaNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceClassName_Id",
                schema: "public",
                table: "ServiceClassName",
                newName: "IX_SerClaNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceClass_ParentId",
                schema: "public",
                table: "ServiceClass",
                newName: "IX_SerCla_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceClass_Id",
                schema: "public",
                table: "ServiceClass",
                newName: "IX_SerCla_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Service_VersioningId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_VersioningId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_TypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_StatutoryServiceGeneralDescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_ServiceCoverageTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_PublishingStatusId",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_PublishingStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_Id",
                schema: "public",
                table: "ServiceVersioned",
                newName: "IX_Ser_Id");

            migrationBuilder.RenameIndex(
                name: "IX_RoleTypeName_TypeId",
                schema: "public",
                table: "RoleTypeName",
                newName: "IX_RolTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleTypeName_LocalizationId",
                schema: "public",
                table: "RoleTypeName",
                newName: "IX_RolTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleTypeName_Id",
                schema: "public",
                table: "RoleTypeName",
                newName: "IX_RolTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_RoleType_Id",
                schema: "public",
                table: "RoleType",
                newName: "IX_RolTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingStatusTypeName_TypeId",
                schema: "public",
                table: "PublishingStatusTypeName",
                newName: "IX_PubStaTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingStatusTypeName_LocalizationId",
                schema: "public",
                table: "PublishingStatusTypeName",
                newName: "IX_PubStaTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingStatusTypeName_Id",
                schema: "public",
                table: "PublishingStatusTypeName",
                newName: "IX_PubStaTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PublishingStatusType_Id",
                schema: "public",
                table: "PublishingStatusType",
                newName: "IX_PubStaTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProvisionTypeName_TypeId",
                schema: "public",
                table: "ProvisionTypeName",
                newName: "IX_ProTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProvisionTypeName_LocalizationId",
                schema: "public",
                table: "ProvisionTypeName",
                newName: "IX_ProTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ProvisionTypeName_Id",
                schema: "public",
                table: "ProvisionTypeName",
                newName: "IX_ProTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProvisionType_Id",
                schema: "public",
                table: "ProvisionType",
                newName: "IX_ProTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrlTypeName_TypeId",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                newName: "IX_PriForChaUrlTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrlTypeName_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                newName: "IX_PriForChaUrlTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrlTypeName_Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                newName: "IX_PriForChaUrlTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrlType_Id",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                newName: "IX_PriForChaUrlTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrl_TypeId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                newName: "IX_PriForChaUrl_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrl_PrintableFormChannelId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                newName: "IX_PriForChaUrl_PrintableFormChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrl_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                newName: "IX_PriForChaUrl_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannelUrl_Id",
                schema: "public",
                table: "PrintableFormChannelUrl",
                newName: "IX_PriForChaUrl_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "PrintableFormChannel",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannel_ServiceChannelId",
                schema: "public",
                table: "PrintableFormChannel",
                newName: "IX_PriForCha_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannel_Id",
                schema: "public",
                table: "PrintableFormChannel",
                newName: "IX_PriForCha_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PrintableFormChannel_DeliveryAddressId",
                schema: "public",
                table: "PrintableFormChannel",
                newName: "IX_PriForCha_DeliveryAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_PostalCode_MunicipalityId",
                schema: "public",
                table: "PostalCode",
                newName: "IX_PosCod_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_PostalCode_Id",
                schema: "public",
                table: "PostalCode",
                newName: "IX_PosCod_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumberTypeName_TypeId",
                schema: "public",
                table: "PhoneNumberTypeName",
                newName: "IX_PhoNumTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumberTypeName_LocalizationId",
                schema: "public",
                table: "PhoneNumberTypeName",
                newName: "IX_PhoNumTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumberTypeName_Id",
                schema: "public",
                table: "PhoneNumberTypeName",
                newName: "IX_PhoNumTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhoneNumberType_Id",
                schema: "public",
                table: "PhoneNumberType",
                newName: "IX_PhoNumTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Phone_TypeId",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Phone_ServiceChargeTypeId",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_ServiceChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Phone_LocalizationId",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Phone_Id",
                schema: "public",
                table: "Phone",
                newName: "IX_Pho_Id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationWebPage",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationWebPage_WebPageId",
                schema: "public",
                table: "OrganizationWebPage",
                newName: "IX_OrgWebPag_WebPageId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationWebPage_TypeId",
                schema: "public",
                table: "OrganizationWebPage",
                newName: "IX_OrgWebPag_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationWebPage_OrganizationId",
                schema: "public",
                table: "OrganizationWebPage",
                newName: "IX_OrgWebPag_OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTypeName_TypeId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "IX_OrgTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTypeName_LocalizationId",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "IX_OrgTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationTypeName_Id",
                schema: "public",
                table: "OrganizationTypeName",
                newName: "IX_OrgTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationType_Id",
                schema: "public",
                table: "OrganizationType",
                newName: "IX_OrgTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceWebPage_WebPageId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                newName: "IX_OrgSerWebPag_WebPageId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceWebPage_TypeId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                newName: "IX_OrgSerWebPag_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceWebPage_OrganizationServiceId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                newName: "IX_OrgSerWebPag_OrganizationServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceAdditionalInformation_OrganizationServiceId",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                newName: "IX_OrgSerAddInf_OrganizationServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceAdditionalInformation_LocalizationId",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                newName: "IX_OrgSerAddInf_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationServiceAdditionalInformation_Id",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                newName: "IX_OrgSerAddInf_Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "public",
                table: "OrganizationService",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationService_ServiceId",
                schema: "public",
                table: "OrganizationService",
                newName: "IX_OrgSer_ServiceVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationService_RoleTypeId",
                schema: "public",
                table: "OrganizationService",
                newName: "IX_OrgSer_RoleTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationService_ProvisionTypeId",
                schema: "public",
                table: "OrganizationService",
                newName: "IX_OrgSer_ProvisionTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationService_OrganizationId",
                schema: "public",
                table: "OrganizationService",
                newName: "IX_OrgSer_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationService_Id",
                schema: "public",
                table: "OrganizationService",
                newName: "IX_OrgSer_Id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationPhone",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPhone_PhoneId",
                schema: "public",
                table: "OrganizationPhone",
                newName: "IX_OrgPho_PhoneId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPhone_OrganizationId",
                schema: "public",
                table: "OrganizationPhone",
                newName: "IX_OrgPho_OrganizationVersionedId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationName",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationName_TypeId",
                schema: "public",
                table: "OrganizationName",
                newName: "IX_OrgNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationName_OrganizationId",
                schema: "public",
                table: "OrganizationName",
                newName: "IX_OrgNam_OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationName_LocalizationId",
                schema: "public",
                table: "OrganizationName",
                newName: "IX_OrgNam_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationEmail",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationEmail_OrganizationId",
                schema: "public",
                table: "OrganizationEmail",
                newName: "IX_OrgEma_OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationEmail_EmailId",
                schema: "public",
                table: "OrganizationEmail",
                newName: "IX_OrgEma_EmailId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationDescription",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationDescription_TypeId",
                schema: "public",
                table: "OrganizationDescription",
                newName: "IX_OrgDes_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationDescription_OrganizationId",
                schema: "public",
                table: "OrganizationDescription",
                newName: "IX_OrgDes_OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationDescription_LocalizationId",
                schema: "public",
                table: "OrganizationDescription",
                newName: "IX_OrgDes_LocalizationId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAddress_TypeId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "IX_OrgAdd_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAddress_OrganizationId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "IX_OrgAdd_OrganizationVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAddress_AddressId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "IX_OrgAdd_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_TypeId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_PublishingStatusId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_PublishingStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_ParentId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_Oid",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_Oid");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_MunicipalityId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_Id",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_DisplayNameTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_BusinessId",
                schema: "public",
                table: "OrganizationVersioned",
                newName: "IX_Org_BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTermParent_ParentId",
                schema: "public",
                table: "OntologyTermParent",
                newName: "IX_OntTerPar_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTermParent_ChildId",
                schema: "public",
                table: "OntologyTermParent",
                newName: "IX_OntTerPar_ChildId");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTermName_OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                newName: "IX_OntTerNam_OntologyTermId");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTermName_LocalizationId",
                schema: "public",
                table: "OntologyTermName",
                newName: "IX_OntTerNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTermName_Id",
                schema: "public",
                table: "OntologyTermName",
                newName: "IX_OntTerNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_OntologyTerm_Id",
                schema: "public",
                table: "OntologyTerm",
                newName: "IX_OntTer_Id");

            migrationBuilder.RenameIndex(
                name: "IX_NameTypeName_TypeId",
                schema: "public",
                table: "NameTypeName",
                newName: "IX_NamTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_NameTypeName_LocalizationId",
                schema: "public",
                table: "NameTypeName",
                newName: "IX_NamTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_NameTypeName_Id",
                schema: "public",
                table: "NameTypeName",
                newName: "IX_NamTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_NameType_Id",
                schema: "public",
                table: "NameType",
                newName: "IX_NamTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_MunicipalityName_MunicipalityId",
                schema: "public",
                table: "MunicipalityName",
                newName: "IX_MunNam_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_MunicipalityName_LocalizationId",
                schema: "public",
                table: "MunicipalityName",
                newName: "IX_MunNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Municipality_Id",
                schema: "public",
                table: "Municipality",
                newName: "IX_Mun_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Locking_Id",
                schema: "public",
                table: "Locking",
                newName: "IX_Loc_Id");

            migrationBuilder.RenameIndex(
                name: "IX_LifeEventName_LocalizationId",
                schema: "public",
                table: "LifeEventName",
                newName: "IX_LifEveNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_LifeEventName_LifeEventId",
                schema: "public",
                table: "LifeEventName",
                newName: "IX_LifEveNam_LifeEventId");

            migrationBuilder.RenameIndex(
                name: "IX_LifeEventName_Id",
                schema: "public",
                table: "LifeEventName",
                newName: "IX_LifEveNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_LifeEvent_ParentId",
                schema: "public",
                table: "LifeEvent",
                newName: "IX_LifEve_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_LifeEvent_Id",
                schema: "public",
                table: "LifeEvent",
                newName: "IX_LifEve_Id");

            migrationBuilder.RenameIndex(
                name: "IX_LawWebPage_WebPageId",
                schema: "public",
                table: "LawWebPage",
                newName: "IX_LawWebPag_WebPageId");

            migrationBuilder.RenameIndex(
                name: "IX_LawWebPage_LawId",
                schema: "public",
                table: "LawWebPage",
                newName: "IX_LawWebPag_LawId");

            migrationBuilder.RenameIndex(
                name: "IX_LawName_LocalizationId",
                schema: "public",
                table: "LawName",
                newName: "IX_LawNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_LawName_LawId",
                schema: "public",
                table: "LawName",
                newName: "IX_LawNam_LawId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageName_LocalizationId",
                schema: "public",
                table: "LanguageName",
                newName: "IX_LanNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageName_LanguageId",
                schema: "public",
                table: "LanguageName",
                newName: "IX_LanNam_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageName_Id",
                schema: "public",
                table: "LanguageName",
                newName: "IX_LanNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Language_Id",
                schema: "public",
                table: "Language",
                newName: "IX_Lan_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Keyword_LocalizationId",
                schema: "public",
                table: "Keyword",
                newName: "IX_Key_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Keyword_Id",
                schema: "public",
                table: "Keyword",
                newName: "IX_Key_Id");

            migrationBuilder.RenameIndex(
                name: "IX_IndustrialClassName_LocalizationId",
                schema: "public",
                table: "IndustrialClassName",
                newName: "IX_IndClaNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_IndustrialClassName_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                newName: "IX_IndClaNam_IndustrialClassId");

            migrationBuilder.RenameIndex(
                name: "IX_IndustrialClassName_Id",
                schema: "public",
                table: "IndustrialClassName",
                newName: "IX_IndClaNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_IndustrialClass_ParentId",
                schema: "public",
                table: "IndustrialClass",
                newName: "IX_IndCla_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_IndustrialClass_Id",
                schema: "public",
                table: "IndustrialClass",
                newName: "IX_IndCla_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FormTypeName_TypeId",
                schema: "public",
                table: "FormTypeName",
                newName: "IX_ForTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_FormTypeName_LocalizationId",
                schema: "public",
                table: "FormTypeName",
                newName: "IX_ForTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_FormTypeName_Id",
                schema: "public",
                table: "FormTypeName",
                newName: "IX_ForTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FormType_Id",
                schema: "public",
                table: "FormType",
                newName: "IX_ForTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Form_TypeId",
                schema: "public",
                table: "Form",
                newName: "IX_For_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Form_LocalizationId",
                schema: "public",
                table: "Form",
                newName: "IX_For_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Form_Id",
                schema: "public",
                table: "Form",
                newName: "IX_For_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalSource_Id",
                schema: "public",
                table: "ExternalSource",
                newName: "IX_ExtSou_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ExceptionHoursStatusTypeName_TypeId",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                newName: "IX_ExcHouStaTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExceptionHoursStatusTypeName_LocalizationId",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                newName: "IX_ExcHouStaTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ExceptionHoursStatusTypeName_Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                newName: "IX_ExcHouStaTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ExceptionHoursStatusType_Id",
                schema: "public",
                table: "ExceptionHoursStatusType",
                newName: "IX_ExcHouStaTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Email_LocalizationId",
                schema: "public",
                table: "Email",
                newName: "IX_Ema_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Email_Id",
                schema: "public",
                table: "Email",
                newName: "IX_Ema_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ElectronicChannelUrl_LocalizationId",
                schema: "public",
                table: "ElectronicChannelUrl",
                newName: "IX_EleChaUrl_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ElectronicChannelUrl_ElectronicChannelId",
                schema: "public",
                table: "ElectronicChannelUrl",
                newName: "IX_EleChaUrl_ElectronicChannelId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ElectronicChannel",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ElectronicChannel_ServiceChannelId",
                schema: "public",
                table: "ElectronicChannel",
                newName: "IX_EleCha_ServiceChannelVersionedId");

            migrationBuilder.RenameIndex(
                name: "IX_ElectronicChannel_Id",
                schema: "public",
                table: "ElectronicChannel",
                newName: "IX_EleCha_Id");

            migrationBuilder.RenameIndex(
                name: "IX_DigitalAuthorizationName_LocalizationId",
                schema: "public",
                table: "DigitalAuthorizationName",
                newName: "IX_DigAutNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_DigitalAuthorizationName_Id",
                schema: "public",
                table: "DigitalAuthorizationName",
                newName: "IX_DigAutNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_DigitalAuthorizationName_DigitalAuthorizationId",
                schema: "public",
                table: "DigitalAuthorizationName",
                newName: "IX_DigAutNam_DigitalAuthorizationId");

            migrationBuilder.RenameIndex(
                name: "IX_DigitalAuthorization_ParentId",
                schema: "public",
                table: "DigitalAuthorization",
                newName: "IX_DigAut_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_DigitalAuthorization_Id",
                schema: "public",
                table: "DigitalAuthorization",
                newName: "IX_DigAut_Id");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionTypeName_TypeId",
                schema: "public",
                table: "DescriptionTypeName",
                newName: "IX_DesTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionTypeName_LocalizationId",
                schema: "public",
                table: "DescriptionTypeName",
                newName: "IX_DesTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionTypeName_Id",
                schema: "public",
                table: "DescriptionTypeName",
                newName: "IX_DesTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_DescriptionType_Id",
                schema: "public",
                table: "DescriptionType",
                newName: "IX_DesTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_DailyOpeningTime_OpeningHourId_DayFrom_IsExtra",
                schema: "public",
                table: "DailyOpeningTime",
                newName: "IX_DaiOpeTim_OpeningHourId_DayFrom_IsExtra");

            migrationBuilder.RenameIndex(
                name: "IX_CountryName_LocalizationId",
                schema: "public",
                table: "CountryName",
                newName: "IX_CouNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_CountryName_Id",
                schema: "public",
                table: "CountryName",
                newName: "IX_CouNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_CountryName_CountryId",
                schema: "public",
                table: "CountryName",
                newName: "IX_CouNam_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Country_Id",
                schema: "public",
                table: "Country",
                newName: "IX_Cou_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Business_Id",
                schema: "public",
                table: "Business",
                newName: "IX_Bus_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AttachmentTypeName_TypeId",
                schema: "public",
                table: "AttachmentTypeName",
                newName: "IX_AttTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AttachmentTypeName_LocalizationId",
                schema: "public",
                table: "AttachmentTypeName",
                newName: "IX_AttTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AttachmentTypeName_Id",
                schema: "public",
                table: "AttachmentTypeName",
                newName: "IX_AttTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AttachmentType_Id",
                schema: "public",
                table: "AttachmentType",
                newName: "IX_AttTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_TypeId",
                schema: "public",
                table: "Attachment",
                newName: "IX_Att_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_LocalizationId",
                schema: "public",
                table: "Attachment",
                newName: "IX_Att_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_Id",
                schema: "public",
                table: "Attachment",
                newName: "IX_Att_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AddressTypeName_TypeId",
                schema: "public",
                table: "AddressTypeName",
                newName: "IX_AddTypNam_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressTypeName_LocalizationId",
                schema: "public",
                table: "AddressTypeName",
                newName: "IX_AddTypNam_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressTypeName_Id",
                schema: "public",
                table: "AddressTypeName",
                newName: "IX_AddTypNam_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AddressType_Id",
                schema: "public",
                table: "AddressType",
                newName: "IX_AddTyp_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AddressAdditionalInformation_LocalizationId",
                schema: "public",
                table: "AddressAdditionalInformation",
                newName: "IX_AddAddInf_LocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AddressAdditionalInformation_AddressId",
                schema: "public",
                table: "AddressAdditionalInformation",
                newName: "IX_AddAddInf_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_PostalCodeId",
                schema: "public",
                table: "Address",
                newName: "IX_Add_PostalCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_MunicipalityId",
                schema: "public",
                table: "Address",
                newName: "IX_Add_MunicipalityId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_Id",
                schema: "public",
                table: "Address",
                newName: "IX_Add_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CountryId",
                schema: "public",
                table: "Address",
                newName: "IX_Add_CountryId");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "public",
                table: "WebPage",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "WebPage",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "public",
                table: "UserOrganization",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "public",
                table: "StreetName",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublishingStatusId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnificRootId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VersioningId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Override",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "ServiceName",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "public",
                table: "ServiceLanguage",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "ServiceChannelName",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnificRootId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VersioningId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "UnificRootId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "public",
                table: "PrintableFormChannelUrl",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "PostalCode",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                schema: "public",
                table: "Phone",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChargeDescription",
                schema: "public",
                table: "Phone",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInformation",
                schema: "public",
                table: "Phone",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrefixNumberId",
                schema: "public",
                table: "Phone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DialCode",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CountryId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialCode_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "public",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(File.ReadAllText(@"SqlMigrations\PTV_1_4\Fixed\5DialCodeMigration.sql"));

            migrationBuilder.DropColumn(
               name: "PrefixNumber",
               schema: "public",
               table: "Phone");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTypeId",
                schema: "public",
                table: "OrganizationType",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "OrganizationName",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "OrganizationDescription",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Oid",
                schema: "public",
                table: "OrganizationVersioned",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnificRootId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));


            migrationBuilder.Sql(File.ReadAllText(@"SqlMigrations\PTV_1_4\Fixed\2AssignRootNodesToVersions.sql"));

            migrationBuilder.AddColumn<Guid>(
                name: "VersioningId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "LawName",
                maxLength: 110,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "public",
                table: "Email",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Email",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "public",
                table: "Business",
                maxLength: 9,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "public",
                table: "Attachment",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "public",
                table: "Attachment",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "public",
                table: "Attachment",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "public",
                table: "AddressAdditionalInformation",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StreetNumber",
                schema: "public",
                table: "Address",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CoordinateType",
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
                    table.PrimaryKey("PK_CoordinateType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExactMatch",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExactMatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionLanguageAvailability",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionVersionedId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionLanguageAvailability", x => new { x.StatutoryServiceGeneralDescriptionVersionedId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionLanguageAvailability_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionLanguageAvailability_PublishingStatusType_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionLanguageAvailability_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                        column: x => x.StatutoryServiceGeneralDescriptionVersionedId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationLanguageAvailability",
                schema: "public",
                columns: table => new
                {
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationLanguageAvailability", x => new { x.OrganizationVersionedId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_OrganizationLanguageAvailability_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationLanguageAvailability_Organization_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationLanguageAvailability_PublishingStatusType_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostalCodeName",
                schema: "public",
                columns: table => new
                {
                    PostalCodeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCodeName", x => new { x.PostalCodeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_PostalCodeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostalCodeName_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelLanguageAvailability",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelLanguageAvailability", x => new { x.ServiceChannelVersionedId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ServiceChannelLanguageAvailability_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelLanguageAvailability_ServiceChannel_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelLanguageAvailability_PublishingStatusType_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ServiceLanguageAvailability",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLanguageAvailability", x => new { x.ServiceVersionedId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ServiceLanguageAvailability_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLanguageAvailability_Service_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLanguageAvailability_PublishingStatusType_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLaw",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    LawId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLaw", x => new { x.ServiceVersionedId, x.LawId });
                    table.ForeignKey(
                        name: "FK_ServiceLaw_Law_LawId",
                        column: x => x.LawId,
                        principalSchema: "public",
                        principalTable: "Law",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLaw_Service_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coordinate",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    CoordinateState = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longtitude = table.Column<double>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordinate_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coordinate_CoordinateType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "CoordinateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoordinateTypeName",
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
                    table.PrimaryKey("PK_CoordinateTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoordinateTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoordinateTypeName_CoordinateType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "CoordinateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTermExactMatch",
                schema: "public",
                columns: table => new
                {
                    OntologyTermId = table.Column<Guid>(nullable: false),
                    ExactMatchId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTermExactMatch", x => new { x.OntologyTermId, x.ExactMatchId });
                    table.ForeignKey(
                        name: "FK_OntologyTermExactMatch_ExactMatch_ExactMatchId",
                        column: x => x.ExactMatchId,
                        principalSchema: "public",
                        principalTable: "ExactMatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OntologyTermExactMatch_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDes_PublishingStatusId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDes_UnificRootId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "UnificRootId");

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDes_VersioningId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "VersioningId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCha_UnificRootId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "UnificRootId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCha_VersioningId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "VersioningId");

            migrationBuilder.CreateIndex(
                name: "IX_Ser_UnificRootId",
                schema: "public",
                table: "ServiceVersioned",
                column: "UnificRootId");

            migrationBuilder.CreateIndex(
                name: "IX_Pho_PrefixNumberId",
                schema: "public",
                table: "Phone",
                column: "PrefixNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgTyp_ParentTypeId",
                schema: "public",
                table: "OrganizationType",
                column: "ParentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Org_UnificRootId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "UnificRootId");

            migrationBuilder.CreateIndex(
                name: "IX_Org_VersioningId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "VersioningId");

            migrationBuilder.CreateIndex(
                name: "IX_Coo_AddressId",
                schema: "public",
                table: "Coordinate",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Coo_Id",
                schema: "public",
                table: "Coordinate",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Coo_TypeId",
                schema: "public",
                table: "Coordinate",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CooTyp_Id",
                schema: "public",
                table: "CoordinateType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CooTypNam_Id",
                schema: "public",
                table: "CoordinateTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CooTypNam_LocalizationId",
                schema: "public",
                table: "CoordinateTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CooTypNam_TypeId",
                schema: "public",
                table: "CoordinateTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaCod_CountryId",
                schema: "public",
                table: "DialCode",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaCod_Id",
                schema: "public",
                table: "DialCode",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExaMat_Id",
                schema: "public",
                table: "ExactMatch",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesLanAva_LanguageId",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesLanAva_StatusId",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesLanAva_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                column: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerExaMat_ExactMatchId",
                schema: "public",
                table: "OntologyTermExactMatch",
                column: "ExactMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerExaMat_OntologyTermId",
                schema: "public",
                table: "OntologyTermExactMatch",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgLanAva_LanguageId",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgLanAva_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                column: "OrganizationVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgLanAva_StatusId",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgRooNod_Id",
                schema: "public",
                table: "Organization",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PosCodNam_LocalizationId",
                schema: "public",
                table: "PostalCodeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCodNam_PostalCodeId",
                schema: "public",
                table: "PostalCodeName",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaIde_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaIde_PrintableFormChannelId",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                column: "PrintableFormChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaRec_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelReceiver",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaRec_PrintableFormChannelId",
                schema: "public",
                table: "PrintableFormChannelReceiver",
                column: "PrintableFormChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaLanAva_LanguageId",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaLanAva_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaLanAva_StatusId",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaRooNod_Id",
                schema: "public",
                table: "ServiceChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerLanAva_LanguageId",
                schema: "public",
                table: "ServiceLanguageAvailability",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SerLanAva_ServiceVersionedId",
                schema: "public",
                table: "ServiceLanguageAvailability",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerLanAva_StatusId",
                schema: "public",
                table: "ServiceLanguageAvailability",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SerLaw_LawId",
                schema: "public",
                table: "ServiceLaw",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "IX_SerLaw_ServiceVersionedId",
                schema: "public",
                table: "ServiceLaw",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerRooNod_Id",
                schema: "public",
                table: "Service",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDesRooNod_Id",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectronicChannel_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ElectronicChannel",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Organization_ParentId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "ParentId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Organization_UnificRootId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "UnificRootId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Versioning_VersioningId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "VersioningId",
                principalSchema: "public",
                principalTable: "Versioning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAddress_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationAddress",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationDescription_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationDescription",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationEmail_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationEmail",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationName_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationName",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPhone_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationPhone",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
    name: "FK_OrganizationService_Organization_OrganizationId",
    schema: "public",
    table: "OrganizationService");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationService_Organization_OrganizationId",
                schema: "public",
                table: "OrganizationService",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationService_Service_ServiceVersionedId",
                schema: "public",
                table: "OrganizationService",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_OrganizationWebPage_Organization_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "OrganizationVersionedId",
                principalSchema: "public",
                principalTable: "OrganizationVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Phone_DialCode_PrefixNumberId",
                schema: "public",
                table: "Phone",
                column: "PrefixNumberId",
                principalSchema: "public",
                principalTable: "DialCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrintableFormChannel_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "PrintableFormChannel",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "ServiceVersioned",
                column: "StatutoryServiceGeneralDescriptionId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceType_TypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Service_UnificRootId",
                schema: "public",
                table: "ServiceVersioned",
                column: "UnificRootId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDescription_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceDescription",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceElectronicCommunicationChannel_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceElectronicNotificationChannel_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannel_Organization_OrganizationId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannel_ServiceChannel_UnificRootId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "UnificRootId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannel_Versioning_VersioningId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "VersioningId",
                principalSchema: "public",
                principalTable: "Versioning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelAttachment_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelAttachment",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelDescription_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelDescription",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelEmail_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelEmail",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelKeyword_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelKeyword",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelLanguage_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelLanguage",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelName_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelName",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelOntologyTerm_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelPhone_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelServiceClass_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelServiceHours_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelTargetGroup_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelWebPage_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceIndustrialClass_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceIndustrialClass",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceKeyword_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceKeyword",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLanguage_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceLanguage",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLifeEvent_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceLifeEvent",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLocationChannel_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceLocationChannel",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceMunicipality_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceName_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceName",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOntologyTerm_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceOntologyTerm",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequirement_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceRequirement",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceClass_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceServiceClass",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTargetGroup_Service_ServiceVersionedId",
                schema: "public",
                table: "ServiceTargetGroup",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceDescription_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceDescription",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescription_PublishingStatusType_PublishingStatusId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "PublishingStatusId",
                principalSchema: "public",
                principalTable: "PublishingStatusType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescription_UnificRootId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "UnificRootId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescription_Versioning_VersioningId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "VersioningId",
                principalSchema: "public",
                principalTable: "Versioning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceIndustrialClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceLanguage_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceLaw_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceLaw",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceLifeEvent_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceName_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceName",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceOntologyTerm_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceRequirement_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceServiceClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceTargetGroup_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebpageChannel_ServiceChannel_ServiceChannelVersionedId",
                schema: "public",
                table: "WebpageChannel",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            AddSqlScripts140Post(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
