using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ServiceFundingType : IPartialMigration
    {
        private DataUtils dataUtils;
        public ServiceFundingType()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5", "2ServiceFundingType.sql"));

            migrationBuilder.CreateTable(
                name: "ServiceFundingType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFundingType", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValueSql: @"GetOrCreateDefaultServiceFundingTypeId('PubliclyFunded')"
            );

            migrationBuilder.CreateTable(
                name: "ServiceFundingTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFundingTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFundingTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceFundingTypeName_ServiceFundingType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceFundingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "FundingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerFunTyp_Id",
                schema: "public",
                table: "ServiceFundingType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerFunTypNam_Id",
                schema: "public",
                table: "ServiceFundingTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerFunTypNam_LocalizationId",
                schema: "public",
                table: "ServiceFundingTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerFunTypNam_TypeId",
                schema: "public",
                table: "ServiceFundingTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceFundingType_FundingTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "FundingTypeId",
                principalSchema: "public",
                principalTable: "ServiceFundingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
