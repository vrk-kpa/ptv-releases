using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations
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
