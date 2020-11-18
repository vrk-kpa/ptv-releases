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
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Database.Migrations.Migrations.Release_3_0
{
    public partial class ServiceNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Adding table ServiceNumber.");
            
            this.AddMigrationAction(serviceProvider =>
            {
                var translationService = serviceProvider.GetService<IServiceNumberService>();
                translationService.SeedServiceNumbersByJsonFile();
            });
            
            migrationBuilder.CreateTable(
                name: "ServiceNumber",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceNumber", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerNum_Id",
                schema: "public",
                table: "ServiceNumber",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceNumber",
                schema: "public");
        }
    }
}
