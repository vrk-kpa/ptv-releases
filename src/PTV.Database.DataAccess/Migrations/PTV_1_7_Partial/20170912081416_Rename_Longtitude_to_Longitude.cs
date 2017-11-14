using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class Rename_Longtitude_to_Longitude : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longtitude",
                schema: "public",
                table: "Coordinate",
                newName: "Longitude");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                schema: "public",
                table: "Coordinate",
                newName: "Longtitude");
        }
    }
}
