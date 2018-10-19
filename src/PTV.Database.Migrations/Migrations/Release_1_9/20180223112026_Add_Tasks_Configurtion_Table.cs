using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class Add_Tasks_Configurtion_Table : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TasksConfiguration",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Entity = table.Column<string>(type: "text", nullable: true),
                    LastOperationId = table.Column<string>(type: "text", nullable: true),
                    Modified = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    PublishingStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Weeks = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksConfiguration", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TasCon_Id",
                schema: "public",
                table: "TasksConfiguration",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TasksConfiguration",
                schema: "public");
        }
    }
}
