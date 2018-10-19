using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class ChangeForeignKeyForTasksFilterTable : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksFilter_AppEnvironmentDataType_TypeId",
                schema: "public",
                table: "TasksFilter");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksFilter_TasksConfiguration_TypeId",
                schema: "public",
                table: "TasksFilter",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "TasksConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksFilter_TasksConfiguration_TypeId",
                schema: "public",
                table: "TasksFilter");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksFilter_AppEnvironmentDataType_TypeId",
                schema: "public",
                table: "TasksFilter",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "AppEnvironmentDataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
