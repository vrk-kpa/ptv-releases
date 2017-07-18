using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class OrganizationDisplayNameType : IPartialMigration
    {
        private DataUtils dataUtils;

        public OrganizationDisplayNameType()
        {
            dataUtils = new DataUtils();

        }

        public void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateTable(
                name: "OrganizationDisplayNameType",
                schema: "public",
                columns: table => new
                {
                    OrganizationVersionedId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DisplayNameTypeId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDisplayNameType", x => new { x.OrganizationVersionedId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_NameType_DisplayNameTypeId",
                        column: x => x.DisplayNameTypeId,
                        principalSchema: "public",
                        principalTable: "NameType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationDisplayNameType_OrganizationVersioned_OrganizationVersionedId",
                        column: x => x.OrganizationVersionedId,
                        principalSchema: "public",
                        principalTable: "OrganizationVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "DisplayNameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_LocalizationId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgDisNamTyp_OrganizationVersionedId",
                schema: "public",
                table: "OrganizationDisplayNameType",
                column: "OrganizationVersionedId");

			dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "5OrganizationDisplayNameType.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_NameType_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropIndex(
                name: "IX_Org_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropColumn(
                name: "DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationDisplayNameType",
                schema: "public");

            migrationBuilder.AddColumn<Guid>(
                name: "DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrgVer_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "DisplayNameTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationVersioned_NameType_DisplayNameTypeId",
                schema: "public",
                table: "OrganizationVersioned",
                column: "DisplayNameTypeId",
                principalSchema: "public",
                principalTable: "NameType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
