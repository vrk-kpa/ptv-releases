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
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_5
{
    public partial class APP_DATA_TABLE : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppEnvironmentDataType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEnvironmentDataType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppEnvironmentData",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    FreeText = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEnvironmentData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEnvironmentData_AppEnvironmentDataType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AppEnvironmentDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppEnvironmentDataTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEnvironmentDataTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEnvironmentDataTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppEnvironmentDataTypeName_AppEnvironmentDataType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AppEnvironmentDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDat_Id",
                schema: "public",
                table: "AppEnvironmentData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDat_TypeId",
                schema: "public",
                table: "AppEnvironmentData",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDatTyp_Id",
                schema: "public",
                table: "AppEnvironmentDataType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDatTypNam_Id",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDatTypNam_LocalizationId",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDatTypNam_TypeId",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                column: "TypeId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEnvironmentData",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AppEnvironmentDataTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AppEnvironmentDataType",
                schema: "public");
        }
    }
}
