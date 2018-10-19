using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_75
{
    public partial class AddressPostalCodeNotMandatory : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AddressPostalCodeNotMandatory()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

           migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "1AddressPostalCode.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStreet_PostalCode_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId",
                principalSchema: "public",
                principalTable: "PostalCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
