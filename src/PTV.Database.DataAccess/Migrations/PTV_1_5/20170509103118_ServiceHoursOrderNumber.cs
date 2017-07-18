using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;
using System.IO;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class ServiceHoursOrderNumber : IPartialMigration
    {
        private DataUtils dataUtils;

        public ServiceHoursOrderNumber()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: true);

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5", "4ServiceHoursOrder.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceChannelServiceHours");
        }
    }
}
