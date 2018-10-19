using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class TrackingEntityTablesUpdate : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingServiceChannelVersioned",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TrackingServiceVersioned",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "TrackingEntityVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UnificRootId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    OperationType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingEntityVersioned", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TraEntVer_Id",
                schema: "public",
                table: "TrackingEntityVersioned",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingEntityVersioned",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "TrackingServiceChannelVersioned",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OperationType = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ServiceChannelId = table.Column<Guid>(nullable: false)
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
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OperationType = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingServiceVersioned", x => x.Id);
                });

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
        }
    }
}
