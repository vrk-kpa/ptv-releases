/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;
using System.IO;

namespace PTV.Database.DataAccess.Migrations.PTV_1_7_Partial
{
    public partial class NameTypesKeys : IPartialMigration
    {
        private readonly DataUtils dataUtils;
        public NameTypesKeys()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_7", "1NameTypeKeys.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypeName_ServiceType_TypeId",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebPageTypeName",
                schema: "public",
                table: "WebPageTypeName");

            migrationBuilder.DropIndex(
                name: "IX_WebPagTypNam_Id",
                schema: "public",
                table: "WebPageTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetGroupName",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropIndex(
                name: "IX_TarGroNam_Id",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceTypeName",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropIndex(
                name: "IX_SerTypNam_Id",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChargeTypeName",
                schema: "public",
                table: "ServiceChargeTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ServCharTypeName_Id",
                schema: "public",
                table: "ServiceChargeTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelTypeName",
                schema: "public",
                table: "ServiceChannelTypeName");

            migrationBuilder.DropIndex(
                name: "IX_SerChaTypNam_Id",
                schema: "public",
                table: "ServiceChannelTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelConnectionTypeName",
                schema: "public",
                table: "ServiceChannelConnectionTypeName");

            migrationBuilder.DropIndex(
                name: "IX_SerChaConTypNam_Id",
                schema: "public",
                table: "ServiceChannelConnectionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceHourTypeName",
                schema: "public",
                table: "ServiceHourTypeName");

            migrationBuilder.DropIndex(
                name: "IX_SerHouTypNam_Id",
                schema: "public",
                table: "ServiceHourTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceFundingTypeName",
                schema: "public",
                table: "ServiceFundingTypeName");

            migrationBuilder.DropIndex(
                name: "IX_SerFunTypNam_Id",
                schema: "public",
                table: "ServiceFundingTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceClassName",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropIndex(
                name: "IX_SerClaNam_Id",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublishingStatusTypeName",
                schema: "public",
                table: "PublishingStatusTypeName");

            migrationBuilder.DropIndex(
                name: "IX_PubStaTypNam_Id",
                schema: "public",
                table: "PublishingStatusTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProvisionTypeName",
                schema: "public",
                table: "ProvisionTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ProTypNam_Id",
                schema: "public",
                table: "ProvisionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrintableFormChannelUrlTypeName",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName");

            migrationBuilder.DropIndex(
                name: "IX_PriForChaUrlTypNam_Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumberTypeName",
                schema: "public",
                table: "PhoneNumberTypeName");

            migrationBuilder.DropIndex(
                name: "IX_PhoNumTypNam_Id",
                schema: "public",
                table: "PhoneNumberTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationTypeName",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropIndex(
                name: "IX_OrgTypNam_Id",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OntologyTermName",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropIndex(
                name: "IX_OntTerNam_Id",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NameTypeName",
                schema: "public",
                table: "NameTypeName");

            migrationBuilder.DropIndex(
                name: "IX_NamTypNam_Id",
                schema: "public",
                table: "NameTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LifeEventName",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropIndex(
                name: "IX_LifEveNam_Id",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LanguageName",
                schema: "public",
                table: "LanguageName");

            migrationBuilder.DropIndex(
                name: "IX_LanNam_Id",
                schema: "public",
                table: "LanguageName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndustrialClassName",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropIndex(
                name: "IX_IndClaNam_Id",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormTypeName",
                schema: "public",
                table: "FormTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ForTypNam_Id",
                schema: "public",
                table: "FormTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraTypeName",
                schema: "public",
                table: "ExtraTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ExtTypNam_Id",
                schema: "public",
                table: "ExtraTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraSubTypeName",
                schema: "public",
                table: "ExtraSubTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ExtSubTypNam_Id",
                schema: "public",
                table: "ExtraSubTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExceptionHoursStatusTypeName",
                schema: "public",
                table: "ExceptionHoursStatusTypeName");

            migrationBuilder.DropIndex(
                name: "IX_ExcHouStaTypNam_Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DigitalAuthorizationName",
                schema: "public",
                table: "DigitalAuthorizationName");

            migrationBuilder.DropIndex(
                name: "IX_DigAutNam_Id",
                schema: "public",
                table: "DigitalAuthorizationName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionTypeName",
                schema: "public",
                table: "DescriptionTypeName");

            migrationBuilder.DropIndex(
                name: "IX_DesTypNam_Id",
                schema: "public",
                table: "DescriptionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryName",
                schema: "public",
                table: "CountryName");

            migrationBuilder.DropIndex(
                name: "IX_CouNam_Id",
                schema: "public",
                table: "CountryName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoordinateTypeName",
                schema: "public",
                table: "CoordinateTypeName");

            migrationBuilder.DropIndex(
                name: "IX_CooTypNam_Id",
                schema: "public",
                table: "CoordinateTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttachmentTypeName",
                schema: "public",
                table: "AttachmentTypeName");

            migrationBuilder.DropIndex(
                name: "IX_AttTypNam_Id",
                schema: "public",
                table: "AttachmentTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AreaTypeName",
                schema: "public",
                table: "AreaTypeName");

            migrationBuilder.DropIndex(
                name: "IX_AreTypNam_Id",
                schema: "public",
                table: "AreaTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AreaInformationTypeName",
                schema: "public",
                table: "AreaInformationTypeName");

            migrationBuilder.DropIndex(
                name: "IX_AreInfTypNam_Id",
                schema: "public",
                table: "AreaInformationTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppEnvironmentDataTypeName",
                schema: "public",
                table: "AppEnvironmentDataTypeName");

            migrationBuilder.DropIndex(
                name: "IX_AppEnvDatTypNam_Id",
                schema: "public",
                table: "AppEnvironmentDataTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressTypeName",
                schema: "public",
                table: "AddressTypeName");

            migrationBuilder.DropIndex(
                name: "IX_AddTypNam_Id",
                schema: "public",
                table: "AddressTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessRightName",
                schema: "public",
                table: "AccessRightName");

            migrationBuilder.DropIndex(
                name: "IX_AccRigNam_Id",
                schema: "public",
                table: "AccessRightName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "WebPageTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChargeTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceChannelConnectionTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceHourTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceFundingTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "PublishingStatusTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ProvisionTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "PhoneNumberTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "NameTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "LawName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "LanguageName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "FormTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ExtraTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ExtraSubTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "DigitalAuthorizationName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "DescriptionTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "CountryName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "CoordinateTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AttachmentTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AreaTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AreaName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AreaInformationTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AppEnvironmentDataTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AddressTypeName");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "AccessRightName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebPageTypeName",
                schema: "public",
                table: "WebPageTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetGroupName",
                schema: "public",
                table: "TargetGroupName",
                columns: new[] { "TargetGroupId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceTypeName",
                schema: "public",
                table: "ServiceTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChargeTypeName",
                schema: "public",
                table: "ServiceChargeTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelTypeName",
                schema: "public",
                table: "ServiceChannelTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelConnectionTypeName",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceHourTypeName",
                schema: "public",
                table: "ServiceHourTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceFundingTypeName",
                schema: "public",
                table: "ServiceFundingTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceClassName",
                schema: "public",
                table: "ServiceClassName",
                columns: new[] { "ServiceClassId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublishingStatusTypeName",
                schema: "public",
                table: "PublishingStatusTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProvisionTypeName",
                schema: "public",
                table: "ProvisionTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrintableFormChannelUrlTypeName",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumberTypeName",
                schema: "public",
                table: "PhoneNumberTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationTypeName",
                schema: "public",
                table: "OrganizationTypeName",
                columns: new[] { "OrganizationTypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OntologyTermName",
                schema: "public",
                table: "OntologyTermName",
                columns: new[] { "OntologyTermId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NameTypeName",
                schema: "public",
                table: "NameTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LifeEventName",
                schema: "public",
                table: "LifeEventName",
                columns: new[] { "LifeEventId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LanguageName",
                schema: "public",
                table: "LanguageName",
                columns: new[] { "LanguageId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndustrialClassName",
                schema: "public",
                table: "IndustrialClassName",
                columns: new[] { "IndustrialClassId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormTypeName",
                schema: "public",
                table: "FormTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraTypeName",
                schema: "public",
                table: "ExtraTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraSubTypeName",
                schema: "public",
                table: "ExtraSubTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExceptionHoursStatusTypeName",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DigitalAuthorizationName",
                schema: "public",
                table: "DigitalAuthorizationName",
                columns: new[] { "DigitalAuthorizationId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionTypeName",
                schema: "public",
                table: "DescriptionTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryName",
                schema: "public",
                table: "CountryName",
                columns: new[] { "CountryId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoordinateTypeName",
                schema: "public",
                table: "CoordinateTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttachmentTypeName",
                schema: "public",
                table: "AttachmentTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AreaTypeName",
                schema: "public",
                table: "AreaTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AreaInformationTypeName",
                schema: "public",
                table: "AreaInformationTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppEnvironmentDataTypeName",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressTypeName",
                schema: "public",
                table: "AddressTypeName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessRightName",
                schema: "public",
                table: "AccessRightName",
                columns: new[] { "TypeId", "LocalizationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "OrganizationTypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypeName_ServiceType_TypeId",
                schema: "public",
                table: "ServiceTypeName",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypeName_ServiceType_TypeId",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WebPageTypeName",
                schema: "public",
                table: "WebPageTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetGroupName",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceTypeName",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChargeTypeName",
                schema: "public",
                table: "ServiceChargeTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelTypeName",
                schema: "public",
                table: "ServiceChannelTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceChannelConnectionTypeName",
                schema: "public",
                table: "ServiceChannelConnectionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceHourTypeName",
                schema: "public",
                table: "ServiceHourTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceFundingTypeName",
                schema: "public",
                table: "ServiceFundingTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceClassName",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublishingStatusTypeName",
                schema: "public",
                table: "PublishingStatusTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProvisionTypeName",
                schema: "public",
                table: "ProvisionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrintableFormChannelUrlTypeName",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneNumberTypeName",
                schema: "public",
                table: "PhoneNumberTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationTypeName",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OntologyTermName",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NameTypeName",
                schema: "public",
                table: "NameTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LifeEventName",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LanguageName",
                schema: "public",
                table: "LanguageName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndustrialClassName",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormTypeName",
                schema: "public",
                table: "FormTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraTypeName",
                schema: "public",
                table: "ExtraTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraSubTypeName",
                schema: "public",
                table: "ExtraSubTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExceptionHoursStatusTypeName",
                schema: "public",
                table: "ExceptionHoursStatusTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DigitalAuthorizationName",
                schema: "public",
                table: "DigitalAuthorizationName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DescriptionTypeName",
                schema: "public",
                table: "DescriptionTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryName",
                schema: "public",
                table: "CountryName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoordinateTypeName",
                schema: "public",
                table: "CoordinateTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttachmentTypeName",
                schema: "public",
                table: "AttachmentTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AreaTypeName",
                schema: "public",
                table: "AreaTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AreaInformationTypeName",
                schema: "public",
                table: "AreaInformationTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppEnvironmentDataTypeName",
                schema: "public",
                table: "AppEnvironmentDataTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressTypeName",
                schema: "public",
                table: "AddressTypeName");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessRightName",
                schema: "public",
                table: "AccessRightName");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "WebPageTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "TargetGroupName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ServiceClassName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "OntologyTermName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "NameTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "LifeEventName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "LawName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "LanguageName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "IndustrialClassName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "FormTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ExtraTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "CountryName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AreaTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AreaName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AddressTypeName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "AccessRightName",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebPageTypeName",
                schema: "public",
                table: "WebPageTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetGroupName",
                schema: "public",
                table: "TargetGroupName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceTypeName",
                schema: "public",
                table: "ServiceTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChargeTypeName",
                schema: "public",
                table: "ServiceChargeTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelTypeName",
                schema: "public",
                table: "ServiceChannelTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceChannelConnectionTypeName",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceHourTypeName",
                schema: "public",
                table: "ServiceHourTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceFundingTypeName",
                schema: "public",
                table: "ServiceFundingTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceClassName",
                schema: "public",
                table: "ServiceClassName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublishingStatusTypeName",
                schema: "public",
                table: "PublishingStatusTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProvisionTypeName",
                schema: "public",
                table: "ProvisionTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrintableFormChannelUrlTypeName",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneNumberTypeName",
                schema: "public",
                table: "PhoneNumberTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationTypeName",
                schema: "public",
                table: "OrganizationTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OntologyTermName",
                schema: "public",
                table: "OntologyTermName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NameTypeName",
                schema: "public",
                table: "NameTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LifeEventName",
                schema: "public",
                table: "LifeEventName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LanguageName",
                schema: "public",
                table: "LanguageName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndustrialClassName",
                schema: "public",
                table: "IndustrialClassName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormTypeName",
                schema: "public",
                table: "FormTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraTypeName",
                schema: "public",
                table: "ExtraTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraSubTypeName",
                schema: "public",
                table: "ExtraSubTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExceptionHoursStatusTypeName",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DigitalAuthorizationName",
                schema: "public",
                table: "DigitalAuthorizationName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescriptionTypeName",
                schema: "public",
                table: "DescriptionTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryName",
                schema: "public",
                table: "CountryName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoordinateTypeName",
                schema: "public",
                table: "CoordinateTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttachmentTypeName",
                schema: "public",
                table: "AttachmentTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AreaTypeName",
                schema: "public",
                table: "AreaTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AreaInformationTypeName",
                schema: "public",
                table: "AreaInformationTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppEnvironmentDataTypeName",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressTypeName",
                schema: "public",
                table: "AddressTypeName",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessRightName",
                schema: "public",
                table: "AccessRightName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebPagTypNam_Id",
                schema: "public",
                table: "WebPageTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TarGroNam_Id",
                schema: "public",
                table: "TargetGroupName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerTypNam_Id",
                schema: "public",
                table: "ServiceTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServCharTypeName_Id",
                schema: "public",
                table: "ServiceChargeTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaTypNam_Id",
                schema: "public",
                table: "ServiceChannelTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerChaConTypNam_Id",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerHouTypNam_Id",
                schema: "public",
                table: "ServiceHourTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerFunTypNam_Id",
                schema: "public",
                table: "ServiceFundingTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerClaNam_Id",
                schema: "public",
                table: "ServiceClassName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PubStaTypNam_Id",
                schema: "public",
                table: "PublishingStatusTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProTypNam_Id",
                schema: "public",
                table: "ProvisionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PriForChaUrlTypNam_Id",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoNumTypNam_Id",
                schema: "public",
                table: "PhoneNumberTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgTypNam_Id",
                schema: "public",
                table: "OrganizationTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OntTerNam_Id",
                schema: "public",
                table: "OntologyTermName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NamTypNam_Id",
                schema: "public",
                table: "NameTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LifEveNam_Id",
                schema: "public",
                table: "LifeEventName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LanNam_Id",
                schema: "public",
                table: "LanguageName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IndClaNam_Id",
                schema: "public",
                table: "IndustrialClassName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ForTypNam_Id",
                schema: "public",
                table: "FormTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtTypNam_Id",
                schema: "public",
                table: "ExtraTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtSubTypNam_Id",
                schema: "public",
                table: "ExtraSubTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExcHouStaTypNam_Id",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DigAutNam_Id",
                schema: "public",
                table: "DigitalAuthorizationName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DesTypNam_Id",
                schema: "public",
                table: "DescriptionTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CouNam_Id",
                schema: "public",
                table: "CountryName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CooTypNam_Id",
                schema: "public",
                table: "CoordinateTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttTypNam_Id",
                schema: "public",
                table: "AttachmentTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AreTypNam_Id",
                schema: "public",
                table: "AreaTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AreInfTypNam_Id",
                schema: "public",
                table: "AreaInformationTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnvDatTypNam_Id",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AddTypNam_Id",
                schema: "public",
                table: "AddressTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccRigNam_Id",
                schema: "public",
                table: "AccessRightName",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationTypeName_OrganizationType_OrganizationTypeId",
                schema: "public",
                table: "OrganizationTypeName",
                column: "OrganizationTypeId",
                principalSchema: "public",
                principalTable: "OrganizationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypeName_ServiceType_TypeId",
                schema: "public",
                table: "ServiceTypeName",
                column: "TypeId",
                principalSchema: "public",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
