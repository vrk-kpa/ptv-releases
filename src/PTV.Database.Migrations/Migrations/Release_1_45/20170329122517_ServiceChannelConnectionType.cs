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
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_45
{
    public partial class ServiceChannelConnectionType : IPartialMigration
    {
        private MigrateHelper migrateHelper;

        public ServiceChannelConnectionType()
        {
            this.migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "4ServiceChannelConnectionType.sql"));

            migrationBuilder.CreateTable(
                name: "ServiceChannelConnectionType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelConnectionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelConnectionTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelConnectionTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChannelConnectionTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelConnectionTypeName_ServiceChannelConnectionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelConnectionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
               name: "ConnectionTypeId",
               schema: "public",
               table: "ServiceChannelVersioned",
               nullable: false,
               defaultValueSql: @"GetOrCreateDefaultServiceChannelConnectionTypeId('NotCommon')"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_ConnectionTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "ConnectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaConTyp_Id",
                schema: "public",
                table: "ServiceChannelConnectionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaConTypNam_Id",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaConTypNam_LocalizationId",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaConTypNam_TypeId",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelConnectionType_ConnectionTypeId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "ConnectionTypeId",
                principalSchema: "public",
                principalTable: "ServiceChannelConnectionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelConnectionType_ConnectionTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropTable(
                name: "ServiceChannelConnectionTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelConnectionType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_ConnectionTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "ConnectionTypeId",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
