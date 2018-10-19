using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;

namespace PTV.Database.Migrations.Migrations.Release_1_7
{
    public partial class form_state : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormState",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    EntityId = table.Column<string>(nullable: true),
                    EntityType = table.Column<string>(nullable: true),
                    FormName = table.Column<string>(nullable: true),
                    LanguageId = table.Column<Guid>(nullable: false),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormState", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForSta_Id",
                schema: "public",
                table: "FormState",
                column: "Id");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormState",
                schema: "public");
        }
    }
}
