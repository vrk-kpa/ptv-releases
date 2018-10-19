using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class LanguageAvailabilityMassChanges : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Reviewed",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reviewed",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reviewed",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reviewed",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Reviewed",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reviewed",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");
        }
    }
}
