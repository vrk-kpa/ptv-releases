using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_4
{
    public partial class AddServiceVersionedVoucherType : Migration
    {
        private readonly MigrateHelper migrateHelper = new MigrateHelper();
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VoucherType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherTypeName",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_VoucherTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherTypeName_VoucherType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "VoucherType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "VoucherTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VouTyp_Id",
                schema: "public",
                table: "VoucherType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VouTypNam_LocalizationId",
                schema: "public",
                table: "VoucherTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_VouTypNam_TypeId",
                schema: "public",
                table: "VoucherTypeName",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_VoucherType_VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "VoucherTypeId",
                principalSchema: "public",
                principalTable: "VoucherType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            
            Console.WriteLine($"{DateTime.UtcNow} - Insert VoucherType data");
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_3_4", "1InsertVoucherTypes.sql"));
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceVersioned_VoucherType_VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropTable(
                name: "VoucherTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VoucherType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_SerVer_VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "VoucherTypeId",
                schema: "public",
                table: "ServiceVersioned");
        }
    }
}
