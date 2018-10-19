using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class AccessibilityClassificationLevel : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccessibilityClassificationLevelType",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityClassificationLevelType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WcagLevelType",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WcagLevelType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityClassificationLevelTypeName",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityClassificationLevelTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AccClaLevTypNam_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccClaLevTypNam_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AccessibilityClassificationLevelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WcagLevelTypeName",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WcagLevelTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_WcagLevelTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WcagLevelTypeName_WcagLevelType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "WcagLevelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AccessibilityClassificationLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "WcagLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccClaLevTyp_Id",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccClaLevTypNam_LocalizationId",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccClaLevTypNam_TypeId",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WcaLevTyp_Id",
                schema: "public",
                table: "WcagLevelType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WcaLevTypNam_LocalizationId",
                schema: "public",
                table: "WcagLevelTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WcaLevTypNam_TypeId",
                schema: "public",
                table: "WcagLevelTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SerChaVer_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "AccessibilityClassificationLevelTypeId",
                principalSchema: "public",
                principalTable: "AccessibilityClassificationLevelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_WcagLevelType_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "WcagLevelTypeId",
                principalSchema: "public",
                principalTable: "WcagLevelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerChaVer_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_WcagLevelType_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropTable(
                name: "AccessibilityClassificationLevelTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WcagLevelTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessibilityClassificationLevelType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WcagLevelType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "AccessibilityClassificationLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "WcagLevelTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
