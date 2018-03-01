using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    public partial class TranslationOrderRootId : IPartialMigration
    {
        private readonly DataUtils dataUtils;

        public TranslationOrderRootId()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_1_8", "5DeleteTranslationData.sql")
            );
            
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralDescriptionTranslationOrder_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelTranslationOrder_ServiceChannelVersioned_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTranslationOrder_ServiceVersioned_ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceTranslationOrder",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_SerTraOrd_ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelTranslationOrder",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_SerChaTraOrd_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralDescriptionTranslationOrder",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.RenameColumn(
                name: "ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder",
                newName: "ServiceVersionedIdentifier");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                newName: "ServiceChannelIdentifier");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                newName: "StatutoryServiceGeneralDescriptionIdentifier");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessCode",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationIdentifier",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceEntityName",
                schema: "public",
                table: "TranslationOrder",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceLanguageCharAmount",
                schema: "public",
                table: "TranslationOrder",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceTranslationOrder",
                schema: "public",
                table: "ServiceTranslationOrder",
                columns: new[] { "ServiceId", "TranslationOrderId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelTranslationOrder",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                columns: new[] { "ServiceChannelId", "TranslationOrderId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralDescriptionTranslationOrder",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                columns: new[] { "StatutoryServiceGeneralDescriptionId", "TranslationOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerTraOrd_ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaTraOrd_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "StatutoryServiceGeneralDescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralDescriptionTranslationOrder_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "StatutoryServiceGeneralDescriptionId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelTranslationOrder_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTranslationOrder_Service_ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralDescriptionTranslationOrder_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelTranslationOrder_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTranslationOrder_Service_ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceTranslationOrder",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_SerTraOrd_ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelTranslationOrder",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_SerChaTraOrd_ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneralDescriptionTranslationOrder",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropIndex(
                name: "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessCode",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationIdentifier",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "OrganizationName",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "SourceEntityName",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "SourceLanguageCharAmount",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropColumn(
                name: "ServiceChannelId",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropColumn(
                name: "StatutoryServiceGeneralDescriptionId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.RenameColumn(
                name: "ServiceVersionedIdentifier",
                schema: "public",
                table: "ServiceTranslationOrder",
                newName: "ServiceVersionedId");

            migrationBuilder.RenameColumn(
                name: "ServiceChannelIdentifier",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                newName: "ServiceChannelVersionedId");

            migrationBuilder.RenameColumn(
                name: "StatutoryServiceGeneralDescriptionIdentifier",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                newName: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceTranslationOrder",
                schema: "public",
                table: "ServiceTranslationOrder",
                columns: new[] { "ServiceVersionedId", "TranslationOrderId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelTranslationOrder",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                columns: new[] { "ServiceChannelVersionedId", "TranslationOrderId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneralDescriptionTranslationOrder",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                columns: new[] { "StatutoryServiceGeneralDescriptionVersionedId", "TranslationOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerTraOrd_ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaTraOrd_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralDescriptionTranslationOrder_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "StatutoryServiceGeneralDescriptionVersionedId",
                principalSchema: "public",
                principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelTranslationOrder_ServiceChannelVersioned_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "ServiceChannelVersionedId",
                principalSchema: "public",
                principalTable: "ServiceChannelVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTranslationOrder_ServiceVersioned_ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "ServiceVersionedId",
                principalSchema: "public",
                principalTable: "ServiceVersioned",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
