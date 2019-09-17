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
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class SoteExtraTypes : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressExtraType",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    ExtraTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressExtraType", x => new { x.AddressId, x.ExtraTypeId });
                    table.ForeignKey(
                        name: "FK_AddressExtraType_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressExtraType_ExtraType_ExtraTypeId",
                        column: x => x.ExtraTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneExtraType",
                schema: "public",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(nullable: false),
                    ExtraTypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneExtraType", x => new { x.PhoneId, x.ExtraTypeId });
                    table.ForeignKey(
                        name: "FK_PhoneExtraType_ExtraType_ExtraTypeId",
                        column: x => x.ExtraTypeId,
                        principalSchema: "public",
                        principalTable: "ExtraType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneExtraType_Phone_PhoneId",
                        column: x => x.PhoneId,
                        principalSchema: "public",
                        principalTable: "Phone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddExtTyp_AddressId",
                schema: "public",
                table: "AddressExtraType",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddExtTyp_ExtraTypeId",
                schema: "public",
                table: "AddressExtraType",
                column: "ExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoExtTyp_ExtraTypeId",
                schema: "public",
                table: "PhoneExtraType",
                column: "ExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoExtTyp_PhoneId",
                schema: "public",
                table: "PhoneExtraType",
                column: "PhoneId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressExtraType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PhoneExtraType",
                schema: "public");
        }
    }
}
