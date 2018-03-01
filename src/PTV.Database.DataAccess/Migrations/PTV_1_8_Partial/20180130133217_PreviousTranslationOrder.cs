using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    public partial class PreviousTranslationOrder : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraOrd_PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder",
                column: "PreviousTranslationOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationOrder_TranslationOrder_PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder",
                column: "PreviousTranslationOrderId",
                principalSchema: "public",
                principalTable: "TranslationOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslationOrder_TranslationOrder_PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_TraOrd_PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "PreviousTranslationOrderId",
                schema: "public",
                table: "TranslationOrder");
        }
    }
}
