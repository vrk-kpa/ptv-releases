using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class serviceCollectionService : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.RenameColumn(
                   name: "ServiceCollectionVersionedId",
                   schema: "public",
                   table: "ServiceCollectionService",
                   newName: "ServiceCollectionId");           

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService",
                columns: new[] { "ServiceCollectionId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerColSer_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollection_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionId",
                principalSchema: "public",
                principalTable: "ServiceCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollection_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropIndex(
                name: "IX_SerColSer_ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropColumn(
                name: "ServiceCollectionId",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceCollectionService",
                schema: "public",
                table: "ServiceCollectionService",
                columns: new[] { "ServiceCollectionVersionedId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionVersionedId",
                principalSchema: "public",
                principalTable: "ServiceCollectionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
