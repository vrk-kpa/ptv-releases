using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class AccessibilityRegisterUpdate : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessibilityRegisterGroup_AccessibilityRegister_AccessibilityRegisterId",
                schema: "public",
                table: "AccessibilityRegisterGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccRegGro_AccessibilityRegisterId",
                schema: "public",
                table: "AccessibilityRegisterGroup");

            migrationBuilder.DropColumn(
                name: "AccessibilityRegisterId",
                schema: "public",
                table: "AccessibilityRegisterGroup");

            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                schema: "public",
                table: "Address",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AccessibilityRegisterEntranceId",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                schema: "public",
                table: "AccessibilityRegister",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterEntrance",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessibilityRegisterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    EntranceId = table.Column<int>(type: "int4", nullable: false),
                    IsMain = table.Column<bool>(type: "bool", nullable: false),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    OrderNumber = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterEntrance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterEntrance_AccessibilityRegister_AccessibilityRegisterId",
                        column: x => x.AccessibilityRegisterId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegister",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterEntrance_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterEntranceName",
                schema: "public",
                columns: table => new
                {
                    AccessibilityRegisterEntranceId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterEntranceName", x => new { x.AccessibilityRegisterEntranceId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterEntranceName_AccessibilityRegisterEntrance_AccessibilityRegisterEntranceId",
                        column: x => x.AccessibilityRegisterEntranceId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegisterEntrance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterEntranceName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccRegGro_AccessibilityRegisterEntranceId",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                column: "AccessibilityRegisterEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegEnt_AccessibilityRegisterId",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                column: "AccessibilityRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegEnt_AddressId",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegEnt_Id",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegEntNam_AccessibilityRegisterEntranceId",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                column: "AccessibilityRegisterEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegEntNam_LocalizationId",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                column: "LocalizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessibilityRegisterGroup_AccessibilityRegisterEntrance_AccessibilityRegisterEntranceId",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                column: "AccessibilityRegisterEntranceId",
                principalSchema: "public",
                principalTable: "AccessibilityRegisterEntrance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // set UnificId for addresses
            migrationBuilder.Sql(@"UPDATE ""Address"" SET ""UniqueId"" = ""Id""");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
