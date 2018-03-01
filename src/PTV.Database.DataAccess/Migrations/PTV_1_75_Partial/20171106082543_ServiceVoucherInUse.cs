using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Utils;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_75_Partial
{
    public partial class ServiceVoucherInUse : IPartialMigration
    {
        private DataUtils dataUtils;

        public ServiceVoucherInUse()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WebPageInUse",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValue: false);

            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "2ServiceVoucherInUse.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebPageInUse",
                schema: "public",
                table: "ServiceVersioned");
        }
    }
}
