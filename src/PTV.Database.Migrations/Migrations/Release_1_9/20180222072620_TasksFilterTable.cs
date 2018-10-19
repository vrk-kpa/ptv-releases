using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class TasksFilterTable : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TasksFilter",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksFilter", x => new { x.TypeId, x.UserId, x.EntityId });
                    table.ForeignKey(
                        name: "FK_TasksFilter_AppEnvironmentDataType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AppEnvironmentDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TasFil_TypeId_UserId_EntityId",
                schema: "public",
                table: "TasksFilter",
                columns: new[] { "TypeId", "UserId", "EntityId" });
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TasksFilter",
                schema: "public");
        }
    }
}
