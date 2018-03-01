using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class ConnectionDetailsKeys : IPartialMigration
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelAddress_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelAddress_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelEmail_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelEmail_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelExtraType_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelPhone_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelPhone_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelWebPage_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaWebPag_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaWebPag_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaSerHou_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaSerHou_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaPho_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaPho_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaExtTyp_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaEma_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaEma_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaAdd_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaAdd_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceId",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceServiceChannel",
                table: "ServiceServiceChannel",
                columns: new[] { "ServiceId", "ServiceChannelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelAddress_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelEmail_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelPhone_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelWebPage_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                columns: new[] { "ServiceId", "ServiceChannelId" },
                principalSchema: "public",
                principalTable: "ServiceServiceChannel",
                principalColumns: new[] { "ServiceId", "ServiceChannelId" },
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelAddress_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelEmail_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelPhone_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelWebPage_ServiceServiceChannel_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaWebPag_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaSerHou_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaPho_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaExtTyp_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaEma_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaAdd_ServiceId_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaSerHou_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaPho_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaExtTyp_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaEma_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaAdd_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelAddress_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelAddress_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelEmail_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelEmail_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelExtraType_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelExtraType_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelPhone_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelPhone_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelServiceHours_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelWebPage_ServiceChannel_ServiceChannelId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceChannelId",
                principalSchema: "public",
                principalTable: "ServiceChannel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Service_ServiceId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "ServiceId",
                principalSchema: "public",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
