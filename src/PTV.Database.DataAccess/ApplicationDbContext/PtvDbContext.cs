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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models.Base;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Models.Privileges;
using PTV.Database.Model.Views;

namespace PTV.Database.DataAccess.ApplicationDbContext
{
    internal partial class PtvDbContext : DbContext
    {
        private readonly IEntityNavigationsMap entityNavigationsMap;
        private readonly ICacheBuilder cacheBuilder;

        public PtvDbContext(DbContextOptions<PtvDbContext> options, IEntityNavigationsMap entityNavigationsMap, ICacheBuilder cacheBuilder) : base(options)
        {
            this.entityNavigationsMap = entityNavigationsMap;
            this.cacheBuilder = cacheBuilder;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(i => i.Throw());
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(i => i.Throw());
        }

        private void AddKeysForTypeNames(ModelBuilder modelBuilder)
        {
            List<Type> types = new List<Type>
            {
                typeof(AccessRightName),
                typeof(AddressTypeName),
                typeof(AppEnvironmentDataTypeName),
                typeof(AreaInformationTypeName),
                typeof(AreaTypeName),
                typeof(AttachmentTypeName),
                typeof(CoordinateTypeName),
                typeof(DescriptionTypeName),
                typeof(ExceptionHoursStatusTypeName),
                typeof(NameTypeName),
                typeof(PhoneNumberTypeName),
                typeof(PrintableFormChannelUrlTypeName),
                typeof(ProvisionTypeName),
                typeof(PublishingStatusTypeName),
                typeof(ServiceChannelConnectionTypeName),
                typeof(ServiceChannelTypeName),
                typeof(ServiceChargeTypeName),
                typeof(ServiceFundingTypeName),
                typeof(ServiceHourTypeName),
                typeof(ServiceTypeName),
                typeof(GeneralDescriptionTypeName),
                typeof(AddressCharacterName),
                typeof(WebPageTypeName),
                typeof(TranslationStateTypeName),
                typeof(AccessibilityClassificationLevelTypeName),
                typeof(WcagLevelTypeName),
                typeof(HolidayName),
                typeof(JobExecutionTypeName),
                typeof(JobStatusTypeName),
                typeof(VoucherTypeName),
            };

            types.ForEach(x =>
                {
                    modelBuilder.Entity(x, entity =>
                    {
                        entity.HasKey("TypeId", "LocalizationId");
                    });
                }
            );

            modelBuilder.Entity<OrganizationTypeName>(entity =>
            {
                entity.HasKey(x => new {x.OrganizationTypeId, x.LocalizationId});
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.RemovePluralizingTableNameConvention();

            AddKeysForTypeNames(modelBuilder);

            modelBuilder.Entity<LanguageName>(entity =>
            {
                entity.HasOne(e => e.Language).WithMany(p => p.Names).HasForeignKey(d => d.LanguageId);
                entity.HasOne(e => e.Localization).WithMany(p => p.Localizations).HasForeignKey(d => d.LocalizationId);
                entity.HasKey(e => new { e.LanguageId, e.LocalizationId });
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

            modelBuilder.Entity<CountryName>(entity =>
            {
                entity.HasKey(e => new { e.CountryId, e.LocalizationId });
            });

            modelBuilder.Entity<PostOfficeBoxName>(entity =>
            {
                entity.HasKey(e => new { e.AddressPostOfficeBoxId, e.LocalizationId });
            });

            modelBuilder.Entity<AddressForeignTextName>(entity =>
            {
                entity.HasKey(e => new { e.AddressForeignId, e.LocalizationId });
            });

            modelBuilder.Entity<AreaMunicipality>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.MunicipalityId });
            });

            modelBuilder.Entity<OrganizationAddress>(entity =>
            {
                entity.HasKey(e => new {OrganizationId = e.OrganizationVersionedId, e.AddressId, e.CharacterId});
            });

            modelBuilder.Entity<OrganizationEInvoicingAdditionalInformation>(entity => {
                entity.HasKey(e => new { e.OrganizationEInvoicingId, e.LocalizationId });
                entity.HasOne(e => e.OrganizationEInvoicing).WithMany(e => e.EInvoicingAdditionalInformations).HasForeignKey(i => i.OrganizationEInvoicingId).HasPrincipalKey(i => i.Id);
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

            modelBuilder.Entity<StatutoryServiceGeneralDescriptionKeyword>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionVersionedId, e.KeywordId });
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
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId}).HasName("PK_ServiceServiceChannel");
                entity.HasIndex(e => e.ServiceId);
                entity.HasIndex(e => e.ServiceChannelId);
            });

            modelBuilder.Entity<GeneralDescriptionServiceChannel>(entity =>
            {
                entity.HasKey(e => new { StatutoryServiceGeneralDescriptionId = e.StatutoryServiceGeneralDescriptionId, e.ServiceChannelId });//.ForNpgsqlHasName("PK_GenDesSerChan");
                entity.HasOne(e => e.ChargeType).WithMany().HasForeignKey(d => d.ChargeTypeId);//.ForNpgsqlHasConstraintName("FK_GenDesSerChan_ChaTypId");
                entity.HasOne(e => e.ServiceChannel).WithMany(d=>d.StatutoryServiceGeneralDescriptionServiceChannels).HasForeignKey(d => d.ServiceChannelId);//.ForNpgsqlHasConstraintName("FK_GenDesSerChan_SerChaId");
                entity.HasOne(e => e.StatutoryServiceGeneralDescription).WithMany(d=>d.StatutoryServiceGeneralDescriptionServiceChannels).HasForeignKey(d => d.StatutoryServiceGeneralDescriptionId);//.ForNpgsqlHasConstraintName("FK_GenDesSerChan_GenDesId");
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

            modelBuilder.Entity<ServiceChannelOntologyTerm>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.OntologyTermId });
            });

            modelBuilder.Entity<ServiceChannelKeyword>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.KeywordId });
            });

            modelBuilder.Entity<ServiceChannelLanguage>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.LanguageId });
            });

            modelBuilder.Entity<ServiceHoursAdditionalInformation>(entity =>
            {
                entity.HasKey(e => new { e.ServiceHoursId, e.LocalizationId });
            });

            modelBuilder.Entity<DailyOpeningTime>(entity =>
            {
                entity.HasKey(e => new { e.OpeningHourId, e.DayFrom, e.OrderNumber });
            });

            modelBuilder.Entity<AuthorizationEntryPoint>(entity =>
            {
                entity.HasKey(e => new { e.Id });
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
                entity.HasKey(e => new { e.OrganizationId, e.ServiceVersionedId });
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.ServiceVersionedId);
            });

            modelBuilder.Entity<ServiceCollectionVersioned>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<ServiceCollectionService>(entity =>
            {
                entity.HasKey(e => new { e.ServiceCollectionId, e.ServiceId});
                entity.HasIndex(e => e.ServiceId);
                entity.HasIndex(e => e.ServiceCollectionId);
            });

            modelBuilder.Entity<ServiceCollectionServiceChannel>(entity =>
            {
                entity.HasKey(e => new { e.ServiceCollectionId, e.ServiceChannelId});
                entity.HasIndex(e => e.ServiceChannelId);
                entity.HasIndex(e => e.ServiceCollectionId);
            });

            modelBuilder.Entity<ServiceCollectionName>(entity =>
            {
                entity.HasKey(e => new { ServiceCollectionId = e.ServiceCollectionVersionedId, e.TypeId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceCollectionDescription>(entity =>
            {
                entity.HasKey(e => new { ServiceCollectionId = e.ServiceCollectionVersionedId, e.TypeId, e.LocalizationId });
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

            modelBuilder.Entity<UserAccessRightsGroupName>(entity =>
            {
                entity.HasKey(e => new { e.UserAccessRightsGroupId, e.LocalizationId });
            });

            modelBuilder.Entity<DigitalAuthorizationName>(entity =>
            {
                entity.HasKey(e => new { e.DigitalAuthorizationId, e.LocalizationId });
            });

            modelBuilder.Entity<IndustrialClassName>(entity =>
            {
                entity.HasKey(e => new { e.IndustrialClassId, e.LocalizationId });
            });
            modelBuilder.Entity<LifeEventName>(entity =>
            {
                entity.HasKey(e => new { e.LifeEventId, e.LocalizationId });
            });
            modelBuilder.Entity<TargetGroupName>(entity =>
            {
                entity.HasKey(e => new { e.TargetGroupId, e.LocalizationId });
            });
            modelBuilder.Entity<OntologyTermName>(entity =>
            {
                entity.HasKey(e => new { e.OntologyTermId, e.LocalizationId });
            });
            modelBuilder.Entity<ServiceClassName>(entity =>
            {
                entity.HasKey(e => new { e.ServiceClassId, e.LocalizationId });
            });
            modelBuilder.Entity<ServiceClassDescription>(entity =>
            {
                entity.HasKey(e => new { e.ServiceClassId, e.LocalizationId });
            });
            modelBuilder.Entity<IndustrialClassName>(entity =>
            {
                entity.HasKey(e => new { e.IndustrialClassId, e.LocalizationId });
            });
            modelBuilder.Entity<IndustrialClassDescription>(entity =>
            {
                entity.HasKey(e => new { e.IndustrialClassId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceServiceChannelDigitalAuthorization>(entity =>
            {
                entity.HasKey(e => new { e.DigitalAuthorizationId, e.ServiceId, e.ServiceChannelId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelDigitalAuthorizations).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
            });

            modelBuilder.Entity<Versioning>(entity =>
            {
                entity.HasOne(e => e.PreviousVersion).WithMany(e => e.ChildrenVersions).HasForeignKey(e => e.PreviousVersionId);
                entity.HasIndex(e => e.UnificRootId);
            });

            modelBuilder.Entity<LawName>(entity =>
            {
                entity.HasKey(e => new { e.LawId, e.LocalizationId });
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

            modelBuilder.Entity<ServiceCollectionLanguageAvailability>(entity =>
            {
                entity.HasKey(e => new { ServiceCollectionId = e.ServiceCollectionVersionedId, e.LanguageId });
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
// SOTE has been disabled (SFIPTV-1177)
//                entity.HasOne(e => e.ResponsibleOrganizationRegion).WithMany(e => e.RegionRelatedOrganizations).HasForeignKey(e => e.ResponsibleOrganizationRegionId);
            });


            modelBuilder.Entity<PrintableFormChannelIdentifier>(entity =>
            {
                entity.HasKey(e => new { e.PrintableFormChannelId, e.LocalizationId });
            });

            modelBuilder.Entity<UserAccessRight>(entity =>
            {
                entity.HasKey(e => new { e.AccessRightId, e.UserId });
            });

            modelBuilder.Entity<ServiceServiceChannelExtraType>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelExtraTypes).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
            });

            modelBuilder.Entity<ServiceServiceChannelExtraTypeDescription>(entity =>
            {
                entity.HasKey(e => new { e.LocalizationId, e.ServiceServiceChannelExtraTypeId });
                entity.HasOne(e => e.ServiceServiceChannelExtraType).WithMany(p => p.ServiceServiceChannelExtraTypeDescriptions).HasForeignKey(d => new { d.ServiceServiceChannelExtraTypeId });
            });

            modelBuilder.Entity<ExtraTypeName>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.LocalizationId });
                entity.HasOne(e => e.Type).WithMany(p => p.Names).HasForeignKey(d => new { d.TypeId });

            });

            modelBuilder.Entity<ExtraSubTypeName>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.LocalizationId });
                entity.HasOne(e => e.Type).WithMany(p => p.Names).HasForeignKey(d => new { d.TypeId });

            });

            modelBuilder.Entity<ExtraSubType>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.HasOne(e => e.ExtraType).WithMany(p => p.ExtraSubType).HasForeignKey(d => new { d.ExtraTypeId });

            });

            modelBuilder.Entity<ServiceProducerOrganization>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationId, e.ServiceProducerId });
            });
            modelBuilder.Entity<PostOfficeBoxName>(entity =>
            {
                entity.HasKey(e => new { e.AddressPostOfficeBoxId, e.LocalizationId });
            });
            modelBuilder.Entity<AddressForeignTextName>(entity =>
            {
                entity.HasKey(e => new { e.AddressForeignId, e.LocalizationId });
            });
            modelBuilder.Entity<AddressPostOfficeBox>(entity =>
            {
                entity.HasKey(e => new { e.AddressId });
                entity.HasOne(e => e.Address).WithMany(e => e.AddressPostOfficeBoxes).HasForeignKey(i => i.AddressId).HasPrincipalKey(i => i.Id);
            });
            modelBuilder.Entity<AddressForeign>(entity =>
            {
                entity.HasKey(e => new { e.AddressId });
                entity.HasOne(e => e.Address).WithMany(e => e.AddressForeigns).HasForeignKey(i => i.AddressId).HasPrincipalKey(i => i.Id);
            });
            modelBuilder.Entity<AddressOther>(entity =>
            {
                entity.HasKey(e => new { e.AddressId });
                entity.HasOne(e => e.Address).WithMany(e => e.AddressOthers).HasForeignKey(i => i.AddressId).HasPrincipalKey(i => i.Id);
            });
            modelBuilder.Entity<ServiceChannelServiceHours>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelVersionedId, e.ServiceHoursId });
                entity.HasOne(e => e.ServiceChannelVersioned).WithMany(p => p.ServiceChannelServiceHours).HasForeignKey(d => new { d.ServiceChannelVersionedId });
                entity.HasOne(e => e.ServiceHours).WithMany(p => p.ServiceChannelServiceHours).HasForeignKey(d => new { d.ServiceHoursId });
            });
            modelBuilder.Entity<ServiceServiceChannelServiceHours>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId, e.ServiceHoursId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelServiceHours).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
                entity.HasOne(e => e.ServiceHours).WithMany(p => p.ServiceServiceChannelServiceHours).HasForeignKey(d => new { d.ServiceHoursId });
            });
            modelBuilder.Entity<ServiceServiceChannelAddress>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId, e.AddressId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelAddresses).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
            });
            modelBuilder.Entity<ServiceServiceChannelEmail>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId, e.EmailId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelEmails).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
                entity.HasOne(e => e.Email).WithMany(p => p.ServiceServiceChannelEmails).HasForeignKey(d => new { d.EmailId });
            });
            modelBuilder.Entity<ServiceServiceChannelPhone>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.ServiceChannelId, e.PhoneId });
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelPhones).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
                entity.HasOne(e => e.Phone).WithMany(p => p.ServiceServiceChannelPhones).HasForeignKey(d => new { d.PhoneId });
            });
            modelBuilder.Entity<ServiceServiceChannelWebPage>(entity =>
            {
                entity.HasOne(e => e.ServiceServiceChannel).WithMany(p => p.ServiceServiceChannelWebPages).HasForeignKey(d => new { d.ServiceId, d.ServiceChannelId });
                entity.HasOne(e => e.WebPage).WithMany(p => p.ServiceServiceChannelWebPages).HasForeignKey(d => new { d.WebPageId });
            });
            modelBuilder.Entity<SahaOrganizationInformation>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationId, e.SahaId });
                entity.HasOne(e => e.Organization).WithMany(e => e.SahaOrganizationInformations).HasForeignKey(i => i.OrganizationId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<TranslationStateTypeName>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.LocalizationId });
                entity.HasOne(e => e.Type).WithMany(p => p.Names).HasForeignKey(d => new { d.TypeId });
            });

            modelBuilder.Entity<TranslationOrderState>(entity =>
            {
                entity.HasOne(e => e.TranslationOrder).WithMany(e => e.TranslationOrderStates).HasForeignKey(i => i.TranslationOrderId).HasPrincipalKey(i => i.Id);
                entity.HasOne(e => e.TranslationState).WithMany(e => e.TranslationOrderStates).HasForeignKey(i => i.TranslationStateId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<TranslationOrder>(entity =>
            {
                entity.HasOne(e => e.TranslationCompany).WithMany(e => e.TranslationOrders).HasForeignKey(i => i.TranslationCompanyId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<ServiceTranslationOrder>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.TranslationOrderId });
                entity.HasOne(e => e.TranslationOrder).WithMany(e => e.ServiceTranslationOrders).HasForeignKey(i => i.TranslationOrderId).HasPrincipalKey(i => i.Id);
                entity.HasOne(e => e.Service).WithMany(e => e.ServiceTranslationOrders).HasForeignKey(i => i.ServiceId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<ServiceChannelTranslationOrder>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelId, e.TranslationOrderId });
                entity.HasOne(e => e.TranslationOrder).WithMany(e => e.ServiceChannelTranslationOrders).HasForeignKey(i => i.TranslationOrderId).HasPrincipalKey(i => i.Id);
                entity.HasOne(e => e.ServiceChannel).WithMany(e => e.ServiceChannelTranslationOrders).HasForeignKey(i => i.ServiceChannelId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<GeneralDescriptionTranslationOrder>(entity =>
            {
                entity.HasKey(e => new { e.StatutoryServiceGeneralDescriptionId, e.TranslationOrderId });
                entity.HasOne(e => e.TranslationOrder).WithMany(e => e.GeneralDescriptionTranslationOrders).HasForeignKey(i => i.TranslationOrderId).HasPrincipalKey(i => i.Id);
                entity.HasOne(e => e.StatutoryServiceGeneralDescription).WithMany(e => e.GeneralDescriptionTranslationOrders).HasForeignKey(i => i.StatutoryServiceGeneralDescriptionId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<AccessibilityRegisterGroupValue>(entity =>
            {
                entity.HasKey(e => new { e.AccessibilityRegisterGroupId, e.LocalizationId });
            });

            modelBuilder.Entity<AccessibilityRegisterSentenceValue>(entity =>
            {
                entity.HasKey(e => new { e.AccessibilityRegisterSentenceId, e.LocalizationId });
            });

            modelBuilder.Entity<AccessibilityRegisterEntranceName>(entity =>
            {
                entity.HasKey(e => new { e.AccessibilityRegisterEntranceId, e.LocalizationId });
            });

            modelBuilder.Entity<AccessibilityRegister>(entity =>
            {
                entity.HasIndex(e => e.ServiceChannelId).IsUnique();
            });
            
            modelBuilder.Entity<ServiceChannelAddress>(entity =>
            {
                entity.HasKey(e => new { e.ServiceChannelVersionedId, e.AddressId});
                entity.HasIndex(e => e.ServiceChannelVersionedId);
                entity.HasIndex(e => e.AddressId);
            });

            modelBuilder.Entity<AddressReceiver>(entity =>
            {
                entity.HasKey(e => new {e.AddressId, e.LocalizationId});
                entity.HasIndex(e => e.AddressId);
                entity.HasIndex(e => e.LocalizationId);
            });

            modelBuilder.Entity<OrganizationFilter>(entity =>
            {
                entity.HasKey(e => new {e.FilterId, e.OrganizationId});
                entity.HasIndex(e => e.FilterId);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasOne(i => i.Filter).WithMany(i => i.OrganizationFilters).HasPrincipalKey(i => i.Id).HasForeignKey(i => i.FilterId);
            });

            modelBuilder.Entity<RestrictionFilter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EntityType);
                entity.HasIndex(e => e.FilterName).IsUnique();
                entity.HasIndex(e => e.ColumnName);
            });

            modelBuilder.Entity<ServiceChannelSocialHealthCenter>(entity =>
            {
                entity.HasKey(e => e.ServiceChannelId);
                entity.HasIndex(e => e.Oid);
                entity.HasOne(e => e.ServiceChannel).WithMany(s => s.SocialHealthCenters).HasForeignKey(s => s.ServiceChannelId);
            });

            modelBuilder.Entity<ServiceChannelDisplayNameType>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.LocalizationId });
            });

            modelBuilder.Entity<ServiceChannelAccessibilityClassification>(entity =>
            {
                entity.HasKey(e => new { ServiceChannelId = e.ServiceChannelVersionedId, e.AccessibilityClassificationId });
            });

            modelBuilder.Entity<PhoneExtraType>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.PhoneId, e.ExtraTypeId});
            });

            modelBuilder.Entity<AddressExtraType>(entity =>
            {
                entity.HasKey(e => new { ServiceId = e.AddressId, e.ExtraTypeId});
            });

            modelBuilder.Entity<OntologyTerm>(entity =>
            {
                entity.HasIndex(e => e.Uri);
                entity.HasIndex(e => e.ParentUri);
            });

            modelBuilder.Entity<ServiceClass>(entity =>
            {
                entity.HasIndex(e => e.Uri);
                entity.HasIndex(e => e.ParentUri);
            });

            modelBuilder.Entity<NotificationServiceServiceChannelFilter>(entity =>
            {
                entity.HasKey(e => new { e.NotificationServiceServiceChannelId, e.UserId });
            });

            modelBuilder.Entity<ClsAddressStreetName>(entity =>
            {
                entity.HasKey(e => new { AddressStreetId = e.ClsAddressStreetId, e.LocalizationId });
                entity.HasIndex(e => e.Name3);
            });

            modelBuilder.Entity<HolidayServiceHours>(entity =>
            {
                entity.HasKey(e => new { e.HolidayId, e.ServiceHoursId});
                entity.HasIndex(e => e.HolidayId);
                entity.HasIndex(e => e.ServiceHoursId);
                entity.HasOne(e => e.ServiceHours).WithOne(p => p.HolidayServiceHour).HasForeignKey<HolidayServiceHours>(i => i.ServiceHoursId);
            });

            modelBuilder.Entity<HolidayDate>(entity =>
            {
                entity.HasOne(e => e.Holiday).WithMany(e => e.HolidayDates).HasForeignKey(i => i.HolidayId).HasPrincipalKey(i => i.Id);
            });

            modelBuilder.Entity<Localization>(entity => { entity.HasKey(e => e.LanguageCode); });

            modelBuilder.Entity<WebPage>()
                .HasIndex(x => x.Url)
                .IsUnique();

            modelBuilder.Entity<EFMigrationsHistory>(entity => {
                entity.ToTable("__EFMigrationsHistory");
                entity.HasKey(i => i.MigrationId);
            });

            modelBuilder.Entity<VTasksConfiguration>().ToView(nameof(VTasksConfiguration)).HasNoKey();
            modelBuilder.Entity<VBrokenLink>().ToView(nameof(VBrokenLink)).HasNoKey();
            modelBuilder.Entity<VLastArchivedGeneralDescription>().ToView(nameof(VLastArchivedGeneralDescription)).HasNoKey();
            modelBuilder.Entity<VLastArchivedOrganization>().ToView(nameof(VLastArchivedOrganization)).HasNoKey();
            modelBuilder.Entity<VLastArchivedService>().ToView(nameof(VLastArchivedService)).HasNoKey();
            modelBuilder.Entity<VLastArchivedServiceChannel>().ToView(nameof(VLastArchivedServiceChannel)).HasNoKey();
            modelBuilder.Entity<VLastArchivedServiceCollection>().ToView(nameof(VLastArchivedServiceCollection)).HasNoKey();

            EntityFrameworkEntityTools.CreateIndexes(modelBuilder);
            if (entityNavigationsMap != null && cacheBuilder != null)
            {
                ((EntityNavigationsMap) entityNavigationsMap).NavigationsMap =
                    EntityFrameworkEntityTools.BuildEntityPropertiesMap(modelBuilder);
                cacheBuilder.TypeBaseEntities =
                    EntityFrameworkEntityTools.GetEntitiesOfBaseType<TypeBase>(modelBuilder);
            }

            EntityFrameworkEntityTools.BuildContextInfo(modelBuilder);
            EntityFrameworkEntityTools.DataContextInfo.EntitySetsMap = this.BuildSetMap();
        }


        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AddressPostOfficeBox> AddressPostOfficeBoxes { get; set; }
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<AddressForeign> AddressForeigns { get; set; }
        public virtual DbSet<AddressForeignTextName> AddressForeignTextName { get; set; }
        public virtual DbSet<AddressCharacter> AddressCharacters { get; set; }
        public virtual DbSet<AddressCharacterName> AddressCharacterNames { get; set; }
        public virtual DbSet<AddressOther> AddressOthers { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<AddressTypeName> AddressTypeNames { get; set; }
        // ReSharper disable once IdentifierTypo
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
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<PrintableFormChannelIdentifier> PrintableFormChannelIdentifierss { get; set; }
       
        public virtual DbSet<FormState> FormStates { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageName> LanguageNames { get; set; }
        public virtual DbSet<LifeEvent> LifeEvents { get; set; }
        public virtual DbSet<LifeEventName> LifeEventNames { get; set; }
        public virtual DbSet<Localization> Localizations { get; set; }
        public virtual DbSet<Municipality> Municipalities { get; set; }
        public virtual DbSet<MunicipalityName> MunicipalityNames { get; set; }
        public virtual DbSet<NameType> NameTypes { get; set; }
        public virtual DbSet<NameTypeName> NameTypeNames { get; set; }
        public virtual DbSet<OntologyTerm> OntologyTerms { get; set; }
        public virtual DbSet<OntologyTermName> OntologyTermNames { get; set; }
        public virtual DbSet<OrganizationVersioned> OrganizationsVersioned { get; set; }
        public virtual DbSet<OrganizationAddress> OrganizationAddresses { get; set; }
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<OrganizationEInvoicing> OrganizationEInvoicings { get; set; }
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<OrganizationEInvoicingAdditionalInformation> OrganizationEInvoicingAdditionalInformations { get; set; }
        public virtual DbSet<OrganizationEmail> OrganizationEmails { get; set; }
        public virtual DbSet<OrganizationWebPage> OrganizationWebPages { get; set; }
        public virtual DbSet<OrganizationName> OrganizationNames { get; set; }
        public virtual DbSet<OrganizationDisplayNameType> OrganizationDisplayNameTypes { get; set; }
        public virtual DbSet<OrganizationDescription> OrganizationDescriptions { get; set; }
        public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }
        public virtual DbSet<OrganizationTypeName> OrganizationTypeNames { get; set; }
        public virtual DbSet<OrganizationPhone> OrganizationPhones { get; set; }
        public virtual DbSet<OrganizationService> OrganizationServices { get; set; }
        public virtual DbSet<ServiceCollectionName> ServiceCollectionNames { get; set; }
        public virtual DbSet<ServiceCollectionDescription> ServiceCollectionDescriptions { get; set; }
        public virtual DbSet<ServiceCollectionService> ServiceCollectionServices { get; set; }
        public virtual DbSet<ServiceCollectionServiceChannel> ServiceCollectionServiceChannels { get; set; }
        public virtual DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public virtual DbSet<PhoneNumberTypeName> PhoneNumberTypeNames { get; set; }
        public virtual DbSet<PrintableFormChannelUrlType> PrintableFormChannelUrlTypes { get; set; }
        public virtual DbSet<PrintableFormChannelUrlTypeName> PrintableFormChannelUrlTypeNames { get; set; }
        public virtual DbSet<ProvisionType> ProvisionTypes { get; set; }
        public virtual DbSet<ProvisionTypeName> ProvisionTypeNames { get; set; }
        public virtual DbSet<PublishingStatusType> PublishingStatusTypes { get; set; }
        public virtual DbSet<PublishingStatusTypeName> PublishingStatusTypeNames { get; set; }
        public virtual DbSet<ServiceVersioned> ServicesVersioned { get; set; }
        public virtual DbSet<ServiceClass> ServiceClasses { get; set; }
        public virtual DbSet<ServiceClassName> ServiceClassNames { get; set; }
        public virtual DbSet<ServiceClassDescription> ServiceClassDescriptions { get; set; }
        public virtual DbSet<IndustrialClass> IndustrialClasses { get; set; }
        public virtual DbSet<IndustrialClassName> IndustrialClassNames { get; set; }
        public virtual DbSet<IndustrialClassDescription> IndustrialClassDescription { get; set; }
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
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<ServiceElectronicCommunicationChannel> ServiceElectronicComunicationChannels { get; set; }
        public virtual DbSet<ServiceElectronicNotificationChannel> ServiceElectronicNotificationChannels { get; set; }
        // ReSharper disable once IdentifierTypo
        public virtual DbSet<ServiceHoursAdditionalInformation> ServiceHoursAdditionalInformations { get; set; }
        public virtual DbSet<ServiceKeyword> ServiceKeywords { get; set; }
        public virtual DbSet<StatutoryServiceGeneralDescriptionKeyword> StatutoryServiceGeneralDescriptionKeywords { get; set; }
        public virtual DbSet<ServiceLanguage> ServiceLanguages { get; set; }
        public virtual DbSet<ServiceLaw> ServiceLaws { get; set; }
        public virtual DbSet<ServiceName> ServiceNames { get; set; }
        public virtual DbSet<ServiceLifeEvent> ServiceLifeEvents { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<GeneralDescriptionType> StatutoryServiceGeneralDescriptionTypes { get; set; }
        public virtual DbSet<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
        public virtual DbSet<ServiceRequirement> ServiceRequirements { get; set; }
        public virtual DbSet<ServiceServiceClass> ServiceServiceClasses { get; set; }
        public virtual DbSet<ServiceIndustrialClass> ServiceIndustrialClass { get; set; }
        public virtual DbSet<ServiceTargetGroup> ServiceTargetGroups { get; set; }
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
        //public virtual DbSet<UserOrganization> UserOrganizations { get; set; }
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
        public virtual DbSet<ServiceCollectionLanguageAvailability> ServiceCollectionLanguageAvailabilities { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceChannel> ServiceChannels { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<StatutoryServiceGeneralDescription> StatutoryServiceGeneralDescriptions { get; set; }
        public virtual DbSet<ServiceCollection> ServiceCollections { get; set; }
        public virtual DbSet<PostalCodeName> PostalCodeNames { get; set; }
        public virtual DbSet<AddressCoordinate> Coordinates { get; set; }
        public virtual DbSet<ClsStreetNumberCoordinate> ClsStreetNumberCoordinates { get; set; }
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
        public virtual DbSet<ExtraType> ExtraTypes { get; set; }
        public virtual DbSet<ExtraTypeName> ExtraTypeNames { get; set; }
        public virtual DbSet<ExtraSubType> ExtraSubTypes { get; set; }
        public virtual DbSet<ExtraSubTypeName> ExtraSubTypeNames { get; set; }
        public virtual DbSet<ServiceServiceChannelExtraType> ServiceServiceChannelExtraTypes { get; set; }
        public virtual DbSet<ServiceServiceChannelExtraTypeDescription> ServiceServiceChannelExtraTypeDescriptions { get; set; }
        public virtual DbSet<AccessRightType> AccessRightTypes { get; set; }
        public virtual DbSet<AccessRightName> AccessRightNames { get; set; }
        public virtual DbSet<UserAccessRight> UserAccessRights { get; set; }

        public virtual DbSet<ServiceProducer> ServiceProducers { get; set; }
        public virtual DbSet<ServiceProducerAdditionalInformation> ServiceProducerAdditionalInformation { get; set; }
        public virtual DbSet<ServiceProducerOrganization> ServiceProducerOrganizations { get; set; }
        public virtual DbSet<CFGRequestFilter> CFGRequestFilter { get; set; }
        public virtual DbSet<ServiceHours> ServiceHours { get; set; }
        public virtual DbSet<ServiceServiceChannelServiceHours> ServiceServiceChannelServiceHours { get; set; }
        public virtual DbSet<ServiceServiceChannelAddress> ServiceServiceChannelAddress { get; set; }
        public virtual DbSet<ServiceServiceChannelEmail> ServiceServiceChannelEmail { get; set; }
        public virtual DbSet<ServiceServiceChannelPhone> ServiceServiceChannelPhone { get; set; }
        public virtual DbSet<ServiceServiceChannelWebPage> ServiceServiceChannelWebPage { get; set; }
        public virtual DbSet<SahaOrganizationInformation> SahaOrganizationInformation { get; set; }
        public virtual DbSet<TrackingServiceServiceChannel> ServiceServiceChannelTracking { get; set; }
        public virtual DbSet<TrackingServiceCollectionService> ServiceCollectionServiceTracking { get; set; }
        public virtual DbSet<TrackingServiceCollectionServiceChannel> ServiceCollectionServiceChannelTracking { get; set; }
        public virtual DbSet<TrackingGeneralDescriptionServiceChannel> GeneralDescriptionServiceChannelTracking { get; set; }
        public virtual DbSet<TrackingGeneralDescriptionVersioned> GeneralDescriptionVersionedTracking { get; set; }
        public virtual DbSet<TrackingTranslationOrder> TranslationOrderTracking { get; set; }
        public virtual DbSet<TrackingEntityVersioned> EntityVersionedTracking { get; set; }
        public virtual DbSet<GeneralDescriptionTranslationOrder> GeneralDescriptionTranslationOrders { get; set; }
        public virtual DbSet<ServiceChannelTranslationOrder> ServiceChannelTranslationOrders { get; set; }
        public virtual DbSet<ServiceTranslationOrder> ServiceTranslationOrders { get; set; }
        public virtual DbSet<TranslationCompany> TranslationCompanies { get; set; }
        public virtual DbSet<TranslationOrder> TranslationOrders { get; set; }
        public virtual DbSet<TranslationOrderState> TranslationOrderStates { get; set; }
        public virtual DbSet<TranslationStateType> TranslationStateTypes { get; set; }
        public virtual DbSet<TranslationStateTypeName> TranslationStateTypeNames { get; set; }
        public virtual DbSet<UserAccessRightsGroup> UserAccessRightsGroups { get; set; }
        public virtual DbSet<TasksConfiguration> TasksConfiguration { get; set; }
        public virtual DbSet<UserAccessRightsGroupName> UserAccessRightsGroupNames { get; set; }
        public virtual DbSet<UserOrganization> UserOrganizations { get; set; }

        public virtual DbSet<ServiceChannelAddress> ServiceChannelAddresses { get; set; }
        public virtual DbSet<AddressReceiver> ServiceChannelAddressReceivers { get; set; }

        public virtual DbSet<RestrictionFilter> RestrictionFilters { get; set; }
        public virtual DbSet<OrganizationFilter> OrganizationFilters { get; set; }
        public virtual DbSet<AccessRightsOperationsUI> AccessRightsOperationsUI { get; set; }
        public virtual DbSet<AccessibilityClassificationLevelType> AccessibilityClassificationLevelType { get; set; }
        public virtual DbSet<AccessibilityClassificationLevelTypeName> AccessibilityClassificationLevelTypeName { get; set; }
        public virtual DbSet<WcagLevelType> WcagLevelType { get; set; }
        public virtual DbSet<WcagLevelTypeName> WcagLevelTypeName { get; set; }
        public virtual DbSet<EFMigrationsHistory> EFMigrationsHistory { get; set; }
        public virtual DbSet<VTasksConfiguration> VTasksConfiguration { get; set; }
        public virtual DbSet<VBrokenLink> VBrokenLinks { get; set; }
        
        public virtual DbSet<VLastArchivedServiceCollection> VLastArchivedServiceCollections { get; set; }
        public virtual DbSet<VLastArchivedServiceChannel> VLastArchivedServiceChannels { get; set; }
        public virtual DbSet<VLastArchivedService> VLastArchivedServices { get; set; }
        public virtual DbSet<VLastArchivedOrganization> VLastArchivedOrganizations { get; set; }
        public virtual DbSet<VLastArchivedGeneralDescription> VLastArchivedGeneralDescriptions { get; set; }

        public virtual DbSet<NotificationServiceServiceChannel> NotificationServiceServiceChannel { get; set; }
        public virtual DbSet<NotificationServiceServiceChannelFilter> NotificationServiceServiceChannelFilter { get; set; }

        public virtual DbSet<ClsAddressPoint> ClsAddressPoint { get; set; }
        public virtual DbSet<ClsAddressStreet> ClsAddressStreet { get; set; }
        public virtual DbSet<ClsAddressStreetName> ClsAddressStreetName { get; set; }
        public virtual DbSet<ClsAddressStreetNumber> ClsAddressStreetNumber { get; set; }

        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<HolidayName> HolidayNames { get; set; }
        public virtual DbSet<HolidayDate> HolidayDates { get; set; }
        public virtual DbSet<HolidayServiceHours> HolidayServiceHours { get; set; }
        
        public virtual DbSet<ServiceNumber> ServiceNumbers { get; set; }
        
        public virtual DbSet<JobSummary> JobSummaries { get; set; }
        public virtual DbSet<JobLog> JobLogs { get; set; }
        public virtual DbSet<JobStatusType> JobStatusTypes { get; set; }
        public virtual DbSet<JobStatusTypeName> JobStatusTypeNames { get; set; }
        public virtual DbSet<JobExecutionType> JobExecutionTypes { get; set; }
        public virtual DbSet<JobExecutionTypeName> JobExecutionTypeNames { get; set; }
    }
}
