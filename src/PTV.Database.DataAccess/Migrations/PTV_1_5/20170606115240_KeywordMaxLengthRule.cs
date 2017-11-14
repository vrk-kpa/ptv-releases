using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;
using System.IO;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class KeywordMaxLengthRule : IPartialMigration
    {
        private DataUtils dataUtils;

        public KeywordMaxLengthRule()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_5", "5KeywordMaxLengthRule.sql"));
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
