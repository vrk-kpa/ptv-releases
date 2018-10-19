using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class AccessibilityRegisterSentences : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterGroup",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccessibilityRegisterId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterGroup_AccessibilityRegister_AccessibilityRegisterId",
                        column: x => x.AccessibilityRegisterId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegister",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterGroupValue",
                schema: "public",
                columns: table => new
                {
                    AccessibilityRegisterGroupId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterGroupValue", x => new { x.AccessibilityRegisterGroupId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterGroupValue_AccessibilityRegisterGroup_AccessibilityRegisterGroupId",
                        column: x => x.AccessibilityRegisterGroupId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegisterGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterGroupValue_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterSentence",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: false),
                    SentenceGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterSentence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterSentence_AccessibilityRegisterGroup_SentenceGroupId",
                        column: x => x.SentenceGroupId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegisterGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityRegisterSentenceValue",
                schema: "public",
                columns: table => new
                {
                    AccessibilityRegisterSentenceId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegisterSentenceValue", x => new { x.AccessibilityRegisterSentenceId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterSentenceValue_AccessibilityRegisterSentence_AccessibilityRegisterSentenceId",
                        column: x => x.AccessibilityRegisterSentenceId,
                        principalSchema: "public",
                        principalTable: "AccessibilityRegisterSentence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegisterSentenceValue_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccRegGro_AccessibilityRegisterId",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                column: "AccessibilityRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegGro_Id",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegGroVal_AccessibilityRegisterGroupId",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                column: "AccessibilityRegisterGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegGroVal_LocalizationId",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegSen_Id",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegSen_SentenceGroupId",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                column: "SentenceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegSenVal_AccessibilityRegisterSentenceId",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                column: "AccessibilityRegisterSentenceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccRegSenVal_LocalizationId",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                column: "LocalizationId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessibilityRegisterGroupValue",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessibilityRegisterSentenceValue",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessibilityRegisterSentence",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AccessibilityRegisterGroup",
                schema: "public");
        }
    }
}
