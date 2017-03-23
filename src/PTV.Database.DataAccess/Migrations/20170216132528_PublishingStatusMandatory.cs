using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PublishingStatusMandatory : Migration
    {
        private void AddSqlScripts(MigrationBuilder migrationBuilder)
        {
            var directory = new DirectoryInfo(@"SqlMigrations\PTV_1_4_1");
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles("*.sql"))
                {
                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
                }
            }
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddSqlScripts(migrationBuilder);

            migrationBuilder.AlterColumn<Guid>(
                name: "PublishingStatusId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PublishingStatusId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PublishingStatusId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PublishingStatusId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
