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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class OrganizationEInvoicingOrderNumber : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public OrganizationEInvoicingOrderNumber()
        {
            migrateHelper = new MigrateHelper();
        }
        
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_2_0", "1RemoveOrganizationEInvoicingAdditionalInformation.sql")
            );
            
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationEInvoicingAdditionalInformation_OrganizationEInvoicing_OrganizationEInvoicingId",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation");
            
            migrationBuilder.DropUniqueConstraint(
                name: "AK_OrganizationEInvoicing_Id",
                schema: "public",
                table: "OrganizationEInvoicing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationEInvoicing",
                schema: "public",
                table: "OrganizationEInvoicing");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationEInvoicing",
                schema: "public",
                table: "OrganizationEInvoicing",
                column: "Id");
            
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationEInvoicingAdditionalInformation_OrganizationEInvoicing_OrganizationEInvoicingId",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                column: "OrganizationEInvoicingId",
                principalSchema: "public",
                principalTable: "OrganizationEInvoicing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationEInvoicing",
                schema: "public",
                table: "OrganizationEInvoicing");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "OrganizationEInvoicing");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_OrganizationEInvoicing_Id",
                schema: "public",
                table: "OrganizationEInvoicing",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationEInvoicing",
                schema: "public",
                table: "OrganizationEInvoicing",
                columns: new[] { "Id", "OrganizationVersionedId" });
        }
    }
}
