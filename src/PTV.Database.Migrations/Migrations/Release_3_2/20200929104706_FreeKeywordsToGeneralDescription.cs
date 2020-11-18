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

namespace PTV.Database.Migrations.Migrations.Release_3_2
{
    public partial class FreeKeywordsToGeneralDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Adding free keywords to GD.");
            
            migrationBuilder.CreateTable(
                name: "StatutoryServiceGeneralDescriptionKeyword",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionVersionedId = table.Column<Guid>(nullable: false),
                    KeywordId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_StatutoryServiceGeneralDescriptionKeyword", x => new { x.StatutoryServiceGeneralDescriptionVersionedId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_StatutoryServiceGeneralDescriptionKeyword_Keyword_KeywordId",
                        column: x => x.KeywordId,
                        principalSchema: "public",
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaSerGenDesKey_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId",
                        column: x => x.StatutoryServiceGeneralDescriptionVersionedId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDesKey_KeywordId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionKeyword",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_StaSerGenDesKey_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionKeyword",
                column: "StatutoryServiceGeneralDescriptionVersionedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatutoryServiceGeneralDescriptionKeyword",
                schema: "public");
        }
    }
}
