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
    public partial class LinkValidator1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} Link validator part 1");
            
            migrationBuilder.AddColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExceptionComment",
                schema: "public",
                table: "WebPage",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBroken",
                schema: "public",
                table: "WebPage",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsException",
                schema: "public",
                table: "WebPage",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "ServiceWebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "ServiceChannelWebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "OrganizationWebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "public",
                table: "LawWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalizationId",
                schema: "public",
                table: "LawWebPage",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "public",
                table: "LawWebPage",
                maxLength: 110,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "LawWebPage",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebChaUrl_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SerWebPag_LocalizationId",
                schema: "public",
                table: "ServiceWebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerSerChaWebPag_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaWebPag_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaUrl_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgWebPag_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LawWebPag_LocalizationId",
                schema: "public",
                table: "LawWebPage",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EleChaUrl_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl",
                column: "WebPageId");

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
                name: "FK_WebpageChannelUrl_WebPage_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl",
                column: "WebPageId",
                principalSchema: "public",
                principalTable: "WebPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropIndex(
                name: "IX_WebChaUrl_WebPageId",
                schema: "public",
                table: "WebpageChannelUrl");

            migrationBuilder.DropIndex(
                name: "IX_SerWebPag_LocalizationId",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerSerChaWebPag_LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_SerChaWebPag_LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropIndex(
                name: "IX_PriForChaUrl_WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropIndex(
                name: "IX_OrgWebPag_LocalizationId",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropIndex(
                name: "IX_LawWebPag_LocalizationId",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropIndex(
                name: "IX_EleChaUrl_WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl");

            migrationBuilder.DropColumn(
                name: "WebPageId",
                schema: "public",
                table: "WebpageChannelUrl");

            migrationBuilder.DropColumn(
                name: "ExceptionComment",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "IsBroken",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "IsException",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "WebPageId",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "LocalizationId",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "WebPageId",
                schema: "public",
                table: "ElectronicChannelUrl");
        }
    }
}
