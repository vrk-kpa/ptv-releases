using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class COUNTRY_CODE_NULLABLE : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId",
                schema: "public",
                table: "Address");

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                schema: "public",
                table: "Address",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Country_CountryId",
                schema: "public",
                table: "Address",
                column: "CountryId",
                principalSchema: "public",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId",
                schema: "public",
                table: "Address");

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                schema: "public",
                table: "Address",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Country_CountryId",
                schema: "public",
                table: "Address",
                column: "CountryId",
                principalSchema: "public",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
