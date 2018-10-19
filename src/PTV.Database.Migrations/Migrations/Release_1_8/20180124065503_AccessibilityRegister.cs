using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class AccessibilityRegister : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessibilityRegister",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ServiceChannelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRegister", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessibilityRegister_ServiceChannel_ServiceChannelId",
                        column: x => x.ServiceChannelId,
                        principalSchema: "public",
                        principalTable: "ServiceChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccReg_Id",
                schema: "public",
                table: "AccessibilityRegister",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccReg_ServiceChannelId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "ServiceChannelId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessibilityRegister",
                schema: "public");
        }
    }
}
