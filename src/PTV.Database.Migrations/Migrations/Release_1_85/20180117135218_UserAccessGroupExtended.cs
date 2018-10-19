using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class UserAccessGroupExtended : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccessRightFlag",
                schema: "public",
                table: "UserAccessRightsGroup",
                type: "int8",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                schema: "public",
                table: "UserAccessRightsGroup",
                type: "text",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessRightFlag",
                schema: "public",
                table: "UserAccessRightsGroup");

            migrationBuilder.DropColumn(
                name: "UserRole",
                schema: "public",
                table: "UserAccessRightsGroup");
        }
    }
}
