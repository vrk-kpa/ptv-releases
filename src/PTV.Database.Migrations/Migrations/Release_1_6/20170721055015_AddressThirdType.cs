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

namespace PTV.Database.Migrations.Migrations.Release_1_6
{
    public partial class AddressThirdType : IPartialMigration
    {
        private readonly MigrateHelper migrateHelper;
        public AddressThirdType()
        {
            migrateHelper = new MigrateHelper();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AddressType",
                newName: "AddressCharacter",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "AddressType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypeId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressForeign",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressForeign", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_AddressForeign_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });





            migrationBuilder.CreateTable(
                name: "AddressPostOfficeBox",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    MunicipalityId = table.Column<Guid>(nullable: true),
                    PostalCodeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressPostOfficeBox", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_AddressPostOfficeBox_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressPostOfficeBox_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AddressPostOfficeBox_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressStreet",
                schema: "public",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    MunicipalityId = table.Column<Guid>(nullable: true),
                    PostalCodeId = table.Column<Guid>(nullable: false),
                    StreetNumber = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressStreet", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_AddressStreet_Address_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "public",
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressStreet_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AddressStreet_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalSchema: "public",
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressForeignTextName",
                schema: "public",
                columns: table => new
                {
                    AddressForeignId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressForeignTextName", x => new { x.AddressForeignId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AddressForeignTextName_AddressForeign_AddressForeignId",
                        column: x => x.AddressForeignId,
                        principalSchema: "public",
                        principalTable: "AddressForeign",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressForeignTextName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            foreach (AddressTypeEnum addressType in Enum.GetValues(typeof(AddressTypeEnum)))
            {
                migrationBuilder.Sql($@"INSERT INTO ""AddressType""(""Id"", ""Code"", ""Created"", ""Modified"") VALUES ('{addressType.ToString().GetGuid()}','{addressType.ToString()}','2017-01-01 01:01:01','2017-01-01 01:01:01');");
            }

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                schema: "public",
                table: "Address",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrateHelper.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_6", "3AddressThirdType.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Municipality_MunicipalityId",
                schema: "public",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_PostalCode_PostalCodeId",
                schema: "public",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAddress_AddressType_TypeId",
                schema: "public",
                table: "OrganizationAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_PostOfficeBoxName_Address_AddressId",
                schema: "public",
                table: "PostOfficeBoxName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceLocationChannelAddress_AddressType_TypeId",
                schema: "public",
                table: "ServiceLocationChannelAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_StreetAddress_Address_AddressId",
                schema: "public",
                table: "StreetName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreetName",
                schema: "public",
                table: "StreetName");

            migrationBuilder.DropIndex(
                name: "IX_StrNam_Id",
                schema: "public",
                table: "StreetName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostOfficeBoxName",
                schema: "public",
                table: "PostOfficeBoxName");

            migrationBuilder.DropIndex(
                name: "IX_PosOffBoxNam_Id",
                schema: "public",
                table: "PostOfficeBoxName");

            migrationBuilder.DropIndex(
                name: "IX_Add_MunicipalityId",
                schema: "public",
                table: "Address");


            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "StreetName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "PostOfficeBoxName");

            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "PostalCodeId",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                schema: "public",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "Text",
                schema: "public",
                table: "StreetName",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "public",
                table: "StreetName",
                newName: "AddressStreetId");

            migrationBuilder.RenameIndex(
                name: "IX_StrNam_AddressId",
                schema: "public",
                table: "StreetName",
                newName: "IX_StrNam_AddressStreetId");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_SerLocChaAdd_TypeId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                newName: "IX_SerLocChaAdd_CharacterId");

            migrationBuilder.RenameColumn(
                name: "Text",
                schema: "public",
                table: "PostOfficeBoxName",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "public",
                table: "PostOfficeBoxName",
                newName: "AddressPostOfficeBoxId");

            migrationBuilder.RenameIndex(
                name: "IX_PosOffBoxNam_AddressId",
                schema: "public",
                table: "PostOfficeBoxName",
                newName: "IX_PosOffBoxNam_AddressPostOfficeBoxId");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgAdd_TypeId",
                schema: "public",
                table: "OrganizationAddress",
                newName: "IX_OrgAdd_CharacterId");

//            migrationBuilder.AlterColumn<Guid>(
//                name: "PostalCodeId",
//                schema: "public",
//                table: "Address",
//                nullable: true,
//                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                schema: "public",
                table: "Address",
                nullable: true);



            migrationBuilder.AddPrimaryKey(
                name: "PK_StreetName",
                schema: "public",
                table: "StreetName",
                columns: new[] { "AddressStreetId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostOfficeBoxName",
                schema: "public",
                table: "PostOfficeBoxName",
                columns: new[] { "AddressPostOfficeBoxId", "LocalizationId" });



            migrationBuilder.CreateTable(
                name: "AddressCharacterName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressCharacterName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressCharacterName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressCharacterName_AddressCharacter_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AddressCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.DropTable(name: "AddressTypeName", schema: "public");
            migrationBuilder.CreateTable(
                name: "AddressTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastOperationId = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressTypeName_AddressType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "AddressType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddTypNam_Id",
                schema: "public",
                table: "AddressTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Add_TypeId",
                schema: "public",
                table: "Address",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AddFor_AddressId",
                schema: "public",
                table: "AddressForeign",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddForTexNam_AddressForeignId",
                schema: "public",
                table: "AddressForeignTextName",
                column: "AddressForeignId");

            migrationBuilder.CreateIndex(
                name: "IX_AddForTexNam_LocalizationId",
                schema: "public",
                table: "AddressForeignTextName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AddCha_Id",
                schema: "public",
                table: "AddressCharacter",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddChaNam_Id",
                schema: "public",
                table: "AddressCharacterName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddChaNam_LocalizationId",
                schema: "public",
                table: "AddressCharacterName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AddChaNam_TypeId",
                schema: "public",
                table: "AddressCharacterName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AddPosOffBox_AddressId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddPosOffBox_MunicipalityId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AddPosOffBox_PostalCodeId",
                schema: "public",
                table: "AddressPostOfficeBox",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_AddressId",
                schema: "public",
                table: "AddressStreet",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_MunicipalityId",
                schema: "public",
                table: "AddressStreet",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_AddStr_PostalCodeId",
                schema: "public",
                table: "AddressStreet",
                column: "PostalCodeId");

//            migrationBuilder.AddForeignKey(
//                name: "FK_Address_PostalCode_PostalCodeId",
//                schema: "public",
//                table: "Address",
//                column: "PostalCodeId",
//                principalSchema: "public",
//                principalTable: "PostalCode",
//                principalColumn: "Id",
//                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AddressType_TypeId",
                schema: "public",
                table: "Address",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "AddressType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAddress_AddressCharacter_CharacterId",
                schema: "public",
                table: "OrganizationAddress",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "AddressCharacter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostOfficeBoxName_AddressPostOfficeBox_AddressPostOfficeBoxId",
                schema: "public",
                table: "PostOfficeBoxName",
                column: "AddressPostOfficeBoxId",
                principalSchema: "public",
                principalTable: "AddressPostOfficeBox",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceLocationChannelAddress_AddressCharacter_CharacterId",
                schema: "public",
                table: "ServiceLocationChannelAddress",
                column: "CharacterId",
                principalSchema: "public",
                principalTable: "AddressCharacter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreetName_AddressStreet_AddressStreetId",
                schema: "public",
                table: "StreetName",
                column: "AddressStreetId",
                principalSchema: "public",
                principalTable: "AddressStreet",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
