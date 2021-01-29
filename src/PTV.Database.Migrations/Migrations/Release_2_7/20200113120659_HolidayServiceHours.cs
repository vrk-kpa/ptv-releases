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

namespace PTV.Database.Migrations.Migrations.Release_2_7
{
    public partial class HolidayServiceHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holiday",
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
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayDate",
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
                    Date = table.Column<DateTime>(nullable: false),
                    HolidayId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayDate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayDate_Holiday_HolidayId",
                        column: x => x.HolidayId,
                        principalSchema: "public",
                        principalTable: "Holiday",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HolidayName",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_HolidayName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayName_Holiday_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "Holiday",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HolidayServiceHours",
                schema: "public",
                columns: table => new
                {
                    HolidayId = table.Column<Guid>(nullable: false),
                    ServiceHoursId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayServiceHours", x => new { x.HolidayId, x.ServiceHoursId });
                    table.ForeignKey(
                        name: "FK_HolidayServiceHours_Holiday_HolidayId",
                        column: x => x.HolidayId,
                        principalSchema: "public",
                        principalTable: "Holiday",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayServiceHours_ServiceHours_ServiceHoursId",
                        column: x => x.ServiceHoursId,
                        principalSchema: "public",
                        principalTable: "ServiceHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hol_Id",
                schema: "public",
                table: "Holiday",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HolDat_HolidayId",
                schema: "public",
                table: "HolidayDate",
                column: "HolidayId");

            migrationBuilder.CreateIndex(
                name: "IX_HolDat_Id",
                schema: "public",
                table: "HolidayDate",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HolNam_LocalizationId",
                schema: "public",
                table: "HolidayName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_HolNam_TypeId",
                schema: "public",
                table: "HolidayName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HolSerHou_HolidayId",
                schema: "public",
                table: "HolidayServiceHours",
                column: "HolidayId");

            migrationBuilder.CreateIndex(
                name: "IX_HolSerHou_ServiceHoursId",
                schema: "public",
                table: "HolidayServiceHours",
                column: "ServiceHoursId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HolSerHou_ServiceHoursId",
                schema: "public",
                table: "HolidayServiceHours");
            
            migrationBuilder.DropTable(
                name: "HolidayDate",
                schema: "public");

            migrationBuilder.DropTable(
                name: "HolidayName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "HolidayServiceHours",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Holiday",
                schema: "public");
        }
    }
}
