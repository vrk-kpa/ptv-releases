using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_2_0
{
    public partial class AddressOther_table : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public AddressOther_table()
        {
            migrateHelper = new MigrateHelper();
        }
        
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_2_0", "4RemoveUnnecessaryCoordinates.sql")
            );
            
            migrationBuilder.CreateTable(
                name: "AddressOther",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PostalCodeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressOther", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_AddressOther_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressOther_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddOth_AddressId",
                schema: "public",
                table: "AddressOther",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddOth_PostalCodeId",
                schema: "public",
                table: "AddressOther",
                column: "PostalCodeId");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressOther",
                schema: "public");
        }
    }
}
