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

namespace PTV.Database.Migrations.Migrations.Base
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''");

            migrationBuilder.CreateTable(
                name: "AddressType",
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
                    table.PrimaryKey("PK_AddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentType",
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
                    table.PrimaryKey("PK_AttachmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Business",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionType",
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
                    table.PrimaryKey("PK_DescriptionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionHoursStatusType",
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
                    table.PrimaryKey("PK_ExceptionHoursStatusType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalSource",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ObjectType = table.Column<string>(nullable: true),
                    PTVId = table.Column<Guid>(nullable: false),
                    RelationId = table.Column<string>(nullable: true),
                    SourceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormType",
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
                    table.PrimaryKey("PK_FormType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndustrialClass",
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
                    table.PrimaryKey("PK_IndustrialClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustrialClass_IndustrialClass_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "IndustrialClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Keyword",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsForData = table.Column<bool>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifeEvent",
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
                    table.PrimaryKey("PK_LifeEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeEvent_LifeEvent_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "LifeEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipality",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NameType",
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
                    table.PrimaryKey("PK_NameType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTerm",
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
                    ParentUri = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTerm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpeningHoursType",
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
                    table.PrimaryKey("PK_OpeningHoursType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationType",
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
                    table.PrimaryKey("PK_OrganizationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneDescriptionType",
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
                    table.PrimaryKey("PK_PhoneDescriptionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumberType",
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
                    table.PrimaryKey("PK_PhoneNumberType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostalCode",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PostOffice = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannelUrlType",
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
                    table.PrimaryKey("PK_PrintableFormChannelUrlType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvisionType",
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
                    table.PrimaryKey("PK_ProvisionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishingStatusType",
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
                    table.PrimaryKey("PK_PublishingStatusType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleType",
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
                    table.PrimaryKey("PK_RoleType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAdditionalInformationType",
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
                    table.PrimaryKey("PK_ServiceAdditionalInformationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceClass",
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
                    table.PrimaryKey("PK_ServiceClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceClass_ServiceClass_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "ServiceHourType",
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
                    table.PrimaryKey("PK_ServiceHourType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelType",
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
                    table.PrimaryKey("PK_ServiceChannelType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChargeType",
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
                    table.PrimaryKey("PK_ServiceChargeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
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
                    table.PrimaryKey("PK_ServiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceGeneralDescription",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceGeneralDescription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetGroup",
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
                    table.PrimaryKey("PK_TargetGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetGroup_TargetGroup_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "TargetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganization",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RelationId = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebPageType",
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
                    table.PrimaryKey("PK_WebPageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressTypeName",
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
                    table.PrimaryKey("PK_AddressTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressTypeName_AddressType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AddressType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachment_AttachmentType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AttachmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentTypeName",
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
                    table.PrimaryKey("PK_AttachmentTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachmentTypeName_AttachmentType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AttachmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountryName_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "public",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionTypeName",
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
                    table.PrimaryKey("PK_DescriptionTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DescriptionTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DescriptionTypeName_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionHoursStatusTypeName",
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
                    table.PrimaryKey("PK_ExceptionHoursStatusTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExceptionHoursStatusTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExceptionHoursStatusTypeName_ExceptionHoursStatusType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ExceptionHoursStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Form",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Form_FormType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "FormType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormTypeName",
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
                    table.PrimaryKey("PK_FormTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormTypeName_FormType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "FormType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndustrialClassName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IndustrialClassId = table.Column<Guid>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrialClassName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustrialClassName_IndustrialClass_IndustrialClassId",
                        column: x => x.IndustrialClassId,
                        principalSchema: "public",
                        principalTable: "IndustrialClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndustrialClassName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LanguageId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageName_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebPage",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebPage_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifeEventName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LifeEventId = table.Column<Guid>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeEventName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeEventName_LifeEvent_LifeEventId",
                        column: x => x.LifeEventId,
                        principalSchema: "public",
                        principalTable: "LifeEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LifeEventName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NameTypeName",
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
                    table.PrimaryKey("PK_NameTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NameTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NameTypeName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTermName",
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
                    OntologyTermId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTermName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OntologyTermName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OntologyTermName_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTermParent",
                schema: "public",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(nullable: false),
                    ChildId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTermParent", x => new {x.ParentId, x.ChildId});
                    table.ForeignKey(
                        name: "FK_OntologyTermParent_OntologyTerm_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OntologyTermParent_OntologyTerm_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningHoursTypeName",
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
                    table.PrimaryKey("PK_OpeningHoursTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningHoursTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpeningHoursTypeName_OpeningHoursType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "OpeningHoursType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTypeName",
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
                    table.PrimaryKey("PK_OrganizationTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTypeName_OrganizationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "OrganizationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumberTypeName",
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
                    table.PrimaryKey("PK_PhoneNumberTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneNumberTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneNumberTypeName_PhoneNumberType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PhoneNumberType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    MunicipalityId = table.Column<Guid>(nullable: true),
                    PostOfficeBox = table.Column<string>(nullable: true),
                    PostalCodeId = table.Column<Guid>(nullable: false),
                    Qualifier = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "public",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Address_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannelUrlTypeName",
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
                    table.PrimaryKey("PK_PrintableFormChannelUrlTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelUrlTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelUrlTypeName_PrintableFormChannelUrlType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannelUrlType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProvisionTypeName",
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
                    table.PrimaryKey("PK_ProvisionTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisionTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProvisionTypeName_ProvisionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ProvisionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BusinessId = table.Column<Guid>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DisplayNameTypeId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    MunicipalityId = table.Column<Guid>(nullable: true),
                    Oid = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    PublishingStatusId = table.Column<Guid>(nullable: true),
                    StreetAddressAsPostalAddress = table.Column<bool>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_Business_BusinessId",
                        column: x => x.BusinessId,
                        principalSchema: "public",
                        principalTable: "Business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_NameType_DisplayNameTypeId",
                        column: x => x.DisplayNameTypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Organization_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_Organization_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_PublishingStatusType_PublishingStatusId",
                        column: x => x.PublishingStatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_OrganizationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "OrganizationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishingStatusTypeName",
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
                    table.PrimaryKey("PK_PublishingStatusTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishingStatusTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingStatusTypeName_PublishingStatusType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleTypeName",
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
                    table.PrimaryKey("PK_RoleTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleTypeName_RoleType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "RoleType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceClassName",
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
                    ServiceClassId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceClassName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceClassName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceClassName_ServiceClass_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "ServiceHourTypeName",
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
                    table.PrimaryKey("PK_ServiceHourTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHourTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceHourTypeName_ServiceHourType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceHourType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelTypeName",
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
                    table.PrimaryKey("PK_ServiceChannelTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelTypeName_ServiceChannelType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChargeTypeName",
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
                    table.PrimaryKey("PK_ServiceChargeTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChargeTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChargeTypeName_ServiceChargeType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypeName",
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
                    table.PrimaryKey("PK_ServiceTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTypeName_ServiceType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ElectronicCommunication = table.Column<bool>(nullable: false),
                    ElectronicNotification = table.Column<bool>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PublishingStatusId = table.Column<Guid>(nullable: true),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: true),
                    ServiceCoverageTypeId = table.Column<Guid>(nullable: true),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_PublishingStatusType_PublishingStatusId",
                        column: x => x.PublishingStatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_ServiceCoverageType_ServiceCoverageTypeId",
                        column: x => x.ServiceCoverageTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceCoverageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Service_ServiceType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceDescription",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceDescription",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceDescription_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatutoryServiceDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceLanguage",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceLanguage",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.LanguageId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceLanguage_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceLanguage_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceLifeEvent",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    LifeEventId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceLifeEvent",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.LifeEventId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceLifeEvent_LifeEvent_LifeEventId",
                        column: x => x.LifeEventId,
                        principalSchema: "public",
                        principalTable: "LifeEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceLifeEvent_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceName",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceName",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceName_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatutoryServiceName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceOntologyTerm",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    OntologyTermId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceOntologyTerm",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.OntologyTermId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceOntologyTerm_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceOntologyTerm_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceServiceClass",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    ServiceClassId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceServiceClass",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.ServiceClassId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceServiceClass_ServiceClass_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceServiceClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceTargetGroup",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    TargetGroupId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceTargetGroup",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.TargetGroupId});
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceTargetGroup_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatutoryServiceTargetGroup_TargetGroup_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalSchema: "public",
                        principalTable: "TargetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetGroupName",
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
                    TargetGroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetGroupName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetGroupName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetGroupName_TargetGroup_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalSchema: "public",
                        principalTable: "TargetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebPageTypeName",
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
                    table.PrimaryKey("PK_WebPageTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebPageTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebPageTypeName_WebPageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WebPageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressAdditionalInformation",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressAdditionalInformation", x => new {x.AddressId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_AddressAdditionalInformation_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressAdditionalInformation_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreetAddress",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreetAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreetAddress_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAddress",
                schema: "public",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAddress", x => new {x.OrganizationId, x.AddressId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_OrganizationAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationAddress_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationAddress_AddressType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AddressType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationDescription",
                schema: "public",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDescription", x => new {x.OrganizationId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_OrganizationDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDescription_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationEmail",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationEmail_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationName",
                schema: "public",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationName", x => new {x.OrganizationId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_OrganizationName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationName_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPhone",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    PrefixNumber = table.Column<string>(nullable: true),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPhone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPhone_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationPhone_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationPhone_PhoneNumberType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PhoneNumberType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationWebPage",
                schema: "public",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationWebPage", x => new {x.OrganizationId, x.WebPageId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_OrganizationWebPage_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationWebPage_WebPageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WebPageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Charge = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    PublishingStatusId = table.Column<Guid>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannel_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannel_PublishingStatusType_PublishingStatusId",
                        column: x => x.PublishingStatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceChannel_ServiceChannelType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationService",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: true),
                    ProvisionTypeId = table.Column<Guid>(nullable: true),
                    RoleTypeId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationService_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationService_ProvisionType_ProvisionTypeId",
                        column: x => x.ProvisionTypeId,
                        principalSchema: "public",
                        principalTable: "ProvisionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationService_RoleType_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalSchema: "public",
                        principalTable: "RoleType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAdditionalInformation",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAdditionalInformation",
                        x => new {x.ServiceId, x.LocalizationId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_ServiceAdditionalInformation_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAdditionalInformation_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAdditionalInformation_ServiceAdditionalInformationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceAdditionalInformationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDescription",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDescription", x => new {x.ServiceId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDescription_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceElectronicCommunicationChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ElectronicCommunicationChannel = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceElectronicCommunicationChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceElectronicCommunicationChannel_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceElectronicCommunicationChannel_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceElectronicNotificationChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ElectronicNotificationChannel = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceElectronicNotificationChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceElectronicNotificationChannel_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceElectronicNotificationChannel_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceIndustrialClass",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    IndustrialClassId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIndustrialClass", x => new {x.ServiceId, x.IndustrialClassId});
                    table.ForeignKey(
                        name: "FK_ServiceIndustrialClass_IndustrialClass_IndustrialClassId",
                        column: x => x.IndustrialClassId,
                        principalSchema: "public",
                        principalTable: "IndustrialClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceIndustrialClass_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceKeyword",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    KeywordId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceKeyword", x => new {x.ServiceId, x.KeywordId});
                    table.ForeignKey(
                        name: "FK_ServiceKeyword_Keyword_KeywordId",
                        column: x => x.KeywordId,
                        principalSchema: "public",
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceKeyword_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLanguage",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLanguage", x => new {x.ServiceId, x.LanguageId});
                    table.ForeignKey(
                        name: "FK_ServiceLanguage_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLanguage_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLifeEvent",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    LifeEventId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLifeEvent", x => new {x.ServiceId, x.LifeEventId});
                    table.ForeignKey(
                        name: "FK_ServiceLifeEvent_LifeEvent_LifeEventId",
                        column: x => x.LifeEventId,
                        principalSchema: "public",
                        principalTable: "LifeEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLifeEvent_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMunicipality",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMunicipality", x => new {x.ServiceId, x.MunicipalityId});
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceName",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceName", x => new {x.ServiceId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceName_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOntologyTerm",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    OntologyTermId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOntologyTerm", x => new {x.ServiceId, x.OntologyTermId});
                    table.ForeignKey(
                        name: "FK_ServiceOntologyTerm_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOntologyTerm_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequirement",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Requirement = table.Column<string>(nullable: true),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequirement_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequirement_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceClass",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    ServiceClassId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceClass", x => new {x.ServiceId, x.ServiceClassId});
                    table.ForeignKey(
                        name: "FK_ServiceServiceClass_ServiceClass_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceClass_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTargetGroup",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    TargetGroupId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTargetGroup", x => new {x.ServiceId, x.TargetGroupId});
                    table.ForeignKey(
                        name: "FK_ServiceTargetGroup_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTargetGroup_TargetGroup_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalSchema: "public",
                        principalTable: "TargetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceWebPage",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWebPage", x => new {x.ServiceId, x.WebPageId});
                    table.ForeignKey(
                        name: "FK_ServiceWebPage_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceWebPage_WebPageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WebPageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationEmailDescription",
                schema: "public",
                columns: table => new
                {
                    OrganizationEmailId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationEmailDescription",
                        x => new {x.OrganizationEmailId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_OrganizationEmailDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationEmailDescription_OrganizationEmail_OrganizationEmailId",
                        column: x => x.OrganizationEmailId,
                        principalSchema: "public",
                        principalTable: "OrganizationEmail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPhoneDescription",
                schema: "public",
                columns: table => new
                {
                    OrganizationPhoneId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPhoneDescription",
                        x => new {x.OrganizationPhoneId, x.LocalizationId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_OrganizationPhoneDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationPhoneDescription_OrganizationPhone_OrganizationPhoneId",
                        column: x => x.OrganizationPhoneId,
                        principalSchema: "public",
                        principalTable: "OrganizationPhone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationPhoneDescription_PhoneDescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PhoneDescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RequiresAuthentication = table.Column<bool>(nullable: false),
                    RequiresSignature = table.Column<bool>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    SignatureQuantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicChannel", x => x.Id);
                    table.UniqueConstraint("AK_ElectronicChannel_ServiceChannelId", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_ElectronicChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhoneChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PhoneTypeId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneChannel", x => x.Id);
                    table.UniqueConstraint("AK_PhoneChannel_ServiceChannelId", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_PhoneChannel_PhoneNumberType_PhoneTypeId",
                        column: x => x.PhoneTypeId,
                        principalSchema: "public",
                        principalTable: "PhoneNumberType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DeliveryAddressId = table.Column<Guid>(nullable: true),
                    FormIdentifier = table.Column<string>(nullable: true),
                    FormReceiver = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintableFormChannel", x => x.Id);
                    table.UniqueConstraint("AK_PrintableFormChannel_ServiceChannelId", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannel_Address_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelDescription",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelDescription",
                        x => new {x.ServiceChannelId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelDescription_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelEmail",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelEmail_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelKeyword",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    KeywordId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelKeyword", x => new {x.ServiceChannelId, x.KeywordId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelKeyword_Keyword_KeywordId",
                        column: x => x.KeywordId,
                        principalSchema: "public",
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelKeyword_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelLanguage",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelLanguage", x => new {x.ServiceChannelId, x.LanguageId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelLanguage_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelLanguage_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelName",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelName", x => new {x.ServiceChannelId, x.TypeId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelName_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelOntologyTerm",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    OntologyTermId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelOntologyTerm", x => new {x.ServiceChannelId, x.OntologyTermId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelOntologyTerm_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelOntologyTerm_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelPhone",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelPhone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelPhone_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelPhone_PhoneNumberType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PhoneNumberType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelServiceClass",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ServiceClassId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelServiceClass", x => new {x.ServiceChannelId, x.ServiceClassId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelServiceClass_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelServiceClass_ServiceClass_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalSchema: "public",
                        principalTable: "ServiceClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelServiceHours",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Closes = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ExceptionHoursTypeId = table.Column<Guid>(nullable: true),
                    Friday = table.Column<bool>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Monday = table.Column<bool>(nullable: false),
                    Opens = table.Column<string>(nullable: true),
                    Saturday = table.Column<bool>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ServiceHourTypeId = table.Column<Guid>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true),
                    Wednesday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelServiceHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelServiceHours_ExceptionHoursStatusType_ExceptionHoursTypeId",
                        column: x => x.ExceptionHoursTypeId,
                        principalSchema: "public",
                        principalTable: "ExceptionHoursStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceChannelServiceHours_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelServiceHours_ServiceHourType_ServiceHourTypeId",
                        column: x => x.ServiceHourTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceHourType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelSupport",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PhoneChargeDescription = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelSupport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelSupport_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelSupport_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelTargetGroup",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    TargetGroupId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelTargetGroup", x => new {x.ServiceChannelId, x.TargetGroupId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelTargetGroup_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelTargetGroup_TargetGroup_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalSchema: "public",
                        principalTable: "TargetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelWebPage",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelWebPage", x => new {x.ServiceChannelId, x.WebPageId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelWebPage_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelWebPage_WebPageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WebPageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CoordinateSystem = table.Column<string>(nullable: true),
                    CoordinatesSetManually = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PhoneServiceCharge = table.Column<bool>(nullable: false),
                    ServiceAreaRestricted = table.Column<bool>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannel", x => x.Id);
                    table.UniqueConstraint("AK_ServiceLocationChannel_ServiceChannelId", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannel",
                schema: "public",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceChannel", x => new {x.ServiceId, x.ServiceChannelId});
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannel_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebpageChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebpageChannel", x => x.Id);
                    table.UniqueConstraint("AK_WebpageChannel_ServiceChannelId", x => x.ServiceChannelId);
                    table.ForeignKey(
                        name: "FK_WebpageChannel_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationServiceAdditionalInformation",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrganizationServiceId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationServiceAdditionalInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationServiceAdditionalInformation_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationServiceAdditionalInformation_OrganizationService_OrganizationServiceId",
                        column: x => x.OrganizationServiceId,
                        principalSchema: "public",
                        principalTable: "OrganizationService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationServiceWebPage",
                schema: "public",
                columns: table => new
                {
                    OrganizationServiceId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationServiceWebPage",
                        x => new {x.OrganizationServiceId, x.WebPageId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_OrganizationServiceWebPage_OrganizationService_OrganizationServiceId",
                        column: x => x.OrganizationServiceId,
                        principalSchema: "public",
                        principalTable: "OrganizationService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationServiceWebPage_WebPageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WebPageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationServiceWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicChannelAttachment",
                schema: "public",
                columns: table => new
                {
                    ElectronicChannelId = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicChannelAttachment", x => new {x.ElectronicChannelId, x.AttachmentId});
                    table.ForeignKey(
                        name: "FK_ElectronicChannelAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalSchema: "public",
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectronicChannelAttachment_ElectronicChannel_ElectronicChannelId",
                        column: x => x.ElectronicChannelId,
                        principalSchema: "public",
                        principalTable: "ElectronicChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicChannelUrl",
                schema: "public",
                columns: table => new
                {
                    ElectronicChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicChannelUrl", x => new {x.ElectronicChannelId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ElectronicChannelUrl_ElectronicChannel_ElectronicChannelId",
                        column: x => x.ElectronicChannelId,
                        principalSchema: "public",
                        principalTable: "ElectronicChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectronicChannelUrl_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneChannelPhone",
                schema: "public",
                columns: table => new
                {
                    PhoneChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneChannelPhone", x => new {x.PhoneChannelId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_PhoneChannelPhone_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneChannelPhone_PhoneChannel_PhoneChannelId",
                        column: x => x.PhoneChannelId,
                        principalSchema: "public",
                        principalTable: "PhoneChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneChannelPhoneChargeDescription",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PhoneChannelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneChannelPhoneChargeDescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneChannelPhoneChargeDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneChannelPhoneChargeDescription_PhoneChannel_PhoneChannelId",
                        column: x => x.PhoneChannelId,
                        principalSchema: "public",
                        principalTable: "PhoneChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneChannelServiceChargeType",
                schema: "public",
                columns: table => new
                {
                    PhoneChannelId = table.Column<Guid>(nullable: false),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneChannelServiceChargeType",
                        x => new {x.PhoneChannelId, x.ServiceChargeTypeId});
                    table.ForeignKey(
                        name: "FK_PhoneChannelServiceChargeType_PhoneChannel_PhoneChannelId",
                        column: x => x.PhoneChannelId,
                        principalSchema: "public",
                        principalTable: "PhoneChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneChannelServiceChargeType_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannelAttachment",
                schema: "public",
                columns: table => new
                {
                    PrintableFormChannelId = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintableFormChannelAttachment",
                        x => new {x.PrintableFormChannelId, x.AttachmentId});
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalSchema: "public",
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelAttachment_PrintableFormChannel_PrintableFormChannelId",
                        column: x => x.PrintableFormChannelId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintableFormChannelUrl",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PrintableFormChannelId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintableFormChannelUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelUrl_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelUrl_PrintableFormChannel_PrintableFormChannelId",
                        column: x => x.PrintableFormChannelId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintableFormChannelUrl_PrintableFormChannelUrlType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PrintableFormChannelUrlType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHoursAdditionalInformation",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelServiceHoursId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHoursAdditionalInformation",
                        x => new {x.ServiceChannelServiceHoursId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceHoursAdditionalInformation_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_ServiceHoursAdditionalInformation_ServiceChannelServiceHours_ServiceChannelServiceHoursId",
                        column: x => x.ServiceChannelServiceHoursId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelServiceHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelSupportServiceChargeType",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelSupportId = table.Column<Guid>(nullable: false),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelSupportServiceChargeType",
                        x => new {x.ServiceChannelSupportId, x.ServiceChargeTypeId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelSupportServiceChargeType_ServiceChannelSupport_ServiceChannelSupportId",
                        column: x => x.ServiceChannelSupportId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelSupport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelSupportServiceChargeType_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannelAddress",
                schema: "public",
                columns: table => new
                {
                    ServiceLocationChannelId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannelAddress",
                        x => new {x.ServiceLocationChannelId, x.AddressId, x.TypeId});
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelAddress_ServiceLocationChannel_ServiceLocationChannelId",
                        column: x => x.ServiceLocationChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceLocationChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelAddress_AddressType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AddressType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannelPhoneChargeDescription",
                schema: "public",
                columns: table => new
                {
                    ServiceLocationChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannelPhoneChargeDescription",
                        x => new {x.ServiceLocationChannelId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelPhoneChargeDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_ServiceLocationChannelPhoneChargeDescription_ServiceLocationChannel_ServiceLocationChannelId",
                        column: x => x.ServiceLocationChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceLocationChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannelServiceArea",
                schema: "public",
                columns: table => new
                {
                    ServiceLocationChannelId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannelServiceArea",
                        x => new {x.ServiceLocationChannelId, x.MunicipalityId});
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelServiceArea_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelServiceArea_ServiceLocationChannel_ServiceLocationChannelId",
                        column: x => x.ServiceLocationChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceLocationChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLocationChannelServiceChargeType",
                schema: "public",
                columns: table => new
                {
                    ServiceLocationChannelId = table.Column<Guid>(nullable: false),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLocationChannelServiceChargeType",
                        x => new {x.ServiceLocationChannelId, x.ServiceChargeTypeId});
                    table.ForeignKey(
                        name: "FK_ServiceLocationChannelServiceChargeType_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_ServiceLocationChannelServiceChargeType_ServiceLocationChannel_ServiceLocationChannelId",
                        column: x => x.ServiceLocationChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceLocationChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebpageChannelAttachment",
                schema: "public",
                columns: table => new
                {
                    WebpageChannelId = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebpageChannelAttachment", x => new {x.WebpageChannelId, x.AttachmentId});
                    table.ForeignKey(
                        name: "FK_WebpageChannelAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalSchema: "public",
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebpageChannelAttachment_WebpageChannel_WebpageChannelId",
                        column: x => x.WebpageChannelId,
                        principalSchema: "public",
                        principalTable: "WebpageChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebpageChannelUrl",
                schema: "public",
                columns: table => new
                {
                    WebpageChannelId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebpageChannelUrl", x => new {x.WebpageChannelId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_WebpageChannelUrl_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebpageChannelUrl_WebpageChannel_WebpageChannelId",
                        column: x => x.WebpageChannelId,
                        principalSchema: "public",
                        principalTable: "WebpageChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                schema: "public",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Id",
                schema: "public",
                table: "Address",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Address_MunicipalityId",
                schema: "public",
                table: "Address",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_PostalCodeId",
                schema: "public",
                table: "Address",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressAdditionalInformation_AddressId",
                schema: "public",
                table: "AddressAdditionalInformation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressAdditionalInformation_LocalizationId",
                schema: "public",
                table: "AddressAdditionalInformation",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressType_Id",
                schema: "public",
                table: "AddressType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypeName_Id",
                schema: "public",
                table: "AddressTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypeName_LocalizationId",
                schema: "public",
                table: "AddressTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypeName_TypeId",
                schema: "public",
                table: "AddressTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_Id",
                schema: "public",
                table: "Attachment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_LocalizationId",
                schema: "public",
                table: "Attachment",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TypeId",
                schema: "public",
                table: "Attachment",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentType_Id",
                schema: "public",
                table: "AttachmentType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTypeName_Id",
                schema: "public",
                table: "AttachmentTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTypeName_LocalizationId",
                schema: "public",
                table: "AttachmentTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTypeName_TypeId",
                schema: "public",
                table: "AttachmentTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Id",
                schema: "public",
                table: "Business",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Country_Id",
                schema: "public",
                table: "Country",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CountryName_CountryId",
                schema: "public",
                table: "CountryName",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryName_Id",
                schema: "public",
                table: "CountryName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CountryName_LocalizationId",
                schema: "public",
                table: "CountryName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionType_Id",
                schema: "public",
                table: "DescriptionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionTypeName_Id",
                schema: "public",
                table: "DescriptionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionTypeName_LocalizationId",
                schema: "public",
                table: "DescriptionTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionTypeName_TypeId",
                schema: "public",
                table: "DescriptionTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannel_Id",
                schema: "public",
                table: "ElectronicChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannel_ServiceChannelId",
                schema: "public",
                table: "ElectronicChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannelAttachment_AttachmentId",
                schema: "public",
                table: "ElectronicChannelAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannelAttachment_ElectronicChannelId",
                schema: "public",
                table: "ElectronicChannelAttachment",
                column: "ElectronicChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannelUrl_ElectronicChannelId",
                schema: "public",
                table: "ElectronicChannelUrl",
                column: "ElectronicChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicChannelUrl_LocalizationId",
                schema: "public",
                table: "ElectronicChannelUrl",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionHoursStatusType_Id",
                schema: "public",
                table: "ExceptionHoursStatusType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionHoursStatusTypeName_Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionHoursStatusTypeName_LocalizationId",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionHoursStatusTypeName_TypeId",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSource_Id",
                schema: "public",
                table: "ExternalSource",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Form_Id",
                schema: "public",
                table: "Form",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Form_LocalizationId",
                schema: "public",
                table: "Form",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_TypeId",
                schema: "public",
                table: "Form",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormType_Id",
                schema: "public",
                table: "FormType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FormTypeName_Id",
                schema: "public",
                table: "FormTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FormTypeName_LocalizationId",
                schema: "public",
                table: "FormTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTypeName_TypeId",
                schema: "public",
                table: "FormTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialClass_Id",
                schema: "public",
                table: "IndustrialClass",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialClass_ParentId",
                schema: "public",
                table: "IndustrialClass",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialClassName_Id",
                schema: "public",
                table: "IndustrialClassName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialClassName_IndustrialClassId",
                schema: "public",
                table: "IndustrialClassName",
                column: "IndustrialClassId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustrialClassName_LocalizationId",
                schema: "public",
                table: "IndustrialClassName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Keyword_Id",
                schema: "public",
                table: "Keyword",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Language_Id",
                schema: "public",
                table: "Language",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageName_Id",
                schema: "public",
                table: "LanguageName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageName_LanguageId",
                schema: "public",
                table: "LanguageName",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageName_LocalizationId",
                schema: "public",
                table: "LanguageName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEvent_Id",
                schema: "public",
                table: "LifeEvent",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEvent_ParentId",
                schema: "public",
                table: "LifeEvent",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventName_Id",
                schema: "public",
                table: "LifeEventName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventName_LifeEventId",
                schema: "public",
                table: "LifeEventName",
                column: "LifeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeEventName_LocalizationId",
                schema: "public",
                table: "LifeEventName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipality_Id",
                schema: "public",
                table: "Municipality",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NameType_Id",
                schema: "public",
                table: "NameType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NameTypeName_Id",
                schema: "public",
                table: "NameTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NameTypeName_LocalizationId",
                schema: "public",
                table: "NameTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NameTypeName_TypeId",
                schema: "public",
                table: "NameTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTerm_Id",
                schema: "public",
                table: "OntologyTerm",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTermName_Id",
                schema: "public",
                table: "OntologyTermName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTermName_LocalizationId",
                schema: "public",
                table: "OntologyTermName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTermName_OntologyTermId",
                schema: "public",
                table: "OntologyTermName",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTermParent_ChildId",
                schema: "public",
                table: "OntologyTermParent",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTermParent_ParentId",
                schema: "public",
                table: "OntologyTermParent",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHoursType_Id",
                schema: "public",
                table: "OpeningHoursType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHoursTypeName_Id",
                schema: "public",
                table: "OpeningHoursTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHoursTypeName_LocalizationId",
                schema: "public",
                table: "OpeningHoursTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHoursTypeName_TypeId",
                schema: "public",
                table: "OpeningHoursTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_BusinessId",
                schema: "public",
                table: "Organization",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_DisplayNameTypeId",
                schema: "public",
                table: "Organization",
                column: "DisplayNameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Id",
                schema: "public",
                table: "Organization",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_MunicipalityId",
                schema: "public",
                table: "Organization",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ParentId",
                schema: "public",
                table: "Organization",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_PublishingStatusId",
                schema: "public",
                table: "Organization",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_TypeId",
                schema: "public",
                table: "Organization",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddress_AddressId",
                schema: "public",
                table: "OrganizationAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddress_OrganizationId",
                schema: "public",
                table: "OrganizationAddress",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddress_TypeId",
                schema: "public",
                table: "OrganizationAddress",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDescription_LocalizationId",
                schema: "public",
                table: "OrganizationDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDescription_OrganizationId",
                schema: "public",
                table: "OrganizationDescription",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDescription_TypeId",
                schema: "public",
                table: "OrganizationDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmail_Id",
                schema: "public",
                table: "OrganizationEmail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmail_OrganizationId",
                schema: "public",
                table: "OrganizationEmail",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmailDescription_LocalizationId",
                schema: "public",
                table: "OrganizationEmailDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmailDescription_OrganizationEmailId",
                schema: "public",
                table: "OrganizationEmailDescription",
                column: "OrganizationEmailId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationName_LocalizationId",
                schema: "public",
                table: "OrganizationName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationName_OrganizationId",
                schema: "public",
                table: "OrganizationName",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationName_TypeId",
                schema: "public",
                table: "OrganizationName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_Id",
                schema: "public",
                table: "OrganizationPhone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_OrganizationId",
                schema: "public",
                table: "OrganizationPhone",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_ServiceChargeTypeId",
                schema: "public",
                table: "OrganizationPhone",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_TypeId",
                schema: "public",
                table: "OrganizationPhone",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhoneDescription_LocalizationId",
                schema: "public",
                table: "OrganizationPhoneDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhoneDescription_OrganizationPhoneId",
                schema: "public",
                table: "OrganizationPhoneDescription",
                column: "OrganizationPhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhoneDescription_TypeId",
                schema: "public",
                table: "OrganizationPhoneDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationService_Id",
                schema: "public",
                table: "OrganizationService",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationService_OrganizationId",
                schema: "public",
                table: "OrganizationService",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationService_ProvisionTypeId",
                schema: "public",
                table: "OrganizationService",
                column: "ProvisionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationService_RoleTypeId",
                schema: "public",
                table: "OrganizationService",
                column: "RoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationService_ServiceId",
                schema: "public",
                table: "OrganizationService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceAdditionalInformation_Id",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceAdditionalInformation_LocalizationId",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceAdditionalInformation_OrganizationServiceId",
                schema: "public",
                table: "OrganizationServiceAdditionalInformation",
                column: "OrganizationServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceWebPage_OrganizationServiceId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                column: "OrganizationServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceWebPage_TypeId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationServiceWebPage_WebPageId",
                schema: "public",
                table: "OrganizationServiceWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationType_Id",
                schema: "public",
                table: "OrganizationType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTypeName_Id",
                schema: "public",
                table: "OrganizationTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTypeName_LocalizationId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTypeName_TypeId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationWebPage_OrganizationId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationWebPage_TypeId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationWebPage_WebPageId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneDescriptionType_Id",
                schema: "public",
                table: "PhoneDescriptionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannel_Id",
                schema: "public",
                table: "PhoneChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannel_PhoneTypeId",
                schema: "public",
                table: "PhoneChannel",
                column: "PhoneTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannel_ServiceChannelId",
                schema: "public",
                table: "PhoneChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelPhone_LocalizationId",
                schema: "public",
                table: "PhoneChannelPhone",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelPhone_PhoneChannelId",
                schema: "public",
                table: "PhoneChannelPhone",
                column: "PhoneChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelPhoneChargeDescription_Id",
                schema: "public",
                table: "PhoneChannelPhoneChargeDescription",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelPhoneChargeDescription_LocalizationId",
                schema: "public",
                table: "PhoneChannelPhoneChargeDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelPhoneChargeDescription_PhoneChannelId",
                schema: "public",
                table: "PhoneChannelPhoneChargeDescription",
                column: "PhoneChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelServiceChargeType_PhoneChannelId",
                schema: "public",
                table: "PhoneChannelServiceChargeType",
                column: "PhoneChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneChannelServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "PhoneChannelServiceChargeType",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberType_Id",
                schema: "public",
                table: "PhoneNumberType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberTypeName_Id",
                schema: "public",
                table: "PhoneNumberTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberTypeName_LocalizationId",
                schema: "public",
                table: "PhoneNumberTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumberTypeName_TypeId",
                schema: "public",
                table: "PhoneNumberTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCode_Id",
                schema: "public",
                table: "PostalCode",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannel_DeliveryAddressId",
                schema: "public",
                table: "PrintableFormChannel",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannel_Id",
                schema: "public",
                table: "PrintableFormChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannel_ServiceChannelId",
                schema: "public",
                table: "PrintableFormChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelAttachment_AttachmentId",
                schema: "public",
                table: "PrintableFormChannelAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelAttachment_PrintableFormChannelId",
                schema: "public",
                table: "PrintableFormChannelAttachment",
                column: "PrintableFormChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrl_Id",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrl_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrl_PrintableFormChannelId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "PrintableFormChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrl_TypeId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrlType_Id",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrlTypeName_Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrlTypeName_LocalizationId",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintableFormChannelUrlTypeName_TypeId",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionType_Id",
                schema: "public",
                table: "ProvisionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionTypeName_Id",
                schema: "public",
                table: "ProvisionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionTypeName_LocalizationId",
                schema: "public",
                table: "ProvisionTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionTypeName_TypeId",
                schema: "public",
                table: "ProvisionTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingStatusType_Id",
                schema: "public",
                table: "PublishingStatusType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingStatusTypeName_Id",
                schema: "public",
                table: "PublishingStatusTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingStatusTypeName_LocalizationId",
                schema: "public",
                table: "PublishingStatusTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingStatusTypeName_TypeId",
                schema: "public",
                table: "PublishingStatusTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleType_Id",
                schema: "public",
                table: "RoleType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTypeName_Id",
                schema: "public",
                table: "RoleTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTypeName_LocalizationId",
                schema: "public",
                table: "RoleTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleTypeName_TypeId",
                schema: "public",
                table: "RoleTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_Id",
                schema: "public",
                table: "Service",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Service_PublishingStatusId",
                schema: "public",
                table: "Service",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceChargeTypeId",
                schema: "public",
                table: "Service",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceCoverageTypeId",
                schema: "public",
                table: "Service",
                column: "ServiceCoverageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "Service",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_TypeId",
                schema: "public",
                table: "Service",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAdditionalInformation_LocalizationId",
                schema: "public",
                table: "ServiceAdditionalInformation",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAdditionalInformation_ServiceId",
                schema: "public",
                table: "ServiceAdditionalInformation",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAdditionalInformation_TypeId",
                schema: "public",
                table: "ServiceAdditionalInformation",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAdditionalInformationType_Id",
                schema: "public",
                table: "ServiceAdditionalInformationType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClass_Id",
                schema: "public",
                table: "ServiceClass",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClass_ParentId",
                schema: "public",
                table: "ServiceClass",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassName_Id",
                schema: "public",
                table: "ServiceClassName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassName_LocalizationId",
                schema: "public",
                table: "ServiceClassName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassName_ServiceClassId",
                schema: "public",
                table: "ServiceClassName",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCoverageType_Id",
                schema: "public",
                table: "ServiceCoverageType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCoverageTypeName_Id",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCoverageTypeName_LocalizationId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCoverageTypeName_TypeId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDescription_LocalizationId",
                schema: "public",
                table: "ServiceDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDescription_ServiceId",
                schema: "public",
                table: "ServiceDescription",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDescription_TypeId",
                schema: "public",
                table: "ServiceDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicCommunicationChannel_Id",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicCommunicationChannel_LocalizationId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicCommunicationChannel_ServiceId",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicNotificationChannel_Id",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicNotificationChannel_LocalizationId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceElectronicNotificationChannel_ServiceId",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHoursAdditionalInformation_LocalizationId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHoursAdditionalInformation_ServiceChannelServiceHoursId",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                column: "ServiceChannelServiceHoursId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHourType_Id",
                schema: "public",
                table: "ServiceHourType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHourTypeName_Id",
                schema: "public",
                table: "ServiceHourTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHourTypeName_LocalizationId",
                schema: "public",
                table: "ServiceHourTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHourTypeName_TypeId",
                schema: "public",
                table: "ServiceHourTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannel_Id",
                schema: "public",
                table: "ServiceChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannel_OrganizationId",
                schema: "public",
                table: "ServiceChannel",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannel_PublishingStatusId",
                schema: "public",
                table: "ServiceChannel",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannel_TypeId",
                schema: "public",
                table: "ServiceChannel",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelDescription_LocalizationId",
                schema: "public",
                table: "ServiceChannelDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelDescription_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelDescription",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelDescription_TypeId",
                schema: "public",
                table: "ServiceChannelDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelEmail_Id",
                schema: "public",
                table: "ServiceChannelEmail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelEmail_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelEmail",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelKeyword_KeywordId",
                schema: "public",
                table: "ServiceChannelKeyword",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelKeyword_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelKeyword",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelLanguage_LanguageId",
                schema: "public",
                table: "ServiceChannelLanguage",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelLanguage_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelLanguage",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelName_LocalizationId",
                schema: "public",
                table: "ServiceChannelName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelName_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelName",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelName_TypeId",
                schema: "public",
                table: "ServiceChannelName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelOntologyTerm_OntologyTermId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelOntologyTerm_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelPhone_Id",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelPhone_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelPhone_TypeId",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceClass_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceHours_ExceptionHoursTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "ExceptionHoursTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceHours_Id",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceHours_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelServiceHours_ServiceHourTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours",
                column: "ServiceHourTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelSupport_Id",
                schema: "public",
                table: "ServiceChannelSupport",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelSupport_LocalizationId",
                schema: "public",
                table: "ServiceChannelSupport",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelSupport_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelSupport",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelSupportServiceChargeType_ServiceChannelSupportId",
                schema: "public",
                table: "ServiceChannelSupportServiceChargeType",
                column: "ServiceChannelSupportId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelSupportServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceChannelSupportServiceChargeType",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelTargetGroup_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelTargetGroup_TargetGroupId",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelType_Id",
                schema: "public",
                table: "ServiceChannelType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelTypeName_Id",
                schema: "public",
                table: "ServiceChannelTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelTypeName_LocalizationId",
                schema: "public",
                table: "ServiceChannelTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelTypeName_TypeId",
                schema: "public",
                table: "ServiceChannelTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelWebPage_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelWebPage_TypeId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelWebPage_WebPageId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChargeType_Id",
                schema: "public",
                table: "ServiceChargeType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChargeTypeName_Id",
                schema: "public",
                table: "ServiceChargeTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChargeTypeName_LocalizationId",
                schema: "public",
                table: "ServiceChargeTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChargeTypeName_TypeId",
                schema: "public",
                table: "ServiceChargeTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIndustrialClass_IndustrialClassId",
                schema: "public",
                table: "ServiceIndustrialClass",
                column: "IndustrialClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIndustrialClass_ServiceId",
                schema: "public",
                table: "ServiceIndustrialClass",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceKeyword_KeywordId",
                schema: "public",
                table: "ServiceKeyword",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceKeyword_ServiceId",
                schema: "public",
                table: "ServiceKeyword",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLanguage_LanguageId",
                schema: "public",
                table: "ServiceLanguage",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLanguage_ServiceId",
                schema: "public",
                table: "ServiceLanguage",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLifeEvent_LifeEventId",
                schema: "public",
                table: "ServiceLifeEvent",
                column: "LifeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLifeEvent_ServiceId",
                schema: "public",
                table: "ServiceLifeEvent",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannel_Id",
                schema: "public",
                table: "ServiceLocationChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceLocationChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelAddress_AddressId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelAddress_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                column: "ServiceLocationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelAddress_TypeId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelPhoneChargeDescription_LocalizationId",
                schema: "public",
                table: "ServiceLocationChannelPhoneChargeDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelPhoneChargeDescription_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelPhoneChargeDescription",
                column: "ServiceLocationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelServiceArea_MunicipalityId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelServiceArea_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelServiceArea",
                column: "ServiceLocationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceLocationChannelServiceChargeType",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLocationChannelServiceChargeType_ServiceLocationChannelId",
                schema: "public",
                table: "ServiceLocationChannelServiceChargeType",
                column: "ServiceLocationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMunicipality_MunicipalityId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMunicipality_ServiceId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceName_LocalizationId",
                schema: "public",
                table: "ServiceName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceName_ServiceId",
                schema: "public",
                table: "ServiceName",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceName_TypeId",
                schema: "public",
                table: "ServiceName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOntologyTerm_OntologyTermId",
                schema: "public",
                table: "ServiceOntologyTerm",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOntologyTerm_ServiceId",
                schema: "public",
                table: "ServiceOntologyTerm",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequirement_Id",
                schema: "public",
                table: "ServiceRequirement",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequirement_LocalizationId",
                schema: "public",
                table: "ServiceRequirement",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequirement_ServiceId",
                schema: "public",
                table: "ServiceRequirement",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceClass_ServiceClassId",
                schema: "public",
                table: "ServiceServiceClass",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceClass_ServiceId",
                schema: "public",
                table: "ServiceServiceClass",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannel_ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTargetGroup_ServiceId",
                schema: "public",
                table: "ServiceTargetGroup",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTargetGroup_TargetGroupId",
                schema: "public",
                table: "ServiceTargetGroup",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceType_Id",
                schema: "public",
                table: "ServiceType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeName_Id",
                schema: "public",
                table: "ServiceTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeName_LocalizationId",
                schema: "public",
                table: "ServiceTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeName_TypeId",
                schema: "public",
                table: "ServiceTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWebPage_ServiceId",
                schema: "public",
                table: "ServiceWebPage",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWebPage_TypeId",
                schema: "public",
                table: "ServiceWebPage",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWebPage_WebPageId",
                schema: "public",
                table: "ServiceWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceDescription_LocalizationId",
                schema: "public",
                table: "StatutoryServiceDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceDescription",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceDescription_TypeId",
                schema: "public",
                table: "StatutoryServiceDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceGeneralDescription_Id",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLanguage_LanguageId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLanguage_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLanguage",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLifeEvent_LifeEventId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                column: "LifeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLifeEvent_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceName_LocalizationId",
                schema: "public",
                table: "StatutoryServiceName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceName_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceName",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceName_TypeId",
                schema: "public",
                table: "StatutoryServiceName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceOntologyTerm_OntologyTermId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceOntologyTerm_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceServiceClass_ServiceClassId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceServiceClass_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceTargetGroup_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceTargetGroup_TargetGroupId",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetAddress_AddressId",
                schema: "public",
                table: "StreetAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetAddress_Id",
                schema: "public",
                table: "StreetAddress",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StreetAddress_LocalizationId",
                schema: "public",
                table: "StreetAddress",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroup_Id",
                schema: "public",
                table: "TargetGroup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroup_ParentId",
                schema: "public",
                table: "TargetGroup",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroupName_Id",
                schema: "public",
                table: "TargetGroupName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroupName_LocalizationId",
                schema: "public",
                table: "TargetGroupName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetGroupName_TargetGroupId",
                schema: "public",
                table: "TargetGroupName",
                column: "TargetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganization_Id",
                schema: "public",
                table: "UserOrganization",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebPage_Id",
                schema: "public",
                table: "WebPage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebPage_LocalizationId",
                schema: "public",
                table: "WebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannel_Id",
                schema: "public",
                table: "WebpageChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannel_ServiceChannelId",
                schema: "public",
                table: "WebpageChannel",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannelAttachment_AttachmentId",
                schema: "public",
                table: "WebpageChannelAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannelAttachment_WebpageChannelId",
                schema: "public",
                table: "WebpageChannelAttachment",
                column: "WebpageChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannelUrl_LocalizationId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WebpageChannelUrl_WebpageChannelId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "WebpageChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WebPageType_Id",
                schema: "public",
                table: "WebPageType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebPageTypeName_Id",
                schema: "public",
                table: "WebPageTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebPageTypeName_LocalizationId",
                schema: "public",
                table: "WebPageTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WebPageTypeName_TypeId",
                schema: "public",
                table: "WebPageTypeName",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
