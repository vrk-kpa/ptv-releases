using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PTV_1_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddColumns(migrationBuilder);

            CreateTables(migrationBuilder);

            AddSqlScripts(migrationBuilder);

            DropFks(migrationBuilder);

            DropTables(migrationBuilder);

            DropIndexes(migrationBuilder);

            DropColumns(migrationBuilder);

            AddPrimaryKeys(migrationBuilder);

            AddFks(migrationBuilder);
        }

		private void AddSqlScripts(MigrationBuilder migrationBuilder)
        {
            var directory = new DirectoryInfo(@"SqlMigrations\PTV_1_3");
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles("*.sql"))
                {
                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
                }

            }
        }

        private static void AddFks(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceGeneralDescription_ChargeTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceGeneralDescription_TypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannel_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelEmail_EmailId",
                schema: "public",
                table: "ServiceChannelEmail",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_VersioningId",
                schema: "public",
                table: "Service",
                column: "VersioningId");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCode_MunicipalityId",
                schema: "public",
                table: "PostalCode",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmail_EmailId",
                schema: "public",
                table: "OrganizationEmail",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Oid",
                schema: "public",
                table: "Organization",
                column: "Oid");

            migrationBuilder.CreateIndex(
                name: "IX_Keyword_LocalizationId",
                schema: "public",
                table: "Keyword",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyOpeningTime_OpeningHourId_DayFrom_IsExtra",
                schema: "public",
                table: "DailyOpeningTime",
                columns: new[] {"OpeningHourId", "DayFrom", "IsExtra"});

            migrationBuilder.CreateIndex(
                name: "IX_Email_Id",
                schema: "public",
                table: "Email",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Email_LocalizationId",
                schema: "public",
                table: "Email",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Law_Id",
                schema: "public",
                table: "Law",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LawName_LawId",
                schema: "public",
                table: "LawName",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "IX_LawName_LocalizationId",
                schema: "public",
                table: "LawName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LawWebPage_LawId",
                schema: "public",
                table: "LawWebPage",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "IX_LawWebPage_WebPageId",
                schema: "public",
                table: "LawWebPage",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Locking_Id",
                schema: "public",
                table: "Locking",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_Id",
                schema: "public",
                table: "Phone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_LocalizationId",
                schema: "public",
                table: "Phone",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_ServiceChargeTypeId",
                schema: "public",
                table: "Phone",
                column: "ServiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_TypeId",
                schema: "public",
                table: "Phone",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelAttachment_AttachmentId",
                schema: "public",
                table: "ServiceChannelAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelAttachment_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelAttachment",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannelDescription_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannelDescription_TypeId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannelDescription_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                columns: new[] {"ServiceId", "ServiceChannelId"});

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceIndustrialClass_IndustrialClassId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                column: "IndustrialClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceIndustrialClass_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLaw_LawId",
                schema: "public",
                table: "StatutoryServiceLaw",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceLaw_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceLaw",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceRequirement_Id",
                schema: "public",
                table: "StatutoryServiceRequirement",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceRequirement_LocalizationId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StatutoryServiceRequirement_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "StatutoryServiceRequirement",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetName_AddressId",
                schema: "public",
                table: "StreetName",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_StreetName_Id",
                schema: "public",
                table: "StreetName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StreetName_LocalizationId",
                schema: "public",
                table: "StreetName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Versioning_Id",
                schema: "public",
                table: "Versioning",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Versioning_PreviousVersionId",
                schema: "public",
                table: "Versioning",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChannelPhone_PhoneId",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "PhoneId");


            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_PhoneId",
                schema: "public",
                table: "OrganizationPhone",
                column: "PhoneId");
            migrationBuilder.AddForeignKey(
                name: "FK_Keyword_Language_LocalizationId",
                schema: "public",
                table: "Keyword",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationEmail_Email_EmailId",
                schema: "public",
                table: "OrganizationEmail",
                column: "EmailId",
                principalSchema: "public",
                principalTable: "Email",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPhone_Phone_PhoneId",
                schema: "public",
                table: "OrganizationPhone",
                column: "PhoneId",
                principalSchema: "public",
                principalTable: "Phone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostalCode_Municipality_MunicipalityId",
                schema: "public",
                table: "PostalCode",
                column: "MunicipalityId",
                principalSchema: "public",
                principalTable: "Municipality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Versioning_VersioningId",
                schema: "public",
                table: "Service",
                column: "VersioningId",
                principalSchema: "public",
                principalTable: "Versioning",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelEmail_Email_EmailId",
                schema: "public",
                table: "ServiceChannelEmail",
                column: "EmailId",
                principalSchema: "public",
                principalTable: "Email",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelPhone_Phone_PhoneId",
                schema: "public",
                table: "ServiceChannelPhone",
                column: "PhoneId",
                principalSchema: "public",
                principalTable: "Phone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescription_ServiceChargeType_ChargeTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "ChargeTypeId",
                principalSchema: "public",
                principalTable: "ServiceChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescription_ServiceType_TypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        private static void CreateTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyOpeningTime",
                schema: "public",
                columns: table => new
                {
                    OpeningHourId = table.Column<Guid>(nullable: false),
                    DayFrom = table.Column<int>(nullable: false),
                    IsExtra = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DayTo = table.Column<int>(nullable: true),
                    From = table.Column<TimeSpan>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    To = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyOpeningTime", x => new {x.OpeningHourId, x.DayFrom, x.IsExtra});
                    table.ForeignKey(
                        name: "FK_DailyOpeningTime_ServiceChannelServiceHours_OpeningHourId",
                        column: x => x.OpeningHourId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelServiceHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Email",
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
                    Value = table.Column<string>(nullable: true),
                    Visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Law",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Law", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "Locking",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LockedAt = table.Column<DateTime>(nullable: false),
                    LockedBy = table.Column<string>(nullable: true),
                    LockedEntityId = table.Column<Guid>(nullable: true),
                    TableName = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Locking", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "Phone",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    ChargeDescription = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    PrefixNumber = table.Column<string>(nullable: true),
                    ServiceChargeTypeId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phone_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Phone_ServiceChargeType_ServiceChargeTypeId",
                        column: x => x.ServiceChargeTypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChargeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Phone_PhoneNumberType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "PhoneNumberType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelAttachment",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelAttachment", x => new {x.ServiceChannelId, x.AttachmentId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalSchema: "public",
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelAttachment_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServiceChannelDescription",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServiceChannelDescription",
                        x => new {x.TypeId, x.LocalizationId, x.ServiceChannelId, x.ServiceId});
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServiceChannelDescription_ServiceServiceChannel_ServiceId_ServiceChannelId",
                        columns: x => new {x.ServiceId, x.ServiceChannelId},
                        principalSchema: "public",
                        principalTable: "ServiceServiceChannel",
                        principalColumns: new[] {"ServiceId", "ServiceChannelId"},
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceIndustrialClass",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    IndustrialClassId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceIndustrialClass",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.IndustrialClassId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceIndustrialClass_IndustrialClass_IndustrialClassId",
                        column: x => x.IndustrialClassId,
                        principalSchema: "public",
                        principalTable: "IndustrialClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceIndustrialClass_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceRequirement",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Requirement = table.Column<string>(maxLength: 2500, nullable: true),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatutoryServiceRequirement_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceRequirement_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.RenameTable(
                name: "StreetAddress",
                newName: "StreetName",
                schema: "public");

//            migrationBuilder.CreateTable(
//                name: "StreetName",
//                schema: "public",
//                columns: table => new
//                {
//                    Id = table.Column<Guid>(nullable: false),
//                    AddressId = table.Column<Guid>(nullable: false),
//                    Created = table.Column<DateTime>(nullable: false),
//                    CreatedBy = table.Column<string>(nullable: true),
//                    LocalizationId = table.Column<Guid>(nullable: false),
//                    Modified = table.Column<DateTime>(nullable: false),
//                    ModifiedBy = table.Column<string>(nullable: true),
//                    Text = table.Column<string>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_StreetName", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_StreetName_Address_AddressId",
//                        column: x => x.AddressId,
//                        principalSchema: "public",
//                        principalTable: "Address",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_StreetName_Language_LocalizationId",
//                        column: x => x.LocalizationId,
//                        principalSchema: "public",
//                        principalTable: "Language",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

            migrationBuilder.CreateTable(
                name: "Versioning",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PreviousVersionId = table.Column<Guid>(nullable: true),
                    VersionMajor = table.Column<int>(nullable: false),
                    VersionMinor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versioning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versioning_Versioning_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalSchema: "public",
                        principalTable: "Versioning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LawName",
                schema: "public",
                columns: table => new
                {
                    LawId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_LawName", x => new {x.LawId, x.LocalizationId});
                    table.ForeignKey(
                        name: "FK_LawName_Law_LawId",
                        column: x => x.LawId,
                        principalSchema: "public",
                        principalTable: "Law",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LawName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LawWebPage",
                schema: "public",
                columns: table => new
                {
                    LawId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawWebPage", x => new {x.LawId, x.WebPageId});
                    table.ForeignKey(
                        name: "FK_LawWebPage_Law_LawId",
                        column: x => x.LawId,
                        principalSchema: "public",
                        principalTable: "Law",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LawWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatutoryServiceLaw",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: false),
                    LawId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutoryServiceLaw",
                        x => new {x.StatutoryServiceGeneralDescriptionId, x.LawId});
                    table.ForeignKey(
                        name: "FK_StatutoryServiceLaw_Law_LawId",
                        column: x => x.LawId,
                        principalSchema: "public",
                        principalTable: "Law",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name:
                        "FK_StatutoryServiceLaw_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        private static void AddPrimaryKeys(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelPhone",
                schema: "public",
                table: "ServiceChannelPhone",
                columns: new[] {"PhoneId", "ServiceChannelId"});

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelEmail",
                schema: "public",
                table: "ServiceChannelEmail",
                columns: new[] {"EmailId", "ServiceChannelId"});

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationPhone",
                schema: "public",
                table: "OrganizationPhone",
                columns: new[] {"PhoneId", "OrganizationId"});

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationEmail",
                schema: "public",
                table: "OrganizationEmail",
                columns: new[] {"EmailId", "OrganizationId"});
        }

        private static void AddColumns(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .OldAnnotation("Npgsql:PostgresExtension:.uuid-ossp", "'uuid-ossp', '', ''");

            migrationBuilder.AddColumn<Guid>(
                name: "ChargeTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                nullable: true);

            migrationBuilder.Sql(
                @"CREATE OR REPLACE FUNCTION ServiceType_Default() RETURNS uuid LANGUAGE SQL AS  $$ SELECT ""Id"" FROM ""ServiceType"" WHERE ""Code"" = 'Service'; $$;");


            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescription",
                nullable: false,
                defaultValueSql: @"ServiceType_Default()");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceChargeTypeId",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceRequirement",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceName",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceChannelName",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmailId",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")); // ServiceChannelEmail.Id

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "ServiceDescription",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VersioningId",
                schema: "public",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipalityId",
                schema: "public",
                table: "PostalCode",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "OrganizationName",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmailId",
                schema: "public",
                table: "OrganizationEmail",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")); // id

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                schema: "public",
                table: "OrganizationDescription",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(
                @"CREATE OR REPLACE FUNCTION Language_Default() RETURNS uuid LANGUAGE SQL AS  $$ SELECT ""Id"" FROM ""Language"" WHERE ""Code"" = 'fi'; $$;");

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "Keyword",
                nullable: false,
                defaultValueSql: @"Language_Default()");

            migrationBuilder.AddColumn<string>(
                name: "CoordinateState",
                schema: "public",
                table: "Address",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "public",
                table: "Address",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longtitude",
                schema: "public",
                table: "Address",
                nullable: true);

            // changed manualy

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PhoneId",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "PhoneId",
                schema: "public",
                table: "OrganizationPhone",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        private static void DropColumns(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "public",
                table: "ServiceLocationChannel");

            migrationBuilder.DropColumn(
                name: "Fax",
                schema: "public",
                table: "ServiceLocationChannel");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "public",
                table: "ServiceLocationChannel");

            migrationBuilder.DropColumn(
                name: "Closes",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "ExceptionHoursTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Friday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Monday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Opens",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Saturday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Sunday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Thursday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Tuesday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "PrefixNumber",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "ServiceChargeTypeId",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "StreetAddressAsPostalAddress",
                schema: "public",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Wednesday",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "TypeId",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "TypeId",
                schema: "public",
                table: "OrganizationPhone");


            migrationBuilder.RenameColumn(
                name: "Qualifier",
                schema: "public",
                table: "Address",
                newName: "StreetNumber");

            migrationBuilder.RenameIndex(
                name: "PK_StreetAddress",
                schema: "public",
                table: "StreetName",
                newName: "PK_StreetName");
        }

        private static void DropIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WebpageChannel_ServiceChannelId",
                schema: "public",
                table: "WebpageChannel");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ServiceLocationChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceLocationChannel");

            migrationBuilder.DropIndex(
                name: "IX_ServiceChannelServiceHours_ExceptionHoursTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelPhone",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropIndex(
                name: "IX_ServiceChannelPhone_Id",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelEmail",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropIndex(
                name: "IX_ServiceChannelEmail_Id",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PrintableFormChannel_ServiceChannelId",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationPhone",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhone_Id",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPhone_ServiceChargeTypeId",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationEmail",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationEmail_Id",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ElectronicChannel_ServiceChannelId",
                schema: "public",
                table: "ElectronicChannel");
        }

        private static void DropTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectronicChannelAttachment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OpeningHoursTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationEmailDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationPhoneDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneChannelPhone",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneChannelPhoneChargeDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneChannelServiceChargeType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PrintableFormChannelAttachment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceAdditionalInformation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelSupportServiceChargeType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceLocationChannelPhoneChargeDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceLocationChannelServiceChargeType",
                schema: "public");

            // rename
//            migrationBuilder.DropTable(
//                name: "StreetAddress",
//                schema: "public");

            migrationBuilder.DropTable(
                name: "WebpageChannelAttachment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OpeningHoursType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneDescriptionType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneChannel",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceAdditionalInformationType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelSupport",
                schema: "public");
        }

        private static void DropFks(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPhone_ServiceChargeType_ServiceChargeTypeId",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPhone_PhoneNumberType_TypeId",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelPhone_PhoneNumberType_TypeId",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelServiceHours_ExceptionHoursStatusType_ExceptionHoursTypeId",
                schema: "public",
                table: "ServiceChannelServiceHours");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
