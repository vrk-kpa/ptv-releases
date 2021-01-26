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

namespace PTV.Database.Migrations.Migrations.Release_2_3
{
    public partial class DotNet22ColumnsRevert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddParametrizedColumnRename(migrationBuilder, "StreetName", "AddressStreetAddressId", "AddressStreetId");
            AddParametrizedColumnRename(migrationBuilder, "PostOfficeBoxName", "AddressPostOfficeBoxAddressId", "AddressPostOfficeBoxId");
            AddParametrizedColumnRename(migrationBuilder, "AddressForeignTextName", "AddressForeignAddressId", "AddressForeignId");
        }

        private void AddParametrizedColumnRename(MigrationBuilder migrationBuilder, string table, string oldName, string newName, string schema = "public")
        {
            migrationBuilder.Sql(string.Format(@"
            DO $$
            BEGIN
            IF EXISTS(SELECT * FROM information_schema.columns WHERE table_name='{0}' and column_name='{1}')
            THEN ALTER TABLE ""{3}"".""{0}"" RENAME COLUMN ""{1}"" TO ""{2}"";
            END IF;
            END $$;", table, oldName, newName, schema));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
