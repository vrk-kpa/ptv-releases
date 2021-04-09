/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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

namespace PTV.Database.Migrations.Migrations.Release_2_2_1
{
    public partial class LangAvailArchivedRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "SetForArchivedBy");

            migrationBuilder.RenameColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "SetForArchived");

            migrationBuilder.RenameColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "SetForArchivedBy");

            migrationBuilder.RenameColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "SetForArchived");

            migrationBuilder.RenameColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "SetForArchivedBy");

            migrationBuilder.RenameColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "SetForArchived");

            migrationBuilder.RenameColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "SetForArchivedBy");

            migrationBuilder.RenameColumn(
                name: "Archived",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "SetForArchived");

            migrationBuilder.RenameColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "SetForArchivedBy");

            migrationBuilder.RenameColumn(
                name: "Archived",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "SetForArchived");

            migrationBuilder.Sql(@"UPDATE ""ServiceLanguageAvailability"" SET ""SetForArchived"" =  ""Reviewed"", ""SetForArchivedBy"" =  ""ReviewedBy""  WHERE ""ArchiveAt"" IS NOT NULL AND ""ReviewedBy"" <> '' AND ""ReviewedBy"" IS NOT NULL;");
            migrationBuilder.Sql(@"UPDATE ""ServiceChannelLanguageAvailability"" SET ""SetForArchived"" =  ""Reviewed"", ""SetForArchivedBy"" =  ""ReviewedBy""  WHERE ""ArchiveAt"" IS NOT NULL AND ""ReviewedBy"" <> '' AND ""ReviewedBy"" IS NOT NULL;");
            migrationBuilder.Sql(@"UPDATE ""ServiceCollectionLanguageAvailability"" SET ""SetForArchived"" =  ""Reviewed"", ""SetForArchivedBy"" =  ""ReviewedBy""  WHERE ""ArchiveAt"" IS NOT NULL AND ""ReviewedBy"" <> '' AND ""ReviewedBy"" IS NOT NULL;");
            migrationBuilder.Sql(@"UPDATE ""OrganizationLanguageAvailability"" SET ""SetForArchived"" =  ""Reviewed"", ""SetForArchivedBy"" =  ""ReviewedBy""  WHERE ""ArchiveAt"" IS NOT NULL AND ""ReviewedBy"" <> '' AND ""ReviewedBy"" IS NOT NULL;");
            migrationBuilder.Sql(@"UPDATE ""GeneralDescriptionLanguageAvailability"" SET ""SetForArchived"" =  ""Reviewed"", ""SetForArchivedBy"" =  ""ReviewedBy""  WHERE ""ArchiveAt"" IS NOT NULL AND ""ReviewedBy"" <> '' AND ""ReviewedBy"" IS NOT NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SetForArchivedBy",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "ArchivedBy");

            migrationBuilder.RenameColumn(
                name: "SetForArchived",
                schema: "public",
                table: "ServiceLanguageAvailability",
                newName: "Archived");

            migrationBuilder.RenameColumn(
                name: "SetForArchivedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "ArchivedBy");

            migrationBuilder.RenameColumn(
                name: "SetForArchived",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                newName: "Archived");

            migrationBuilder.RenameColumn(
                name: "SetForArchivedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "ArchivedBy");

            migrationBuilder.RenameColumn(
                name: "SetForArchived",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                newName: "Archived");

            migrationBuilder.RenameColumn(
                name: "SetForArchivedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "ArchivedBy");

            migrationBuilder.RenameColumn(
                name: "SetForArchived",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                newName: "Archived");

            migrationBuilder.RenameColumn(
                name: "SetForArchivedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "ArchivedBy");

            migrationBuilder.RenameColumn(
                name: "SetForArchived",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                newName: "Archived");
        }
    }
}
