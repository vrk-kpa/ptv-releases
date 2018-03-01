using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations.PTV_1_8_Partial
{
    public partial class RemoveSoftHyphens : IPartialMigration
    {
        private readonly DataUtils dataUtils;
        public RemoveSoftHyphens()
        {
            dataUtils = new DataUtils();
        }
        public void Up(MigrationBuilder migrationBuilder)
        {

            dataUtils.AddSqlScript(
               migrationBuilder,
               Path.Combine("SqlMigrations", "PTV_1_8", "6RemoveSoftHyphens.sql")
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
