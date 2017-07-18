using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ValidityFixAndBackgroundDesc : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "OpeningHoursTo");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "OpeningHoursFrom");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccessRightType",
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
                    table.PrimaryKey("PK_AccessRightType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessRightName",
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
                    table.PrimaryKey("PK_AccessRightName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessRightName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRightName_AccessRightType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionBlockedAccessRight_StatutoryServiceGeneralDescription_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    StatutoryServiceGeneralDescriptionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_ChannelBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelBlockedAccessRight_ServiceChannel_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelBlockedAccessRight_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                        column: x => x.StatutoryServiceGeneralDescriptionId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_OrganizationBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationBlockedAccessRight_Organization_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceBlockedAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessBlockedId = table.Column<Guid>(nullable: false),
                    EntityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBlockedAccessRight", x => new { x.AccessBlockedId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_ServiceBlockedAccessRight_AccessRightType_AccessBlockedId",
                        column: x => x.AccessBlockedId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceBlockedAccessRight_Service_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "public",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessRight",
                schema: "public",
                columns: table => new
                {
                    AccessRightId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRight", x => new { x.AccessRightId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserAccessRight_AccessRightType_AccessRightId",
                        column: x => x.AccessRightId,
                        principalSchema: "public",
                        principalTable: "AccessRightType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccRigNam_Id",
                schema: "public",
                table: "AccessRightName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRigNam_LocalizationId",
                schema: "public",
                table: "AccessRightName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRigNam_TypeId",
                schema: "public",
                table: "AccessRightName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRigTyp_Id",
                schema: "public",
                table: "AccessRightType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesBloAccRig_AccessBlockedId",
                schema: "public",
                table: "GeneralDescriptionBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesBloAccRig_EntityId",
                schema: "public",
                table: "GeneralDescriptionBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_EntityId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChaBloAccRig_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "ChannelBlockedAccessRight",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgBloAccRig_AccessBlockedId",
                schema: "public",
                table: "OrganizationBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgBloAccRig_EntityId",
                schema: "public",
                table: "OrganizationBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerBloAccRig_AccessBlockedId",
                schema: "public",
                table: "ServiceBlockedAccessRight",
                column: "AccessBlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerBloAccRig_EntityId",
                schema: "public",
                table: "ServiceBlockedAccessRight",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UseAccRig_AccessRightId_UserId",
                schema: "public",
                table: "UserAccessRight",
                columns: new[] { "AccessRightId", "UserId" });
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRightName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ChannelBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OrganizationBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceBlockedAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserAccessRight",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessRightType",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");

            migrationBuilder.RenameColumn(
                name: "OpeningHoursTo",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "OpeningHoursFrom",
                schema: "public",
                table: "ServiceChannelServiceHours",
                newName: "ValidFrom");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "WebPage",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "WebPage",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "UserOrganization",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "UserOrganization",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "Business",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "Attachment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "Attachment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "Address",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "Address",
                nullable: true);
        }
    }
}
