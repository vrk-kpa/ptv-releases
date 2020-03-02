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

namespace PTV.Database.Migrations.Migrations.Release_2_2_1
{
    public partial class ClsAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClsAddressStreet",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsAddressStreet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClsAddressStreet_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClsAddressStreetName",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    ClsAddressStreetId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsAddressStreetName", x => new { x.ClsAddressStreetId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ClsAddressStreetName_ClsAddressStreet_ClsAddressStreetId",
                        column: x => x.ClsAddressStreetId,
                        principalSchema: "public",
                        principalTable: "ClsAddressStreet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClsAddressStreetName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClsAddressStreetNumber",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ClsAddressStreetId = table.Column<Guid>(nullable: false),
                    PostalCodeId = table.Column<Guid>(nullable: false),
                    IsEven = table.Column<bool>(nullable: false),
                    StartNumber = table.Column<int>(nullable: false),
                    StartCharacter = table.Column<string>(nullable: true),
                    StartNumberEnd = table.Column<int>(nullable: false),
                    StartCharacterEnd = table.Column<string>(nullable: true),
                    EndNumber = table.Column<int>(nullable: false),
                    EndCharacter = table.Column<string>(nullable: true),
                    EndNumberEnd = table.Column<int>(nullable: false),
                    EndCharacterEnd = table.Column<string>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsAddressStreetNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClsAddressStreetNumber_ClsAddressStreet_ClsAddressStreetId",
                        column: x => x.ClsAddressStreetId,
                        principalSchema: "public",
                        principalTable: "ClsAddressStreet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClsAddressStreetNumber_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClsAddressPoint",
                schema: "public",
                columns: table => new
                {
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    PostalCodeId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    AddressStreetId = table.Column<Guid>(nullable: false),
                    AddressStreetNumberId = table.Column<Guid>(nullable: true),
                    AddressId = table.Column<Guid>(nullable: true),
                    StreetNumber = table.Column<string>(maxLength: 30, nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsAddressPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClsAddressPoint_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClsAddressPoint_ClsAddressStreet_AddressStreetId",
                        column: x => x.AddressStreetId,
                        principalSchema: "public",
                        principalTable: "ClsAddressStreet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClsAddressPoint_ClsAddressStreetNumber_AddressStreetNumberId",
                        column: x => x.AddressStreetNumberId,
                        principalSchema: "public",
                        principalTable: "ClsAddressStreetNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClsAddressPoint_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClsAddressPoint_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_AddressId",
                schema: "public",
                table: "ClsAddressPoint",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_AddressStreetId",
                schema: "public",
                table: "ClsAddressPoint",
                column: "AddressStreetId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_AddressStreetNumberId",
                schema: "public",
                table: "ClsAddressPoint",
                column: "AddressStreetNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_Id",
                schema: "public",
                table: "ClsAddressPoint",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_MunicipalityId",
                schema: "public",
                table: "ClsAddressPoint",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddPoi_PostalCodeId",
                schema: "public",
                table: "ClsAddressPoint",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStr_Id",
                schema: "public",
                table: "ClsAddressStreet",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStr_MunicipalityId",
                schema: "public",
                table: "ClsAddressStreet",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNam_ClsAddressStreetId",
                schema: "public",
                table: "ClsAddressStreetName",
                column: "ClsAddressStreetId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNam_LocalizationId",
                schema: "public",
                table: "ClsAddressStreetName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNum_ClsAddressStreetId",
                schema: "public",
                table: "ClsAddressStreetNumber",
                column: "ClsAddressStreetId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNum_Id",
                schema: "public",
                table: "ClsAddressStreetNumber",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClsAddStrNum_PostalCodeId",
                schema: "public",
                table: "ClsAddressStreetNumber",
                column: "PostalCodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClsAddressPoint",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClsAddressStreetName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClsAddressStreetNumber",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClsAddressStreet",
                schema: "public");
        }
    }
}
