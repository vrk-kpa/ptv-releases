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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_5
{
    public partial class RemoveOldStreets : Migration
    {
        private readonly MigrateHelper migrateHelper;

        public RemoveOldStreets()
        {
            migrateHelper = new MigrateHelper();
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var file = Path.Combine("SqlMigrations", "PTV_2_5", "1RemoveOldStreets.sql");
            migrateHelper.AddSqlScript(migrationBuilder, file);
            
            migrationBuilder.DropTable(
                name: "StreetName",
                schema: "public");
            
            migrationBuilder.DropTable(
                name: "AddressStreet",
                schema: "public");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressStreet",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    MunicipalityId = table.Column<Guid>(nullable: true),
                    PostalCodeId = table.Column<Guid>(nullable: true),
                    StreetNumber = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressStreet", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_AddressStreet_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressStreet_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AddressStreet_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StreetName",
                schema: "public",
                columns: table => new
                {
                    AddressStreetId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetName", x => new { x.AddressStreetId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_StreetName_AddressStreet_AddressStreetId",
                        column: x => x.AddressStreetId,
                        principalSchema: "public",
                        principalTable: "AddressStreet",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreetName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_AddressId",
                schema: "public",
                table: "AddressStreet",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_MunicipalityId",
                schema: "public",
                table: "AddressStreet",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StrNam_AddressStreetId",
                schema: "public",
                table: "StreetName",
                column: "AddressStreetId");

            migrationBuilder.CreateIndex(
                name: "IX_StrNam_LocalizationId",
                schema: "public",
                table: "StreetName",
                column: "LocalizationId");
        }
    }
}
