using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class RestrictionFilterTypeChange : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeValue",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeValue",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilterName",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                schema: "public",
                table: "RestrictionFilter",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_EntityType",
                schema: "public",
                table: "RestrictionFilter",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_FilterName",
                schema: "public",
                table: "RestrictionFilter",
                column: "FilterName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResFil_TypeName",
                schema: "public",
                table: "RestrictionFilter",
                column: "TypeName");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResFil_EntityType",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.DropIndex(
                name: "IX_ResFil_FilterName",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.DropIndex(
                name: "IX_ResFil_TypeName",
                schema: "public",
                table: "RestrictionFilter");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TypeValue",
                schema: "public",
                table: "RestrictionFilter",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "public",
                table: "RestrictionFilter",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FilterName",
                schema: "public",
                table: "RestrictionFilter",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "EntityType",
                schema: "public",
                table: "RestrictionFilter",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_SerColSer_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCollectionService_ServiceCollectionVersioned_ServiceCollectionVersionedId",
                schema: "public",
                table: "ServiceCollectionService",
                column: "ServiceCollectionVersionedId",
                principalSchema: "public",
                principalTable: "ServiceCollectionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
