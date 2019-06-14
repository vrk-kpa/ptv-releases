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

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class OrgRestrictionFilter : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestrictionFilter",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EntityType = table.Column<string>(nullable: true),
                    FilterName = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    TypeValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationFilter",
                schema: "public",
                columns: table => new
                {
                    FilterId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationFilter", x => new { x.FilterId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_OrganizationFilter_RestrictionFilter_FilterId",
                        column: x => x.FilterId,
                        principalSchema: "public",
                        principalTable: "RestrictionFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationFilter_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "public",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgFil_FilterId",
                schema: "public",
                table: "OrganizationFilter",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgFil_OrganizationId",
                schema: "public",
                table: "OrganizationFilter",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_Id",
                schema: "public",
                table: "RestrictionFilter",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationFilter",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RestrictionFilter",
                schema: "public");
        }
    }
}
