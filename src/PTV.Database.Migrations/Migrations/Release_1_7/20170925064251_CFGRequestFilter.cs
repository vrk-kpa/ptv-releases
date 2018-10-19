using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class CFGRequestFilter : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CFGRequestFilter",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Interface = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    ConcurrentRequests = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFGRequestFilter", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CFGReqFil_Id",
                schema: "public",
                table: "CFGRequestFilter",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CFGRequestFilter",
                schema: "public");
        }
    }
}
