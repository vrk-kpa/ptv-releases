using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class ServiceCollection : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCollection",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_ServiceCollectionBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionBlockedAccessRight_ServiceCollection_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "ServiceCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IsVisibleForAll = table.Column<bool>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    PublishingStatusId = table.Column<Guid>(nullable: false),
                    UnificRootId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true),
                    VersioningId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionVersioned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionVersioned_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionVersioned_PublishingStatusType_PublishingStatusId",
                        column: x => x.PublishingStatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionVersioned_ServiceCollection_UnificRootId",
                        column: x => x.UnificRootId,
                        principalSchema: "public",
                        principalTable: "ServiceCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionVersioned_Versioning_VersioningId",
                        column: x => x.VersioningId,
                        principalSchema: "public",
                        principalTable: "Versioning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionDescription",
                schema: "public",
                columns: table => new
                {
                    ServiceCollectionVersionedId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionDescription", x => new { x.ServiceCollectionVersionedId, x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ServiceCollectionDescription_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionDescription_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                        column: x => x.ServiceCollectionVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceCollectionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionDescription_DescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "DescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionLanguageAvailability",
                schema: "public",
                columns: table => new
                {
                    ServiceCollectionVersionedId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionLanguageAvailability", x => new { x.ServiceCollectionVersionedId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ServiceCollectionLanguageAvailability_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionLanguageAvailability_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                        column: x => x.ServiceCollectionVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceCollectionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionLanguageAvailability_PublishingStatusType_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "public",
                        principalTable: "PublishingStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionName",
                schema: "public",
                columns: table => new
                {
                    ServiceCollectionVersionedId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionName", x => new { x.ServiceCollectionVersionedId, x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ServiceCollectionName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionName_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                        column: x => x.ServiceCollectionVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceCollectionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionName_NameType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCollectionService",
                schema: "public",
                columns: table => new
                {
                    ServiceCollectionVersionedId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCollectionService", x => new { x.ServiceCollectionVersionedId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                        column: x => x.ServiceCollectionVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceCollectionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCollectionService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerCol_Id",
                schema: "public",
                table: "ServiceCollection",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerColBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ServiceCollectionBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColBloAccRig_EntityId",
                schema: "public",
                table: "ServiceCollectionBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColDes_LocalizationId",
                schema: "public",
                table: "ServiceCollectionDescription",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColDes_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionDescription",
                column: "ServiceCollectionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColDes_TypeId",
                schema: "public",
                table: "ServiceCollectionDescription",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColLanAva_LanguageId",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColLanAva_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                column: "ServiceCollectionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColLanAva_StatusId",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColNam_LocalizationId",
                schema: "public",
                table: "ServiceCollectionName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColNam_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionName",
                column: "ServiceCollectionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColNam_TypeId",
                schema: "public",
                table: "ServiceCollectionName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColSer_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColSer_ServiceId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_Id",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_OrganizationId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_PublishingStatusId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "PublishingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_UnificRootId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "UnificRootId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_VersioningId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "VersioningId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceCollectionBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionDescription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionLanguageAvailability",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionService",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollectionVersioned",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCollection",
                schema: "public");
        }
    }
}
