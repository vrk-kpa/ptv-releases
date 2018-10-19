using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class Change_of_tasks_configuration_field : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weeks",
                schema: "public",
                table: "TasksConfiguration");

            migrationBuilder.AddColumn<string>(
                name: "Interval",
                schema: "public",
                table: "TasksConfiguration",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interval",
                schema: "public",
                table: "TasksConfiguration");

            migrationBuilder.AddColumn<int>(
                name: "Weeks",
                schema: "public",
                table: "TasksConfiguration",
                nullable: false,
                defaultValue: 0);
        }
    }
}
