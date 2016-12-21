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

namespace PTV.Database.DataAccess.ApplicationDbContext
{
    internal partial class PtvDbContext : DbContext
    {
        private EntityNavigationsMap entityNavigationsMap;
        private ICacheBuilder cacheBuilder;

        public PtvDbContext(DbContextOptions<PtvDbContext> options, EntityNavigationsMap entityNavigationsMap, ICacheBuilder cacheBuilder) : base(options)
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
                entity.HasOne(e => e.Language).WithMany(p => p.LanguageNames).HasForeignKey(d => d.LanguageId);
                entity.HasOne(e => e.Localization).WithMany(p => p.Localizations).HasForeignKey(d => d.LocalizationId);
            });

            modelBuilder.Entity<OntologyTermParent>(entity =>
            {
                entity.HasKey(e => new { e.ParentId, e.ChildId });
                entity.HasOne(e => e.Parent).WithMany(p => p.Children).HasForeignKey(d => d.ParentId);
                entity.HasOne(e => e.Child).WithMany(p => p.Parents).HasForeignKey(d => d.ChildId);
            });

            modelBuilder.Entity<OrganizationAddress>(entity =>
            {
                entity.HasKey(e => new {e.OrganizationId, e.AddressId, e.TypeId});
            });

            modelBuilder.Entity<OrganizationDescription>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationId, e.TypeId, e.LocalizationId});
            });

            modelBuilder.Entity<OrganizationEmail>(entity =>
            {
                entity.HasKey(e => new { e.EmailId, e.OrganizationId});
            });

            modelBuilder.Entity<OrganizationName>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationId, e.TypeId, e.LocalizationId});
            });

            modelBuilder.Entity<OrganizationPhone>(entity =>
            {
                entity.HasKey(e => new { e.PhoneId, e.OrganizationId});
            });

            modelBuilder.Entity<OrganizationServiceWebPage>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationServiceId, e.WebPageId, e.TypeId });
            });

            modelBuilder.Entity<OrganizationWebPage>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationId, e.WebPageId, e.TypeId });
            });

            modelBuilder.Entity<ServiceDescription>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceName>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceMunicipality>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.MunicipalityId });
            });

            modelBuilder.Entity<ServiceKeyword>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.KeywordId });
            });

            modelBuilder.Entity<ServiceLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.LanguageId });
            });

            modelBuilder.Entity<ServiceLifeEvent>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.LifeEventId });
            });

            modelBuilder.Entity<ServiceOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.OntologyTermId });
            });

            modelBuilder.Entity<ServiceServiceClass>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceClassId });
            });

            modelBuilder.Entity<ServiceIndustrialClass>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.IndustrialClassId });
            });

            modelBuilder.Entity<ServiceServiceChannel>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId });
            });

            modelBuilder.Entity<ServiceTargetGroup>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.TargetGroupId });
            });

            modelBuilder.Entity<ServiceWebPage>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.WebPageId });
            });

            modelBuilder.Entity<ServiceChannelDescription>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelName>(entity =>
            {
                entity.HasKey(e => new {e.ServiceChannelId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelWebPage>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.WebPageId });
            });

            modelBuilder.Entity<StatutoryServiceName>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<StatutoryServiceDescription>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<StatutoryServiceServiceClass>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.ServiceClassId });
            });

            modelBuilder.Entity<StatutoryServiceTargetGroup>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.TargetGroupId });
            });

            modelBuilder.Entity<StatutoryServiceIndustrialClass>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.IndustrialClassId });
            });

            modelBuilder.Entity<StatutoryServiceLaw>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.LawId });
            });

            modelBuilder.Entity<StatutoryServiceLanguage>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.LanguageId });
            });

            modelBuilder.Entity<StatutoryServiceOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.OntologyTermId });
            });

            modelBuilder.Entity<StatutoryServiceLifeEvent>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.LifeEventId });
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
                entity.HasKey(e => new { e.ServiceChannelId, e.TargetGroupId });
            });

            modelBuilder.Entity<ServiceLocationChannelAddress>(entity =>
            {
                entity.HasKey(e => new { e.ServiceLocationChannelId, e.AddressId, e.TypeId });
            });
            modelBuilder.Entity<ServiceLocationChannelServiceArea>(entity =>
            {
                entity.HasKey(e => new { e.ServiceLocationChannelId, e.MunicipalityId });
            });

            modelBuilder.Entity<ServiceChannelOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.OntologyTermId });
            });

            modelBuilder.Entity<ServiceChannelKeyword>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.KeywordId });
            });

            modelBuilder.Entity<ServiceChannelServiceClass>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.ServiceClassId });
            });

            modelBuilder.Entity<ServiceChannelLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.LanguageId });
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
                entity.HasKey(e => new { e.ServiceChannelId, e.AttachmentId });
            });

            modelBuilder.Entity<AddressAdditionalInformation>(entity =>
            {
                entity.HasKey(e => new { e.AddressId, e.LocalizationId });
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<ServiceChannelPhone>(entity =>
            {
                entity.HasKey(e => new { e.PhoneId, e.ServiceChannelId });
            });

            modelBuilder.Entity<ServiceChannelEmail>(entity =>
            {
                entity.HasKey(e => new { e.EmailId, e.ServiceChannelId });
            });

			modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => e.Oid);
            });

            modelBuilder.Entity<OrganizationService>(entity =>
            {
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.ServiceId);
            });

            modelBuilder.Entity<ServiceServiceChannelDescription>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.LocalizationId, e.ServiceChannelId, e.ServiceId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelDescriptions).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });

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

            EntityFrameworkEntityTools.CreateIndexes(modelBuilder);
            entityNavigationsMap.NavigationsMap = EntityFrameworkEntityTools.BuildEntityPropertiesMap(modelBuilder);
            cacheBuilder.TypeBaseEntities = EntityFrameworkEntityTools.GetEntitiesOfBaseType<TypeBase>(modelBuilder);
            EntityFrameworkEntityTools.BuildContextInfo(modelBuilder);
            EntityFrameworkEntityTools.DataContextInfo.EntitySetsMap = this.BuildSetMap();
        }


        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<AddressTypeName> AddressTypeNames { get; set; }
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
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormType> FormTypes { get; set; }
        public virtual DbSet<FormTypeName> FormTypeNames { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageName> LanguageNames { get; set; }
        public virtual DbSet<LifeEvent> LifeEvents { get; set; }
        public virtual DbSet<LifeEventName> LifeEventNames { get; set; }
        public virtual DbSet<Municipality> Municipalities { get; set; }
        public virtual DbSet<NameType> NameTypes { get; set; }
        public virtual DbSet<NameTypeName> NameTypeNames { get; set; }
        public virtual DbSet<OntologyTerm> OntologyTerms { get; set; }
        public virtual DbSet<OntologyTermName> OntologyTermNames { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationAddress> OrganizationAddresses { get; set; }
        public virtual DbSet<OrganizationEmail> OrganizationEmails { get; set; }
        public virtual DbSet<OrganizationWebPage> OrganizationWebPages { get; set; }
        public virtual DbSet<OrganizationName> OrganizationNames { get; set; }
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
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceClass> ServiceClasses { get; set; }
        public virtual DbSet<ServiceClassName> ServiceClassNames { get; set; }
        public virtual DbSet<IndustrialClass> IndustrialClasses { get; set; }
        public virtual DbSet<IndustrialClassName> IndustrialClassNames { get; set; }
        public virtual DbSet<ServiceCoverageType> ServiceCoverageTypes { get; set; }
        public virtual DbSet<ServiceCoverageTypeName> ServiceCoverageTypeNames { get; set; }
        public virtual DbSet<ServiceChannel> ServiceChannels { get; set; }
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
        public virtual DbSet<ServiceName> ServiceNames { get; set; }
        public virtual DbSet<ServiceLifeEvent> ServiceLifeEvents { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceLocationChannelAddress> ServiceLocationChannelAddresses { get; set; }
        public virtual DbSet<ServiceLocationChannel> ServiceLocationChannels { get; set; }
        public virtual DbSet<ServiceLocationChannelServiceArea> ServiceLocationChannelServiceAreas { get; set; }
        public virtual DbSet<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
        public virtual DbSet<ServiceRequirement> ServiceRequirements { get; set; }
        public virtual DbSet<ServiceServiceClass> ServiceServiceClasses { get; set; }
        public virtual DbSet<ServiceIndustrialClass> ServiceIndustrialClass { get; set; }
        public virtual DbSet<ServiceTargetGroup> ServiceTargetGroups { get; set; }
        public virtual DbSet<StreetName> StreetAddresses { get; set; }
        public virtual DbSet<StatutoryServiceDescription> StatutoryServiceDescriptions { get; set; }
        public virtual DbSet<StatutoryServiceGeneralDescription> StatutoryServiceGeneralDescriptions { get; set; }
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
    }
}
