using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    public partial class TranslationOrder : IPartialMigration
    {
        private DataUtils dataUtils;

        public TranslationOrder()
        {
            dataUtils = new DataUtils();
        }


        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslationCompany",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 2500, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Url = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationCompany", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TranslationStateType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationStateType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TranslationOrder",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdditionalInformation = table.Column<string>(maxLength: 2500, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DeliverAt = table.Column<DateTime>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderIdentifier = table.Column<long>(nullable: false),
                    SenderEmail = table.Column<string>(maxLength: 100, nullable: true),
                    SenderName = table.Column<string>(maxLength: 100, nullable: true),
                    SourceLanguageData = table.Column<string>(nullable: true),
                    SourceLanguageDataHash = table.Column<string>(nullable: true),
                    SourceLanguageId = table.Column<Guid>(nullable: false),
                    TargetLanguageData = table.Column<string>(nullable: true),
                    TargetLanguageDataHash = table.Column<string>(nullable: true),
                    TargetLanguageId = table.Column<Guid>(nullable: false),
                    TranslationCompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationOrder_Language_SourceLanguageId",
                        column: x => x.SourceLanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationOrder_Language_TargetLanguageId",
                        column: x => x.TargetLanguageId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationOrder_TranslationCompany_TranslationCompanyId",
                        column: x => x.TranslationCompanyId,
                        principalSchema: "public",
                        principalTable: "TranslationCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationStateTypeName",
                schema: "public",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationStateTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_TranslationStateTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationStateTypeName_TranslationStateType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "TranslationStateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralDescriptionTranslationOrder",
                schema: "public",
                columns: table => new
                {
                    StatutoryServiceGeneralDescriptionVersionedId = table.Column<Guid>(nullable: false),
                    TranslationOrderId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralDescriptionTranslationOrder", x => new { x.StatutoryServiceGeneralDescriptionVersionedId, x.TranslationOrderId });
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionTranslationOrder_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId",
                        column: x => x.StatutoryServiceGeneralDescriptionVersionedId,
                        principalSchema: "public",
                        principalTable: "StatutoryServiceGeneralDescriptionVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralDescriptionTranslationOrder_TranslationOrder_TranslationOrderId",
                        column: x => x.TranslationOrderId,
                        principalSchema: "public",
                        principalTable: "TranslationOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChannelTranslationOrder",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    TranslationOrderId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelTranslationOrder", x => new { x.ServiceChannelVersionedId, x.TranslationOrderId });
                    table.ForeignKey(
                        name: "FK_ServiceChannelTranslationOrder_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelTranslationOrder_TranslationOrder_TranslationOrderId",
                        column: x => x.TranslationOrderId,
                        principalSchema: "public",
                        principalTable: "TranslationOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTranslationOrder",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    TranslationOrderId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTranslationOrder", x => new { x.ServiceVersionedId, x.TranslationOrderId });
                    table.ForeignKey(
                        name: "FK_ServiceTranslationOrder_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTranslationOrder_TranslationOrder_TranslationOrderId",
                        column: x => x.TranslationOrderId,
                        principalSchema: "public",
                        principalTable: "TranslationOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationOrderState",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OperationDescription = table.Column<string>(maxLength: 2500, nullable: true),
                    SendAt = table.Column<DateTime>(nullable: false),
                    TranslationOrderId = table.Column<Guid>(nullable: false),
                    TranslationStateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationOrderState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationOrderState_TranslationOrder_TranslationOrderId",
                        column: x => x.TranslationOrderId,
                        principalSchema: "public",
                        principalTable: "TranslationOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationOrderState_TranslationStateType_TranslationStateId",
                        column: x => x.TranslationStateId,
                        principalSchema: "public",
                        principalTable: "TranslationStateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionVersionedId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "StatutoryServiceGeneralDescriptionVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_GenDesTraOrd_TranslationOrderId",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                column: "TranslationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaTraOrd_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "ServiceChannelVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaTraOrd_TranslationOrderId",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                column: "TranslationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SerTraOrd_ServiceVersionedId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_SerTraOrd_TranslationOrderId",
                schema: "public",
                table: "ServiceTranslationOrder",
                column: "TranslationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TraCom_Id",
                schema: "public",
                table: "TranslationCompany",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrd_Id",
                schema: "public",
                table: "TranslationOrder",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrd_SourceLanguageId",
                schema: "public",
                table: "TranslationOrder",
                column: "SourceLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrd_TargetLanguageId",
                schema: "public",
                table: "TranslationOrder",
                column: "TargetLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrd_TranslationCompanyId",
                schema: "public",
                table: "TranslationOrder",
                column: "TranslationCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrdSta_Id",
                schema: "public",
                table: "TranslationOrderState",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrdSta_TranslationOrderId",
                schema: "public",
                table: "TranslationOrderState",
                column: "TranslationOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TraOrdSta_TranslationStateId",
                schema: "public",
                table: "TranslationOrderState",
                column: "TranslationStateId");

            migrationBuilder.CreateIndex(
                name: "IX_TraStaTyp_Id",
                schema: "public",
                table: "TranslationStateType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TraStaTypNam_LocalizationId",
                schema: "public",
                table: "TranslationStateTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TraStaTypNam_TypeId",
                schema: "public",
                table: "TranslationStateTypeName",
                column: "TypeId");

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "3TranslationCompany.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralDescriptionTranslationOrder",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceChannelTranslationOrder",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceTranslationOrder",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TranslationOrderState",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TranslationStateTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TranslationOrder",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TranslationStateType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TranslationCompany",
                schema: "public");
        }
    }
}
