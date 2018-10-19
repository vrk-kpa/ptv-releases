using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_5
{
    public partial class AreaIsValid : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "Area",
                nullable: false,
                defaultValue: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
