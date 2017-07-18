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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models.Base;
using System.Linq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models.Privileges;

namespace PTV.Database.DataAccess.ApplicationDbContext
{
    internal partial class PtvDbContext : DbContext
    {
        private IEntityNavigationsMap entityNavigationsMap;
        private ICacheBuilder cacheBuilder;

        public PtvDbContext(DbContextOptions<PtvDbContext> options, IEntityNavigationsMap entityNavigationsMap, ICacheBuilder cacheBuilder) : base(options)
        {
            this.entityNavigationsMap = entityNavigationsMap;
            this.cacheBuilder = cacheBuilder;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<LanguageName>(entity =>
            {
                entity.HasOne(e => e.Language).WithMany(p => p.Names).HasForeignKey(d => d.LanguageId);
                entity.HasOne(e => e.Localization).WithMany(p => p.Localizations).HasForeignKey(d => d.LocalizationId);
            });

            modelBuilder.Entity<OntologyTermParent>(entity =>
            {
                entity.HasKey(e => new { e.ParentId, e.ChildId });
                entity.HasOne(e => e.Parent).WithMany(p => p.Children).HasForeignKey(d => d.ParentId);
                entity.HasOne(e => e.Child).WithMany(p => p.Parents).HasForeignKey(d => d.ChildId);
            });

            modelBuilder.Entity<AreaName>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.LocalizationId });
            });

            modelBuilder.Entity<AreaMunicipality>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.MunicipalityId });
            });

            modelBuilder.Entity<OrganizationAddress>(entity =>
            {
                entity.HasKey(e => new {OrganizationId = e.OrganizationVersionedId, e.AddressId, e.TypeId});
            });

            modelBuilder.Entity<OrganizationDescription>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.TypeId, e.LocalizationId});
            });

            modelBuilder.Entity<OrganizationEmail>(entity =>
            {
                entity.HasKey(e => new { e.EmailId, OrganizationId = e.OrganizationVersionedId});
            });

            modelBuilder.Entity<OrganizationName>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.TypeId, e.LocalizationId});
            });

            modelBuilder.Entity<OrganizationDisplayNameType>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.LocalizationId });
            });

            modelBuilder.Entity<OrganizationPhone>(entity =>
            {
                entity.HasKey(e => new { e.PhoneId, OrganizationId = e.OrganizationVersionedId});
            });

            modelBuilder.Entity<ServiceWebPage>(entity =>
            {
                entity.HasKey(e => new { e.ServiceVersionedId, e.WebPageId });
            });

            modelBuilder.Entity<OrganizationWebPage>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.WebPageId, e.TypeId });
            });

            modelBuilder.Entity<OrganizationArea>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.AreaId });
            });

            modelBuilder.Entity<OrganizationAreaMunicipality>(entity =>
            {
                entity.HasKey(e => new { OrganizationId = e.OrganizationVersionedId, e.MunicipalityId });
            });

            modelBuilder.Entity<ServiceDescription>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceName>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceKeyword>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.KeywordId });
            });

            modelBuilder.Entity<ServiceLanguage>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<ServiceLifeEvent>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.LifeEventId });
            });

            modelBuilder.Entity<ServiceOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.OntologyTermId });
            });

            modelBuilder.Entity<ServiceServiceClass>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.ServiceClassId });
            });

            modelBuilder.Entity<ServiceIndustrialClass>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.IndustrialClassId });
            });

            modelBuilder.Entity<ServiceServiceChannel>(entity =>
            {
                entity.HasKey(e => new {ServiceId = e.ServiceId, e.ServiceChannelId});
            });

            modelBuilder.Entity<ServiceTargetGroup>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.TargetGroupId });
            });

            modelBuilder.Entity<ServiceLaw>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.LawId });
            });

            modelBuilder.Entity<ServiceArea>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.AreaId });
            });


            modelBuilder.Entity<ServiceAreaMunicipality>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.MunicipalityId });
            });

            modelBuilder.Entity<ServiceChannelDescription>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelName>(entity =>
            {
                entity.HasKey(e => new {ServiceChannelId = e.ServiceChannelVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelWebPage>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.WebPageId });
            });

            modelBuilder.Entity<ServiceChannelArea>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.AreaId });
            });

            modelBuilder.Entity<ServiceChannelAreaMunicipality>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.MunicipalityId });
            });

            modelBuilder.Entity<StatutoryServiceName>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<StatutoryServiceDescription>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<StatutoryServiceServiceClass>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.ServiceClassId });
            });

            modelBuilder.Entity<StatutoryServiceTargetGroup>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.TargetGroupId });
            });

            modelBuilder.Entity<StatutoryServiceIndustrialClass>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.IndustrialClassId });
            });

            modelBuilder.Entity<StatutoryServiceLaw>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.LawId });
            });

            modelBuilder.Entity<StatutoryServiceLanguage>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<StatutoryServiceOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.OntologyTermId });
            });

            modelBuilder.Entity<StatutoryServiceLifeEvent>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.LifeEventId });
            });

            modelBuilder.Entity<ElectronicChannelUrl>(entity =>
            {
                entity.HasKey(e => new { e.ElectronicChannelId, e.LocalizationId });
            });

            modelBuilder.Entity<WebpageChannelUrl>(entity =>
            {
                entity.HasKey(e => new { e.WebpageChannelId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelTargetGroup>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.TargetGroupId });
            });

            modelBuilder.Entity<ServiceLocationChannelAddress>(entity =>
            {
                entity.HasKey(e => new { e.ServiceLocationChannelId, e.AddressId, e.TypeId });
            });           

            modelBuilder.Entity<ServiceChannelOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.OntologyTermId });
            });

            modelBuilder.Entity<ServiceChannelKeyword>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.KeywordId });
            });

            modelBuilder.Entity<ServiceChannelServiceClass>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.ServiceClassId });
            });

            modelBuilder.Entity<ServiceChannelLanguage>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<ServiceHoursAdditionalInformation>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelServiceHoursId, e.LocalizationId });
            });

            modelBuilder.Entity<DailyOpeningTime>(entity =>
            {
                entity.HasKey(e => new { e.OpeningHourId, e.DayFrom, e.IsExtra });
            });

            modelBuilder.Entity<ServiceChannelAttachment>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.AttachmentId });
            });

            modelBuilder.Entity<AddressAdditionalInformation>(entity =>
            {
                entity.HasKey(e => new { e.AddressId, e.LocalizationId });
            });

            modelBuilder.Entity<OrganizationVersioned>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<ServiceChannelPhone>(entity =>
            {
                entity.HasKey(e => new { e.PhoneId, ServiceChannelId = e.ServiceChannelVersionedId });
            });

            modelBuilder.Entity<ServiceChannelEmail>(entity =>
            {
                entity.HasKey(e => new { e.EmailId, ServiceChannelId = e.ServiceChannelVersionedId });
            });

			modelBuilder.Entity<OrganizationVersioned>(entity =>
            {
                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => e.Oid);
            });

            modelBuilder.Entity<OrganizationService>(entity =>
            {
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.ServiceVersionedId);
            });

            modelBuilder.Entity<ServiceServiceChannelDescription>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.LocalizationId, e.ServiceChannelId, e.ServiceId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelDescriptions).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });

            });

            modelBuilder.Entity<MunicipalityName>(entity =>
            {
                entity.HasKey(e => new { e.MunicipalityId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceServiceChannelDigitalAuthorization>(entity =>
            {
                entity.HasKey(e => new { e.DigitalAuthorizationId, e.ServiceId, e.ServiceChannelId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelDigitalAuthorizations).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
            });

            modelBuilder.Entity<Versioning>(entity =>
            {
                entity.HasOne(e => e.PreviousVersion).WithMany(e => e.ChildrenVersions).HasForeignKey(e => e.PreviousVersionId);
            });

            modelBuilder.Entity<LawName>(entity =>
            {
                entity.HasKey(e => new { e.LawId, e.LocalizationId });
            });

            modelBuilder.Entity<LawWebPage>(entity =>
            {
                entity.HasKey(e => new { e.LawId, e.WebPageId });
            });

            modelBuilder.Entity<ServiceLanguageAvailability>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.ServiceVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<ServiceChannelLanguageAvailability>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<OrganizationLanguageAvailability>(entity =>
            {
                entity.HasKey(e => new {OrganizationId = e.OrganizationVersionedId, e.LanguageId});
            });

            modelBuilder.Entity<GeneralDescriptionLanguageAvailability>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<PostalCodeName>(entity =>
            {
                entity.HasKey(e => new { e.PostalCodeId, e.LocalizationId });
            });

            modelBuilder.Entity<OntologyTermExactMatch>(entity =>
            {
                entity.HasKey(e => new { e.OntologyTermId, e.ExactMatchId });
            });

            modelBuilder.Entity<OrganizationVersioned>(entity =>
            {
                entity.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId);
            });

            modelBuilder.Entity<PrintableFormChannelIdentifier>(entity =>
            {
                entity.HasKey(e => new { e.PrintableFormChannelId, e.LocalizationId });
            });

            modelBuilder.Entity<PrintableFormChannelReceiver>(entity =>
            {
                entity.HasKey(e => new { e.PrintableFormChannelId, e.LocalizationId });
            });

            modelBuilder.Entity<BugReport>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });


            modelBuilder.Entity<ServiceBlockedAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessBlockedId, e.EntityId });
            });

            modelBuilder.Entity<ChannelBlockedAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessBlockedId, e.EntityId });
            });

            modelBuilder.Entity<OrganizationBlockedAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessBlockedId, e.EntityId });
            });

            modelBuilder.Entity<GeneralDescriptionBlockedAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessBlockedId, e.EntityId });
            });

            modelBuilder.Entity<UserAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessRightId, e.UserId });
            });

            EntityFrameworkEntityTools.CreateIndexes(modelBuilder);
            ((EntityNavigationsMap)entityNavigationsMap).NavigationsMap = EntityFrameworkEntityTools.BuildEntityPropertiesMap(modelBuilder);
            cacheBuilder.TypeBaseEntities = EntityFrameworkEntityTools.GetEntitiesOfBaseType<TypeBase>(modelBuilder);
            EntityFrameworkEntityTools.BuildContextInfo(modelBuilder);
            EntityFrameworkEntityTools.DataContextInfo.EntitySetsMap = this.BuildSetMap();
        }


        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<AddressTypeName> AddressTypeNames { get; set; }
        public virtual DbSet<AppEnvironmentData> AppEnvironmentDatas { get; set; }
        public virtual DbSet<AppEnvironmentDataType> AppEnvironmentDataTypes { get; set; }
        public virtual DbSet<AppEnvironmentDataTypeName> AppEnvironmentDataTypeNames { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<AttachmentTypeName> AttachmentTypeNames { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Business> Businesses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryName> CountryNames { get; set; }
        public virtual DbSet<DescriptionType> DescriptionTypes { get; set; }
        public virtual DbSet<DescriptionTypeName> DescriptionTypeNames { get; set; }
        public virtual DbSet<ElectronicChannel> ElectronicChannels { get; set; }
        public virtual DbSet<ElectronicChannelUrl> ElectronicChannelUrls { get; set; }
        public virtual DbSet<ExceptionHoursStatusType> ExceptionHoursStatusTypes { get; set; }
        public virtual DbSet<ExceptionHoursStatusTypeName> ExceptionHoursStatusTypeNames { get; set; }
        public virtual DbSet<ExternalSource> ExternalSources { get; set; }
        public virtual DbSet<PrintableFormChannel> PrintableFormChannels { get; set; }
        public virtual DbSet<PrintableFormChannelUrl> PrintableFormChannelUrls { get; set; }
        public virtual DbSet<PrintableFormChannelIdentifier> PrintableFormChannelIdentifierss { get; set; }
        public virtual DbSet<PrintableFormChannelReceiver> PrintableFormChannelReceivers { get; set; }

        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormType> FormTypes { get; set; }
        public virtual DbSet<FormTypeName> FormTypeNames { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageName> LanguageNames { get; set; }
        public virtual DbSet<LifeEvent> LifeEvents { get; set; }
        public virtual DbSet<LifeEventName> LifeEventNames { get; set; }
        public virtual DbSet<Municipality> Municipalities { get; set; }
        public virtual DbSet<MunicipalityName> MunicipalityNames { get; set; }
        public virtual DbSet<NameType> NameTypes { get; set; }
        public virtual DbSet<NameTypeName> NameTypeNames { get; set; }
        public virtual DbSet<OntologyTerm> OntologyTerms { get; set; }
        public virtual DbSet<OntologyTermName> OntologyTermNames { get; set; }
        public virtual DbSet<OrganizationVersioned> OrganizationsVersioned { get; set; }
        public virtual DbSet<OrganizationAddress> OrganizationAddresses { get; set; }
        public virtual DbSet<OrganizationEmail> OrganizationEmails { get; set; }
        public virtual DbSet<OrganizationWebPage> OrganizationWebPages { get; set; }
        public virtual DbSet<OrganizationName> OrganizationNames { get; set; }
        public virtual DbSet<OrganizationDisplayNameType> OrganizationDisplayNameTypes { get; set; }
        public virtual DbSet<OrganizationDescription> OrganizationDescriptions { get; set; }
        public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }
        public virtual DbSet<OrganizationTypeName> OrganizationTypeNames { get; set; }
        public virtual DbSet<OrganizationPhone> OrganizationPhones { get; set; }
        public virtual DbSet<OrganizationService> OrganizationServices { get; set; }
        public virtual DbSet<OrganizationServiceAdditionalInformation> OrganizationServiceAdditionalInformations { get; set; }
        public virtual DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public virtual DbSet<PhoneNumberTypeName> PhoneNumberTypeNames { get; set; }
        public virtual DbSet<PrintableFormChannelUrlType> PrintableFormChannelUrlTypes { get; set; }
        public virtual DbSet<PrintableFormChannelUrlTypeName> PrintableFormChannelUrlTypeNames { get; set; }
        public virtual DbSet<ProvisionType> ProvisionTypes { get; set; }
        public virtual DbSet<ProvisionTypeName> ProvisionTypeNames { get; set; }
        public virtual DbSet<PublishingStatusType> PublishingStatusTypes { get; set; }
        public virtual DbSet<PublishingStatusTypeName> PublishingStatusTypeNames { get; set; }
        public virtual DbSet<RoleType> RoleTypes { get; set; }
        public virtual DbSet<RoleTypeName> RoleTypeNames { get; set; }
        public virtual DbSet<ServiceVersioned> ServicesVersioned { get; set; }
        public virtual DbSet<ServiceClass> ServiceClasses { get; set; }
        public virtual DbSet<ServiceClassName> ServiceClassNames { get; set; }
        public virtual DbSet<IndustrialClass> IndustrialClasses { get; set; }
        public virtual DbSet<IndustrialClassName> IndustrialClassNames { get; set; }       
        public virtual DbSet<ServiceChannelVersioned> ServiceChannelsVersioned { get; set; }
        public virtual DbSet<ServiceChannelAttachment> ServiceChannelAttachments { get; set; }
        public virtual DbSet<ServiceChargeType> ServiceChargeTypes { get; set; }
        public virtual DbSet<ServiceChargeTypeName> ServiceChargeTypeNames { get; set; }
        public virtual DbSet<ServiceChannelDescription> ServiceChannelDescriptions { get; set; }
        public virtual DbSet<ServiceChannelEmail> ServiceChannelEmails { get; set; }
        public virtual DbSet<ServiceChannelKeyword> ServiceChannelKeywords { get; set; }
        public virtual DbSet<ServiceChannelName> ServiceChannelNames { get; set; }
        public virtual DbSet<ServiceChannelOntologyTerm> ServiceChannelOntologyTerms { get; set; }
        public virtual DbSet<ServiceChannelPhone> ServiceChannelPhones { get; set; }
        public virtual DbSet<ServiceChannelServiceClass> ServiceChannelServiceClasses { get; set; }
        public virtual DbSet<ServiceChannelServiceHours> ServiceChannelServiceHours { get; set; }
        public virtual DbSet<DailyOpeningTime> DailyOpeningTimes { get; set; }
        public virtual DbSet<ServiceChannelTargetGroup> ServiceChannelTargetGroups { get; set; }
        public virtual DbSet<ServiceChannelType> ServiceChannelTypes { get; set; }
        public virtual DbSet<ServiceChannelTypeName> ServiceChannelTypeNames { get; set; }
        public virtual DbSet<ServiceChannelWebPage> ServiceChannelWebPages { get; set; }
        public virtual DbSet<ServiceChannelLanguage> ServiceChannelLanguages { get; set; }
        public virtual DbSet<ServiceDescription> ServiceDescriptions { get; set; }
        public virtual DbSet<ServiceHourType> ServiceHourTypes { get; set; }
        public virtual DbSet<ServiceHourTypeName> ServiceHourTypeNames { get; set; }
        public virtual DbSet<ServiceElectronicCommunicationChannel> ServiceElectronicComunicationChannels { get; set; }
        public virtual DbSet<ServiceElectronicNotificationChannel> ServiceElectronicNotificationChannels { get; set; }
        public virtual DbSet<ServiceHoursAdditionalInformation> ServiceHoursAdditionalInformations { get; set; }
        public virtual DbSet<ServiceKeyword> ServiceKeywords { get; set; }
        public virtual DbSet<ServiceLanguage> ServiceLanguages { get; set; }
        public virtual DbSet<ServiceLaw> ServiceLaws { get; set; }
        public virtual DbSet<ServiceName> ServiceNames { get; set; }
        public virtual DbSet<ServiceLifeEvent> ServiceLifeEvents { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceLocationChannelAddress> ServiceLocationChannelAddresses { get; set; }
        public virtual DbSet<ServiceLocationChannel> ServiceLocationChannels { get; set; }        
        public virtual DbSet<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
        public virtual DbSet<ServiceRequirement> ServiceRequirements { get; set; }
        public virtual DbSet<ServiceServiceClass> ServiceServiceClasses { get; set; }
        public virtual DbSet<ServiceIndustrialClass> ServiceIndustrialClass { get; set; }
        public virtual DbSet<ServiceTargetGroup> ServiceTargetGroups { get; set; }
        public virtual DbSet<StreetName> StreetAddresses { get; set; }
        public virtual DbSet<PostOfficeBoxName> PostOfficeBoxNames { get; set; }
        public virtual DbSet<StatutoryServiceDescription> StatutoryServiceDescriptions { get; set; }
        public virtual DbSet<StatutoryServiceGeneralDescriptionVersioned> StatutoryServiceGeneralDescriptionsVersioned { get; set; }
        public virtual DbSet<StatutoryServiceLanguage> StatutoryServiceLanguages { get; set; }
        public virtual DbSet<StatutoryServiceLifeEvent> StatutoryServiceLifeEvents { get; set; }
        public virtual DbSet<StatutoryServiceName> StatutoryServiceNames { get; set; }
        public virtual DbSet<StatutoryServiceOntologyTerm> StatutoryServiceOntologyTerms { get; set; }
        public virtual DbSet<StatutoryServiceServiceClass> StatutoryServiceServiceClasses { get; set; }
        public virtual DbSet<StatutoryServiceTargetGroup> StatutoryServiceTargetGroups { get; set; }
        public virtual DbSet<StatutoryServiceIndustrialClass> StatutoryServiceIndustrialClass { get; set; }
        public virtual DbSet<StatutoryServiceLaw> StatutoryServiceLaw { get; set; }
        public virtual DbSet<StatutoryServiceRequirement> StatutoryServiceRequirement { get; set; }
        public virtual DbSet<TargetGroup> TargetGroups { get; set; }
        public virtual DbSet<TargetGroupName> TargetGroupNames { get; set; }
        public virtual DbSet<UserOrganization> UserOrganizations { get; set; }
        public virtual DbSet<WebpageChannel> WebpageChannels { get; set; }
        public virtual DbSet<WebpageChannelUrl> WebpageChannelUrls { get; set; }
        public virtual DbSet<WebPage> WebPages { get; set; }
        public virtual DbSet<WebPageType> WebPageTypes { get; set; }
        public virtual DbSet<WebPageTypeName> WebPageTypeNames { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
		public virtual DbSet<Locking> Locking { get; set; }
        public virtual DbSet<Versioning> Versioning { get; set; }
        public virtual DbSet<Law> Law { get; set; }
        public virtual DbSet<LawName> LawName { get; set; }
        public virtual DbSet<LawWebPage> LawWebPage { get; set; }
        public virtual DbSet<DialCode> DialCodes { get; set; }
        public virtual DbSet<GeneralDescriptionLanguageAvailability> GeneralDescriptionLanguageAvailabilities { get; set; }
        public virtual DbSet<ServiceLanguageAvailability> ServiceLanguageAvailabilities { get; set; }
        public virtual DbSet<ServiceChannelLanguageAvailability> ServiceChannelLanguageAvailabilities { get; set; }
        public virtual DbSet<OrganizationLanguageAvailability> OrganizationLanguageAvailabilities { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceChannel> ServiceChannels { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<StatutoryServiceGeneralDescription> StatutoryServiceGeneralDescriptions { get; set; }
		public virtual DbSet<PostalCodeName> PostalCodeNames { get; set; }
        public virtual DbSet<Coordinate> Coordinates { get; set; }
        public virtual DbSet<CoordinateType> CoordinateTypes { get; set; }
        public virtual DbSet<CoordinateTypeName> CoordinateTypeNames { get; set; }
        public virtual DbSet<OntologyTermExactMatch> OntologyTermExactMatches { get; set; }
        public virtual DbSet<ExactMatch> ExactMatches { get; set; }
        public virtual DbSet<AreaInformationType> AreaInformationTypes { get; set; }
        public virtual DbSet<AreaInformationTypeName> AreaInformationTypeNames { get; set; }
        public virtual DbSet<AreaType> AreaTypes { get; set; }
        public virtual DbSet<AreaTypeName> AreaTypeNames { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<AreaMunicipality> AreaMunicipalities { get; set; }
        public virtual DbSet<AreaName> AreaNames { get; set; }
        public virtual DbSet<OrganizationArea> OrganizationAreas { get; set; }
        public virtual DbSet<OrganizationAreaMunicipality> OrganizationAreaMunicipalities { get; set; }
        public virtual DbSet<ServiceArea> ServiceAreas { get; set; }
        public virtual DbSet<ServiceAreaMunicipality> ServiceAreaMunicipalities { get; set; }
        public virtual DbSet<ServiceChannelArea> ServiceChannelAreas { get; set; }
        public virtual DbSet<ServiceChannelAreaMunicipality> ServiceChannelAreaMunicipalities { get; set; }
        public virtual DbSet<ServiceChannelConnectionType> ServiceChannelConnectionType { get; set; }
        public virtual DbSet<ServiceChannelConnectionTypeName> ServiceChannelConnectionTypeName { get; set; }
        public virtual DbSet<ServiceFundingType> ServiceFundingType { get; set; }
        public virtual DbSet<ServiceFundingTypeName> ServiceFundingTypeName { get; set; }

//        public virtual DbSet<Privilege> Privileges { get; set; }
//        public virtual DbSet<UserPrivilege> UserPrivileges { get; set; }
//        public virtual DbSet<AccessModeType> AccessModeTypes { get; set; }
//        public virtual DbSet<AccessModeName> AccessModeNames { get; set; }
//        public virtual DbSet<PrivilegeServiceAssociation> PrivilegeServiceAssociations { get; set; }
//        public virtual DbSet<PrivilegeChannelAssociation> PrivilegeChannelAssociations { get; set; }
//        public virtual DbSet<PrivilegeOrganizationAssociation> PrivilegeOrganizationAssociations { get; set; }
//        public virtual DbSet<PrivilegeGeneralDescriptionAssociation> PrivilegeGeneralDescriptionAssociations { get; set; }


        public virtual DbSet<BugReport> BugReports { get; set; }

        public virtual DbSet<ServiceBlockedAccessRight> ServiceBlockedAccessRights { get; set; }
        public virtual DbSet<ChannelBlockedAccessRight> ChannelBlockedAccessRights { get; set; }
        public virtual DbSet<OrganizationBlockedAccessRight> OrganizationBlockedAccessRights { get; set; }
        public virtual DbSet<GeneralDescriptionBlockedAccessRight> GeneralDescriptionBlockedAccessRights { get; set; }
        public virtual DbSet<AccessRightType> AccessRightTypes { get; set; }
        public virtual DbSet<AccessRightName> AccessRightNames { get; set; }
        public virtual DbSet<UserAccessRight> UserAccessRights { get; set; }
    }
}
