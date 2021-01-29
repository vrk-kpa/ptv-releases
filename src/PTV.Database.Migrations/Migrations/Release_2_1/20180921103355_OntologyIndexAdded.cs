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
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class OntologyIndexAdded : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SerCla_ParentUri",
                schema: "public",
                unique: false,
                table: "ServiceClass",
                column: "ParentUri");

            migrationBuilder.CreateIndex(
                name: "IX_SerCla_Uri",
                schema: "public",
                unique: false,
                table: "ServiceClass",
                column: "Uri");

            migrationBuilder.CreateIndex(
                name: "IX_OntTer_ParentUri",
                schema: "public",
                unique: false,
                table: "OntologyTerm",
                column: "ParentUri");

            migrationBuilder.CreateIndex(
                name: "IX_OntTer_Uri",
                schema: "public",
                unique: false,
                table: "OntologyTerm",
                column: "Uri");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SerCla_ParentUri",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropIndex(
                name: "IX_SerCla_Uri",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropIndex(
                name: "IX_OntTer_ParentUri",
                schema: "public",
                table: "OntologyTerm");

            migrationBuilder.DropIndex(
                name: "IX_OntTer_Uri",
                schema: "public",
                table: "OntologyTerm");
        }
    }
}
