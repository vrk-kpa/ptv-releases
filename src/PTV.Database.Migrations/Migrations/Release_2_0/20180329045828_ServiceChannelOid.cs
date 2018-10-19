using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class ServiceChannelOid : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Oid",
                schema: "public",
                table: "ServiceChannelVersioned",
                maxLength: 100,
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Oid",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
