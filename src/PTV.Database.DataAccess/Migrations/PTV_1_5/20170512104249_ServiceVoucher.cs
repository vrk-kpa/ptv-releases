using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ServiceVoucher : IPartialMigration
    {
        private DataUtils dataUtils;
        public ServiceVoucher()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceWebPage",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    WebPageId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceWebPage", x => new { x.ServiceVersionedId, x.WebPageId });
                    table.ForeignKey(
                        name: "FK_ServiceWebPage_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceWebPage_WebPage_WebPageId",
                        column: x => x.WebPageId,
                        principalSchema: "public",
                        principalTable: "WebPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5", "3ServiceVoucher.sql"));

           migrationBuilder.CreateIndex(
                name: "IX_ServiceWebPage_ServiceVersionedId",
                schema: "public",
                table: "ServiceWebPage",
                column: "ServiceVersionedId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceWebPage_WebPageId",
                schema: "public",
                table: "ServiceWebPage",
                column: "WebPageId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
