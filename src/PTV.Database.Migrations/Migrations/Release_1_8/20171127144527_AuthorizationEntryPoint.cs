﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    public partial class AuthorizationEntryPoint : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizationEntryPoint",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationEntryPoint", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutEntPoi_Id",
                schema: "public",
                table: "AuthorizationEntryPoint",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationEntryPoint",
                schema: "public");
        }
    }
}
