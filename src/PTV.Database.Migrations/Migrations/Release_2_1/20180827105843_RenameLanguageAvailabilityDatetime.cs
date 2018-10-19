using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class RenameLanguageAvailabilityDatetime : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "PublishAt");

            migrationBuilder.RenameColumn(
                name: "ValidTo", 
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "ArchiveAt");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "PublishAt");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "ArchiveAt");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "PublishAt");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "ArchiveAt");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "PublishAt");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "ArchiveAt");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "PublishAt");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "ArchiveAt");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublishAt",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "ArchiveAt",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "PublishAt",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "ArchiveAt",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "PublishAt",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "ArchiveAt",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "PublishAt",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "ArchiveAt",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "PublishAt",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "ArchiveAt",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "ValidTo");
        }
    }
}
