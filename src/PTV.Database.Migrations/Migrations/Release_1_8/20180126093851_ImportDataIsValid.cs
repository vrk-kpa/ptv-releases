using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class ImportDataIsValid : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "TargetGroup",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "ServiceClass",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "OrganizationType",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "OntologyTerm",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "LifeEvent",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "Language",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "IndustrialClass",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: false,
                defaultValue: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "TargetGroup");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "OntologyTerm");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "LifeEvent");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "IndustrialClass");

            migrationBuilder.DropColumn(
                name: "IsValid",
                schema: "public",
                table: "DigitalAuthorization");
        }
    }
}
