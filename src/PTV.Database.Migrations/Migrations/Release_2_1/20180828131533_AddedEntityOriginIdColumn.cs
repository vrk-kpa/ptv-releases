using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_1
{
    public partial class AddedEntityOriginIdColumn : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_SerColVer_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaVer_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "OriginalId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SerColVer_ServiceCollectionVersioned_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceCollectionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceVersioned_OriginalId",
                schema: "public",
                table: "ServiceVersioned",
                column: "OriginalId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_SerColVer_ServiceCollectionVersioned_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_ServiceVersioned_OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerVer_OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerColVer_OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropIndex(
                name: "IX_SerChaVer_OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                schema: "public",
                table: "ServiceChannelVersioned");
        }
    }
}
