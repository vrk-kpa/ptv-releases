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
