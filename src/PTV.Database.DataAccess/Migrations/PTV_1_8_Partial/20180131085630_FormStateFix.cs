using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
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
