using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    public partial class Is_open_247_to_service_hours : IPartialMigration
    {
        private readonly DataUtils dataUtils;

        public Is_open_247_to_service_hours()
        {
            dataUtils = new DataUtils();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNonStop",
                schema: "public",
                table: "ServiceHours",
                nullable: false,
                defaultValue: false);

           dataUtils.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_1_8", "4IsOpenNonStop.sql")
           );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNonStop",
                schema: "public",
                table: "ServiceHours");
        }
    }
}
