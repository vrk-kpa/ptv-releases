/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_6_1
{
    public partial class LinkValidator3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} Link validator part 3");
            Console.WriteLine($"{DateTime.UtcNow} Dropping foreign keys...");
            
            migrationBuilder.DropForeignKey(
                name: "FK_ElectronicChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_LawWebPage_Language_LocalizationId",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationWebPage_Language_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_WebPage_Language_LocalizationId",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_WebpageChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl");
            
            Console.WriteLine($"{DateTime.UtcNow} Dropping indexes and primary keys...");

            migrationBuilder.DropIndex(
                name: "IX_WebPag_LocalizationId",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceWebPage",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceServiceChannelWebPage",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelWebPage",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationWebPage",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LawWebPage",
                schema: "public",
                table: "LawWebPage");
            
            Console.WriteLine($"{DateTime.UtcNow} Dropping columns...");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "WebPage");
            
            Console.WriteLine($"{DateTime.UtcNow} Ading and altering columns...");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceWebPage",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "LawWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "LawWebPage",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
            
            Console.WriteLine($"{DateTime.UtcNow} Adding primary keys...");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceWebPage",
                schema: "public",
                table: "ServiceWebPage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceServiceChannelWebPage",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelWebPage",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationWebPage",
                schema: "public",
                table: "OrganizationWebPage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LawWebPage",
                schema: "public",
                table: "LawWebPage",
                column: "Id");
            
            Console.WriteLine($"{DateTime.UtcNow} Creating indexes...");

            migrationBuilder.CreateIndex(
                name: "IX_SerWebPag_Id",
                schema: "public",
                table: "ServiceWebPage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_Id",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaWebPag_Id",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgWebPag_Id",
                schema: "public",
                table: "OrganizationWebPage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LawWebPag_Id",
                schema: "public",
                table: "LawWebPage",
                column: "Id");
            
            Console.WriteLine($"{DateTime.UtcNow} Adding foreign keys...");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectronicChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LawWebPage_Language_LocalizationId",
                schema: "public",
                table: "LawWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationWebPage_Language_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrintableFormChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebpageChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElectronicChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_LawWebPage_Language_LocalizationId",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationWebPage_Language_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropForeignKey(
                name: "FK_WebpageChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceWebPage",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerWebPag_Id",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceServiceChannelWebPage",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaWebPag_Id",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelWebPage",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerChaWebPag_Id",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationWebPage",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropIndex(
                name: "IX_OrgWebPag_Id",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LawWebPage",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropIndex(
                name: "IX_LawWebPag_Id",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "WebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "WebPage",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "WebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "WebPage",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "LawWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceWebPage",
                schema: "public",
                table: "ServiceWebPage",
                columns: new[] { "ServiceVersionedId", "WebPageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceServiceChannelWebPage",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                columns: new[] { "ServiceId", "ServiceChannelId", "WebPageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelWebPage",
                schema: "public",
                table: "ServiceChannelWebPage",
                columns: new[] { "ServiceChannelVersionedId", "WebPageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationWebPage",
                schema: "public",
                table: "OrganizationWebPage",
                columns: new[] { "OrganizationVersionedId", "WebPageId", "TypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LawWebPage",
                schema: "public",
                table: "LawWebPage",
                columns: new[] { "LawId", "WebPageId" });

            migrationBuilder.CreateIndex(
                name: "IX_WebPag_LocalizationId",
                schema: "public",
                table: "WebPage",
                column: "LocalizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectronicChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LawWebPage_Language_LocalizationId",
                schema: "public",
                table: "LawWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationWebPage_Language_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrintableFormChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceServiceChannelWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceWebPage_Language_LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WebPage_Language_LocalizationId",
                schema: "public",
                table: "WebPage",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebpageChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
