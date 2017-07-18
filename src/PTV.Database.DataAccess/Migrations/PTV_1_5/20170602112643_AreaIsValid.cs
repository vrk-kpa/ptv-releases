using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class AreaIsValid : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                schema: "public",
                table: "Area",
                nullable: false,
                defaultValue: true);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {}
    }
}
