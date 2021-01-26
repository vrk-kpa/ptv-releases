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
