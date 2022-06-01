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
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_0
{
    public partial class RemoveUnusedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Removing unused tables from DB.");
            
            migrationBuilder.DropTable(
                name: "BugReport",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Form",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FormTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelDigitalAuthorization",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelExtraTypeDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OntologyTermDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelServiceClass",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TasksFilter",
                schema: "public");

            migrationBuilder.DropTable(
                name: "FormType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionServiceChannelExtraType",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReport",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Actions = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FinalState = table.Column<string>(type: "text", nullable: true),
                    InitialState = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    OrderNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelDescription",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelDescription", x => new { x.TypeId, x.LocalizationId, x.ServiceChannelId, x.StatutoryServiceGeneralDescriptionId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelDigitalAuthorization",
                schema: "public",
                columns: table => new
                {
                    DigitalAuthorizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelDigitalAuthorization", x => new { x.DigitalAuthorizationId, x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDigAut_DigitalAuthorizationId",
                        column: x => x.DigitalAuthorizationId,
                        principalSchema: "public",
                        principalTable: "DigitalAuthorization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                        columns: x => new { x.StatutoryServiceGeneralDescriptionId, x.ServiceChannelId },
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannel",
                        principalColumns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionServiceChannelExtraType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ExtraSubTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ServiceChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelExtraType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_ExtraSubType_ExtraSubTypeId",
                        column: x => x.ExtraSubTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraSubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTyp_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTermDescription",
                schema: "public",
                columns: table => new
                {
                    OntologyTermId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTermDescription", x => new { x.OntologyTermId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_OntologyTermDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OntologyTermDescription_OntologyTerm_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalSchema: "public",
                        principalTable: "OntologyTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelServiceClass",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelServiceClass", x => new { x.ServiceChannelVersionedId, x.ServiceClassId });
                    table.ForeignKey(
                        name: "FK_SerChaSerCla_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
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
                name: "TasksFilter",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    EntityModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksFilter", x => new { x.TypeId, x.UserId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_TasksFilter_TasksConfiguration_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "TasksConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Form",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypeName", x => new { x.TypeId, x.LocalizationId });
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
                name: "GeneralDescriptionServiceChannelExtraTypeDescription",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneralDescriptionServiceChannelExtraTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastOperationType = table.Column<int>(type: "integer", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionServiceChannelExtraTypeDescription", x => new { x.LocalizationId, x.GeneralDescriptionServiceChannelExtraTypeId });
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId",
                        column: x => x.GeneralDescriptionServiceChannelExtraTypeId,
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionServiceChannelExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenDesSerChaExtTypDes_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugRep_Id",
                schema: "public",
                table: "BugReport",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_For_Id",
                schema: "public",
                table: "Form",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_For_LocalizationId",
                schema: "public",
                table: "Form",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_For_TypeId",
                schema: "public",
                table: "Form",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForTyp_Id",
                schema: "public",
                table: "FormType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ForTypNam_LocalizationId",
                schema: "public",
                table: "FormTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ForTypNam_TypeId",
                schema: "public",
                table: "FormTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_LocalizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_TypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDigAut_DigitalAuthorizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                column: "DigitalAuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_ExtraSubTypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "ExtraSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_Id",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTyp_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                column: "GeneralDescriptionServiceChannelExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesSerChaExtTypDes_LocalizationId",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerDes_LocalizationId",
                schema: "public",
                table: "OntologyTermDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerDes_OntologyTermId",
                schema: "public",
                table: "OntologyTermDescription",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaSerCla_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaSerCla_ServiceClassId",
                schema: "public",
                table: "ServiceChannelServiceClass",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TasFil_TypeId_UserId_EntityId",
                schema: "public",
                table: "TasksFilter",
                columns: new[] { "TypeId", "UserId", "EntityId" });
        }
    }
}
