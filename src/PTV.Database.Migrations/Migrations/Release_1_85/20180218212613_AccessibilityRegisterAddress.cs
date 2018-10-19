using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_85
{
    public partial class AccessibilityRegisterAddress : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccReg_AddressId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessibilityRegister_Address_AddressId",
                schema: "public",
                table: "AccessibilityRegister",
                column: "AddressId",
                principalSchema: "public",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
