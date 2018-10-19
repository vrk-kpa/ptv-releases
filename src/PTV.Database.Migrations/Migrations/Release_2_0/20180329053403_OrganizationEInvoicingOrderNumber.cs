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
