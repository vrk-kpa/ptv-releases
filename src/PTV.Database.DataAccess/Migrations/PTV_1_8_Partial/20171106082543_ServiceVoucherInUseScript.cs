using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Utils;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    /// <summary>
    /// Manualy created migration to rerun script which was missing in 1.75 release
    /// </summary>
    public partial class ServiceVoucherInUseScript : IPartialMigration
    {
        private DataUtils dataUtils;

        public ServiceVoucherInUseScript()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_8", "2ServiceVoucherInUse.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
