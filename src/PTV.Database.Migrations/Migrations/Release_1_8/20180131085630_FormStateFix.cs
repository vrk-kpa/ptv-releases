using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class FormStateFix : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "public",
                table: "FormState");
            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                schema: "public",
                table: "FormState",
                nullable: false);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
