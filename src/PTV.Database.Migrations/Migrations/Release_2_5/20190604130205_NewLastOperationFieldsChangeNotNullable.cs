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
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_5
{
    public partial class NewLastOperationFieldsChangeNotNullable : Migration
    {
        private readonly MigrateHelper migrateHelper;
        
        public NewLastOperationFieldsChangeNotNullable()
        {
            migrateHelper = new MigrateHelper();
        }
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrateHelper.AddSqlScript(
                migrationBuilder,
                Path.Combine("SqlMigrations", "PTV_2_5", "4FillNullLastOperationFields.sql")
            );
          
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPageTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPageTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPageType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPageType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebpageChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebpageChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WcagLevelTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WcagLevelTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WcagLevelType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WcagLevelType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Versioning",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Versioning",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRightsGroupName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRightsGroupName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRightsGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRightsGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRight",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRight",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationStateTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationStateTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationStateType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationStateType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationOrderState",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationOrderState",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationOrder",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationCompany",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationCompany",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingTranslationOrder",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingTranslationOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingServiceCollectionService",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingServiceCollectionService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingEntityVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingEntityVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TasksFilter",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TasksFilter",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TasksConfiguration",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TasksConfiguration",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TargetGroupName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TargetGroupName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TargetGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TargetGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceRequirement",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceRequirement",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLanguage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLanguage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceWebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceRequirement",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceRequirement",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducerOrganization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducerOrganization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducer",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceOntologyTerm",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceOntologyTerm",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLifeEvent",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLifeEvent",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLaw",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLaw",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLanguage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLanguage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceKeyword",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceKeyword",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceIndustrialClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceIndustrialClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHourType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHourType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHours",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHours",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceFundingType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceFundingType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClassName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClassName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClassDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClassDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChargeType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChargeType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelServiceClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelServiceClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelKeyword",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelKeyword",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelConnectionType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelConnectionType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAttachment",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAttachment",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelArea",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelArea",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAddress",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAddress",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceAreaMunicipality",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceAreaMunicipality",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceArea",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceArea",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "SahaOrganizationInformation",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "SahaOrganizationInformation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PublishingStatusType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PublishingStatusType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ProvisionType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ProvisionType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostOfficeBoxName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostOfficeBoxName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostalCodeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostalCodeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostalCode",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostalCode",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneNumberType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneNumberType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneExtraType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneExtraType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Phone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Phone",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationService",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationService",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationPhone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationPhone",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEmail",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEmail",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationDisplayNameType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationDisplayNameType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationArea",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationArea",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationAddress",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationAddress",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermParent",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermParent",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermExactMatch",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermExactMatch",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTerm",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTerm",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NotificationServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NotificationServiceServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NameTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NameTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NameType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NameType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "MunicipalityName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "MunicipalityName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Municipality",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Municipality",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Localization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Localization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LifeEventName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LifeEventName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LifeEvent",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LifeEvent",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LawWebPage",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LawWebPage",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LawName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LawName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Law",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Law",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LanguageName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LanguageName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Language",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Language",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Keyword",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Keyword",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "IndustrialClassName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "IndustrialClassName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "IndustrialClass",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "IndustrialClass",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormState",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormState",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Form",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Form",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraSubType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraSubType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExternalSource",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExternalSource",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExceptionHoursStatusType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExceptionHoursStatusType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExactMatch",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExactMatch",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Email",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Email",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ElectronicChannel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ElectronicChannel",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DialCode",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DialCode",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DescriptionType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DescriptionType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CountryName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CountryName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Country",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Country",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CoordinateType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CoordinateType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressPoint",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressPoint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Business",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Business",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "BugReport",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "BugReport",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AuthorizationEntryPoint",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AuthorizationEntryPoint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AttachmentType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AttachmentType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaMunicipality",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaMunicipality",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaInformationType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaInformationType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Area",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Area",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentDataType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentDataType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentData",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentData",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressReceiver",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressReceiver",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressOther",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressOther",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressForeignTextName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressForeignTextName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressForeign",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressForeign",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressExtraType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressExtraType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCoordinate",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCoordinate",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCharacterName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCharacterName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCharacter",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCharacter",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressAdditionalInformation",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressAdditionalInformation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Address",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Address",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessRightType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessRightType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessRightName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessRightName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassification",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassification",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPageTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPageTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPageType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPageType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebpageChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebpageChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WcagLevelTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WcagLevelTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "WcagLevelType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "WcagLevelType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Versioning",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Versioning",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserOrganization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserOrganization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRightsGroupName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRightsGroupName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRightsGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRightsGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "UserAccessRight",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "UserAccessRight",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationStateTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationStateTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationStateType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationStateType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationOrderState",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationOrderState",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationOrder",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationOrder",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TranslationCompany",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TranslationCompany",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingTranslationOrder",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingTranslationOrder",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingServiceCollectionService",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingServiceCollectionService",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TrackingEntityVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TrackingEntityVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TasksFilter",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TasksFilter",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TasksConfiguration",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TasksConfiguration",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TargetGroupName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TargetGroupName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "TargetGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "TargetGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceRequirement",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceRequirement",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceLanguage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceLanguage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "StatutoryServiceDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "StatutoryServiceDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceRequirement",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceRequirement",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducerOrganization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducerOrganization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceProducer",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceProducer",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceOntologyTerm",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceOntologyTerm",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLifeEvent",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLifeEvent",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLaw",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLaw",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceLanguage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceLanguage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceKeyword",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceKeyword",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceIndustrialClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceIndustrialClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHourType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHourType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceHours",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceHours",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceFundingType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceFundingType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceCollectionDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceCollectionDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClassName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClassName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClassDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClassDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChargeType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChargeType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelServiceClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelServiceClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelKeyword",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelKeyword",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelConnectionType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelConnectionType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAttachment",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAttachment",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelArea",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelArea",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAddress",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAddress",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceAreaMunicipality",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceAreaMunicipality",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ServiceArea",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ServiceArea",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "SahaOrganizationInformation",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "SahaOrganizationInformation",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PublishingStatusType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PublishingStatusType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ProvisionType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ProvisionType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PrintableFormChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PrintableFormChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostOfficeBoxName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostOfficeBoxName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostalCodeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostalCodeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PostalCode",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PostalCode",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneNumberType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneNumberType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "PhoneExtraType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "PhoneExtraType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Phone",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Phone",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationService",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationService",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationPhone",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationPhone",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEmail",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEmail",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationDisplayNameType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationDisplayNameType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationArea",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationArea",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OrganizationAddress",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OrganizationAddress",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermParent",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermParent",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermExactMatch",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermExactMatch",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTermDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTermDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "OntologyTerm",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "OntologyTerm",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NotificationServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NotificationServiceServiceChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NameTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NameTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "NameType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "NameType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "MunicipalityName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "MunicipalityName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Municipality",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Municipality",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Localization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Localization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LifeEventName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LifeEventName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LifeEvent",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LifeEvent",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LawWebPage",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LawWebPage",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LawName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LawName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Law",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Law",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "LanguageName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "LanguageName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Language",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Language",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Keyword",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Keyword",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "IndustrialClassName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "IndustrialClassName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "IndustrialClass",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "IndustrialClass",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "FormState",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "FormState",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Form",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Form",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExtraSubType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExtraSubType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExternalSource",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExternalSource",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExceptionHoursStatusType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExceptionHoursStatusType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ExactMatch",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ExactMatch",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Email",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Email",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ElectronicChannel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ElectronicChannel",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DialCode",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DialCode",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DescriptionType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DescriptionType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CountryName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CountryName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Country",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Country",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "CoordinateType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "CoordinateType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "ClsAddressPoint",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "ClsAddressPoint",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Business",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Business",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "BugReport",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "BugReport",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AuthorizationEntryPoint",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AuthorizationEntryPoint",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AttachmentType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AttachmentType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaMunicipality",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaMunicipality",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AreaInformationType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AreaInformationType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Area",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Area",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentDataType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentDataType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AppEnvironmentData",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AppEnvironmentData",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressReceiver",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressReceiver",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressOther",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressOther",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressForeignTextName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressForeignTextName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressForeign",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressForeign",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressExtraType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressExtraType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCoordinate",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCoordinate",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCharacterName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCharacterName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressCharacter",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressCharacter",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AddressAdditionalInformation",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AddressAdditionalInformation",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "Address",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "Address",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessRightType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessRightType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessRightName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessRightName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastOperationTimeStamp",
                schema: "public",
                table: "AccessibilityClassification",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<Guid>(
                name: "LastOperationIdentifier",
                schema: "public",
                table: "AccessibilityClassification",
                nullable: true,
                oldClrType: typeof(Guid));
       }
    }
}
