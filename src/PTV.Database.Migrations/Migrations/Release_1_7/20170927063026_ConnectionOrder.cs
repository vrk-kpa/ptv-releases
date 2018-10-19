using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class ConnectionOrder : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "GeneralDescriptionServiceChannel");
        }
    }
}
