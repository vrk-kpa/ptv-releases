using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class VersioningUnificRootAndMetaInfo : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Meta",
                schema: "public",
                table: "Versioning",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnificRootId",
                schema: "public",
                table: "Versioning",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Meta",
                schema: "public",
                table: "Versioning");

            migrationBuilder.DropColumn(
                name: "UnificRootId",
                schema: "public",
                table: "Versioning");
        }
    }
}
