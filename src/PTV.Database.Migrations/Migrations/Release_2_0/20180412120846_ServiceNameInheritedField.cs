using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class ServiceNameInheritedField : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Inherited",
                schema: "public",
                table: "ServiceName",
                nullable: false,
                defaultValue: false);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inherited",
                schema: "public",
                table: "ServiceName");
        }
    }
}
