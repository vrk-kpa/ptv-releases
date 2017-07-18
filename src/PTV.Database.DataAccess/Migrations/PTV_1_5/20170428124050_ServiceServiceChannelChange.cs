using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ServiceServiceChannelChange : IPartialMigration
    {
        private DataUtils dataUtils;
        public ServiceServiceChannelChange()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5", "1ServiceServiceChannelChanges.sql"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServiceChannel_ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannel_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
