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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Migrations.Migrations.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.Migrations.Migrations.Release_1_9
{
    public partial class ServiceChannelAddress : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;

        public ServiceChannelAddress()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannel_Address_DeliveryAddressId",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropIndex(
                name: "IX_PriForCha_DeliveryAddressId",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.CreateTable(
                name: "ServiceChannelAddress",
                schema: "public",
                columns: table => new
                {
                    ServiceChannelVersionedId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChannelAddress", x => new {x.ServiceChannelVersionedId, x.AddressId});
                    table.ForeignKey(
                        name: "FK_ServiceChannelAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelAddress_AddressCharacter_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "public",
                        principalTable: "AddressCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceChannelAddress_ServiceChannelVersioned_ServiceChannelVersionedId",
                        column: x => x.ServiceChannelVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceChannelVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.RenameTable(
                name: "PrintableFormChannelReceiver",
                newName: "AddressReceiver",
                schema: "public");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                schema: "public",
                table: "AddressReceiver",
                nullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannelReceiver_PrintableFormChannel_PrintableF",
                schema: "public",
                table: "AddressReceiver");

            migrationBuilder.DropForeignKey(
                name: "FK_PrintableFormChannelReceiver_Language_LocalizationId",
                schema: "public",
                table: "AddressReceiver");
//
            migrationBuilder.DropPrimaryKey(
                name: "PK_PrintableFormChannelReceiver",
                schema: "public",
                table: "AddressReceiver");

            migrationBuilder.Sql($@"INSERT INTO ""AddressCharacter""(""Id"", ""Code"", ""Created"", ""Modified"") VALUES ('{AddressCharacterEnum.Delivery.ToString().GetGuid()}','{AddressCharacterEnum.Delivery.ToString()}','Now()','Now()');");
            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_9", "2ServiceChannelAddress.sql"));

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                schema: "public",
                table: "AddressReceiver",
                nullable: false
            );

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAdd_AddressId",
                schema: "public",
                table: "ServiceChannelAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAdd_CharacterId",
                schema: "public",
                table: "ServiceChannelAddress",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaAdd_ServiceChannelVersionedId",
                schema: "public",
                table: "ServiceChannelAddress",
                column: "ServiceChannelVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressReceiver_Language_LocalizationId",
                schema: "public",
                table: "AddressReceiver",
                column: "LocalizationId",
                principalSchema: "public",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressReceiver_Address_AddressId",
                schema: "public",
                table: "AddressReceiver",
                column: "AddressId",
                principalSchema: "public",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressReceiver",
                schema: "public",
                table: "AddressReceiver",
                columns: new[] {"LocalizationId", "AddressId"});

            migrationBuilder.CreateIndex(
                name: "IX_AddRec_LocalizationId",
                schema: "public",
                table: "AddressReceiver",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AddRec_AddressId",
                schema: "public",
                table: "AddressReceiver",
                column: "AddressId");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropColumn(
                name: "PrintableFormChannelId",
                schema: "public",
                table: "AddressReceiver");

            migrationBuilder.DropTable(
                name: "ServiceLocationChannelAddress",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceLocationChannel",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "FormReceiver",
                schema: "public",
                table: "AddressReceiver",
                newName: "Receiver");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
