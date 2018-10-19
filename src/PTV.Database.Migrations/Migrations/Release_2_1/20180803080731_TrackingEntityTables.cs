using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class TrackingEntityTables : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "TrackingGeneralDescriptionVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GenerealDescriptionId = table.Column<Guid>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingGeneralDescriptionVersioned", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingOrganizationChannel",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingOrganizationChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingServiceChannelVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingServiceChannelVersioned", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingServiceVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingServiceVersioned", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingTranslationOrder",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TranslationOrderId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingTranslationOrder", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraGenDesVer_Id",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrgCha_Id",
                schema: "public",
                table: "TrackingOrganizationChannel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraSerChaVer_Id",
                schema: "public",
                table: "TrackingServiceChannelVersioned",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraSerVer_Id",
                schema: "public",
                table: "TrackingServiceVersioned",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraTraOrd_Id",
                schema: "public",
                table: "TrackingTranslationOrder",
                column: "Id");           
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
