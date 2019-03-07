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
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_2
{
    public partial class LanguageAvailabilityArchivedChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Archived",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Archived",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Archived",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "Archived",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");
        }
    }
}
