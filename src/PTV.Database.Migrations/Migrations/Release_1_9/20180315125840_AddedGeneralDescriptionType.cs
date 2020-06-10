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

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class AddedGeneralDescriptionType : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AddedGeneralDescriptionType()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralDescriptionType",
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
                    table.PrimaryKey("PK_GeneralDescriptionType", x => x.Id);
                });

            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_1_9", "1GeneralDescriptionType.sql")
            );

            migrationBuilder.AddColumn<Guid>(
                name: "GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                defaultValueSql: @"GeneralDescriptionType_Default()");

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionTypeName",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionTypeName_GeneralDescriptionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "GeneralDescriptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDesVer_GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "GeneralDescriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTyp_Id",
                schema: "public",
                table: "GeneralDescriptionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTypNam_LocalizationId",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTypNam_TypeId",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatutoryServiceGeneralDescriptionVersioned_GeneralDescriptionType_GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                column: "GeneralDescriptionTypeId",
                principalSchema: "public",
                principalTable: "GeneralDescriptionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatutoryServiceGeneralDescriptionVersioned_GeneralDescriptionType_GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GeneralDescriptionType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_StaSerGenDesVer_GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");

            migrationBuilder.DropColumn(
                name: "GeneralDescriptionTypeId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");
        }
    }
}
