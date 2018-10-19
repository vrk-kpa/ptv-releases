using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class IsASTIConnection : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsASTIConnection",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                defaultValue: false);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsASTIConnection",
                schema: "public",
                table: "ServiceServiceChannel");
        }
    }
}
