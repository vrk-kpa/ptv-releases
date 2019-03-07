/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
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
