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
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_2_6
{
    public partial class RecreateLastOperationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WebPageTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WebPageType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WebpageChannelUrl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WebpageChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WcagLevelTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "WcagLevelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Versioning",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "UserOrganization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRightsGroupName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRightsGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRight",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationStateTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationStateType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationOrderState",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationCompany",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingTranslationOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingServiceServiceChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingServiceCollectionService",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingEntityVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TasksFilter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TasksConfiguration",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TargetGroupName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "TargetGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceTargetGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceServiceClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceRequirement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceOntologyTerm",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLifeEvent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLaw",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLanguage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceIndustrialClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceWebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTranslationOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTargetGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelWebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelServiceHours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelPhone",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelExtraType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelEmail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelAddress",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceRequirement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducerOrganization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducerAdditionalInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceOntologyTerm",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLifeEvent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLaw",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLanguageAvailability",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLanguage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceKeyword",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceIndustrialClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHourTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHourType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHoursAdditionalInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceFundingTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceFundingType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceElectronicNotificationChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionService",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClassName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClassDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChargeTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChargeType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelWebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTranslationOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTargetGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelServiceHours",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelServiceClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelPhone",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelOntologyTerm",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelLanguageAvailability",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelLanguage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelKeyword",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelEmail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelDisplayNameType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelConnectionTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelConnectionType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAttachment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAreaMunicipality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAddress",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceAreaMunicipality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "SahaOrganizationInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PublishingStatusTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PublishingStatusType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ProvisionTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ProvisionType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrlType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelIdentifier",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PostOfficeBoxName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PostalCodeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PostalCode",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneNumberTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneNumberType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneExtraType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Phone",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationWebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationVersioned",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationService",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationPhone",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationLanguageAvailability",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEmail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEInvoicing",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationDisplayNameType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationAreaMunicipality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationArea",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationAddress",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermParent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermExactMatch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTerm",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "NotificationServiceServiceChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "NameTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "NameType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "MunicipalityName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Municipality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Localization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "LifeEventName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "LifeEvent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "LawWebPage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "LawName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Law",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "LanguageName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Language",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Keyword",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "IndustrialClassName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "IndustrialClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "FormTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "FormType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "FormState",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Form",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraSubTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraSubType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExternalSource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExceptionHoursStatusTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExceptionHoursStatusType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ExactMatch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Email",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ElectronicChannelUrl",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ElectronicChannel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DigitalAuthorizationName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DigitalAuthorization",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DialCode",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DescriptionTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DescriptionType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "DailyOpeningTime",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "CountryName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Country",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "CoordinateTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "CoordinateType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ClsStreetNumberCoordinate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreetNumber",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreetName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressPoint",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Business",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "BugReport",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AuthorizationEntryPoint",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AttachmentTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AttachmentType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Attachment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaMunicipality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaInformationTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AreaInformationType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Area",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentDataTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentDataType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressReceiver",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressPostOfficeBox",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressOther",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressForeignTextName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressForeign",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressExtraType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCoordinate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCharacterName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCharacter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AddressAdditionalInformation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "Address",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessRightType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessRightName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterSentence",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterGroupValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterEntranceName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterEntrance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegister",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassificationLevelType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassification",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WebPageTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WebPageType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WebpageChannelUrl");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WebpageChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WcagLevelTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "WcagLevelType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Versioning");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRightsGroupName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRightsGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "UserAccessRight");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationStateTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationStateType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationOrderState");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationOrder");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TranslationCompany");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingTranslationOrder");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingServiceServiceChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingServiceCollectionService");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingGeneralDescriptionVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingGeneralDescriptionServiceChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TrackingEntityVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TasksFilter");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TasksConfiguration");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TargetGroupName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "TargetGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceTargetGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceServiceClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceRequirement");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceOntologyTerm");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLifeEvent");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLaw");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceLanguage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceIndustrialClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceGeneralDescriptionVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "StatutoryServiceDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceWebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTranslationOrder");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceTargetGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelExtraTypeDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelExtraType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelDigitalAuthorization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannelAddress");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceServiceChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceRequirement");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducerOrganization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducerAdditionalInformation");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceProducer");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceOntologyTerm");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLifeEvent");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLaw");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceLanguage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceKeyword");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceIndustrialClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHourTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHourType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHoursAdditionalInformation");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceHours");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceFundingTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceFundingType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceElectronicNotificationChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceElectronicCommunicationChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionService");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceCollectionDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClassName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClassDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChargeTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChargeType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelWebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTranslationOrder");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelTargetGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelSocialHealthCenter");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelServiceHours");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelServiceClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelPhone");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelOntologyTerm");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelLanguage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelKeyword");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelEmail");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelDisplayNameType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelConnectionTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelConnectionType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAttachment");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAreaMunicipality");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelArea");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAddress");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceChannelAccessibilityClassification");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceAreaMunicipality");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ServiceArea");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "SahaOrganizationInformation");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PublishingStatusTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PublishingStatusType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ProvisionTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ProvisionType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrlTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrlType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelUrl");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannelIdentifier");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PrintableFormChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PostOfficeBoxName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PostalCodeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PostalCode");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneNumberTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneNumberType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "PhoneExtraType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Phone");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationWebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationVersioned");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationService");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationPhone");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEmail");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEInvoicingAdditionalInformation");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationEInvoicing");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationDisplayNameType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationAreaMunicipality");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationArea");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OrganizationAddress");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermParent");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermExactMatch");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTermDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "OntologyTerm");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "NotificationServiceServiceChannelFilter");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "NotificationServiceServiceChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "NameTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "NameType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "MunicipalityName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Localization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "LifeEventName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "LifeEvent");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "LawWebPage");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "LawName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Law");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "LanguageName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Language");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Keyword");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "IndustrialClassName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "IndustrialClass");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionTranslationOrder");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraTypeDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelExtraType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDigitalAuthorization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannelDescription");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionServiceChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "GeneralDescriptionLanguageAvailability");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "FormTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "FormType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "FormState");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Form");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraSubTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExtraSubType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExternalSource");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExceptionHoursStatusTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExceptionHoursStatusType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ExactMatch");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Email");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ElectronicChannelUrl");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ElectronicChannel");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DigitalAuthorizationName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DigitalAuthorization");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DialCode");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DescriptionTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DescriptionType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "DailyOpeningTime");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "CountryName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "CoordinateTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "CoordinateType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ClsStreetNumberCoordinate");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreetNumber");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreetName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressStreet");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "ClsAddressPoint");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Business");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "BugReport");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AuthorizationEntryPoint");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AttachmentTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AttachmentType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaMunicipality");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaInformationTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AreaInformationType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentDataTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentDataType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AppEnvironmentData");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressReceiver");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressPostOfficeBox");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressOther");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressForeignTextName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressForeign");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressExtraType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCoordinate");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCharacterName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressCharacter");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AddressAdditionalInformation");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessRightType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessRightName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterSentenceValue");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterSentence");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterGroupValue");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterGroup");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterEntranceName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegisterEntrance");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityRegister");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassificationLevelTypeName");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassificationLevelType");

            migrationBuilder.DropColumn(
                name: "LastOperationType",
                schema: "public",
                table: "AccessibilityClassification");
        }
    }
}
