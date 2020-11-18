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

namespace PTV.Database.Migrations.Migrations.Release_2_4
{
    public partial class ClsStreetNumberCoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Coordinate", schema: "public", newName: "AddressCoordinate");
            migrationBuilder.RenameColumn(table: "AddressCoordinate", schema: "public", name: "AddressId", newName: "RelatedToId");

            migrationBuilder.CreateTable(
                name: "ClsStreetNumberCoordinate",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    CoordinateState = table.Column<string>(nullable: true),
                    RelatedToId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsStreetNumberCoordinate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClsStreetNumberCoordinate_ClsAddressStreetNumber_RelatedToId",
                        column: x => x.RelatedToId,
                        principalSchema: "public",
                        principalTable: "ClsAddressStreetNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClsStreetNumberCoordinate_CoordinateType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "CoordinateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClsStrNumCoo_Id",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClsStrNumCoo_RelatedToId",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                column: "RelatedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ClsStrNumCoo_TypeId",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
