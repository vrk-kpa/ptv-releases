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


// SOTE has been disabled (SFIPTV-1177)
//
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using PTV.Database.DataAccess.Caches;
//using PTV.Database.DataAccess.Interfaces;
//using PTV.Database.DataAccess.Interfaces.DbContext;
//using PTV.Database.DataAccess.Interfaces.Repositories;
//using PTV.Database.DataAccess.Interfaces.Services;
//using PTV.Database.DataAccess.Interfaces.Services.Security;
//using PTV.Database.DataAccess.Interfaces.Translators;
//using PTV.Database.Model.Models;
//using PTV.Domain.Model.Enums;
//using PTV.Domain.Model.Models;
//using PTV.Framework;
//using PTV.Framework.Logging;
//using Microsoft.Extensions.Logging;
//using PTV.Database.DataAccess.Interfaces.Caches;
//using PTV.Database.Model.Interfaces;
//using PTV.Domain.Model;
//using PTV.Domain.Model.Models.V2.Common;
//using PTV.Domain.Model.Models.V2.Service;
//
//namespace PTV.Database.DataAccess.Services
//{
//    [RegisterService(typeof(ISoteService), RegisterType.Transient)]
//    internal class SoteService : ServiceBase, ISoteService
//    {
//        private readonly ICommonServiceInternal commonService;
//        private readonly IContextManager contextManager;
//        private readonly IPublishingStatusCache publishingStatusCache;
//        private readonly ITranslationViewModel vmToEntityTranslator;
//        private readonly ILogger<SoteService> jobLogger;
//        private readonly ITypesCache typesCache;
//        private readonly ILanguageCache languageCache;
//        private readonly IStreetService streetService;
//
//        private const string DefaultLanguageCode = DomainConstants.DefaultLanguage;
//        private readonly Dictionary<string, Guid> postalCodes;
//        private readonly Dictionary<string, Guid> municipalityCodes;
//
//        public SoteService(
//            ICommonServiceInternal commonService,
//            ITranslationEntity entityToVmTranslator,
//            ITranslationViewModel vmToEntityTranslator,
//            IPublishingStatusCache publishingStatusCache,
//            IUserOrganizationChecker userOrganizationChecker,
//            IContextManager contextManager,
//            IVersioningManager versioningManager,
//            ILogger<SoteService> jobLogger,
//            ICacheManager cacheManager,
//            IStreetService streetService
//        )
//            : base(entityToVmTranslator, vmToEntityTranslator, publishingStatusCache, userOrganizationChecker, versioningManager)
//        {
//            this.commonService = commonService;
//            this.contextManager = contextManager;
//            this.publishingStatusCache = publishingStatusCache;
//            this.vmToEntityTranslator = vmToEntityTranslator;
//            this.jobLogger = jobLogger;
//            this.streetService = streetService;
//            typesCache = cacheManager.TypesCache;
//            languageCache = cacheManager.LanguageCache;
//
//            // load codes
//            postalCodes = LoadPostalCodes();
//            municipalityCodes = LoadMunicipalityByName();
//        }
//
//        public void ImportData(List<VmJsonSoteOrganization> soteOrganizations, VmJobLogEntry logInfo)
//        {
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                var languageId = languageCache.Get(DefaultLanguageCode);
//                var languageAvailabilities = new List<VmLanguageAvailabilityInfo>
//                {
//                    new VmLanguageAvailabilityInfo { CanBePublished = true, LanguageId = languageId }
//                };
//
//                // set organization properties
//                soteOrganizations.ForEach(org =>
//                {
//                    org.Id = null;
//                    var organizationVersioned = GetOrganization(unitOfWork, org.Oid, logInfo);
//                    if (organizationVersioned != null)
//                    {
//                        org.Id = organizationVersioned.Id;
//
//                        // check main organization
//                        if (org.MainOrganizationId != organizationVersioned.ParentId)
//                        {
//                            jobLogger.LogSchedulerWarn(logInfo, $"ParentId has been changed for OrganizationVersionedId: {organizationVersioned.Id}. Previous ParentId: {organizationVersioned.ParentId}");
//                        }
//                    }
//
//                    org.LanguagesAvailabilities = org.Id.IsAssigned() ? null : languageAvailabilities;
//                    FillInAddressInfo(unitOfWork, org.ContactInfo?.Address);
//                });
//
//                // translate organizations
//                var organizationEntities = vmToEntityTranslator.TranslateAll<VmJsonSoteOrganization, OrganizationVersioned>(soteOrganizations, unitOfWork);
//                if (organizationEntities == null)
//                {
//                    jobLogger.LogSchedulerInfo(logInfo, "No organization has been processed.");
//                    return;
//                }
//
//                // handle organization filters (GeneralDescriptionFocTypeOrganizationSoteRelation)
//                commonService.HandleOrganizationSoteFocFilter(unitOfWork, organizationEntities.Select(o => o.UnificRootId).ToList());
//
//                // translate service locations
//                var serviceLocations = soteOrganizations.SelectMany(o => o.ServiceLocations).ToList();
//                serviceLocations.ForEach(sl =>
//                {
//                    sl.Id = null;
//                    sl.LocalizationId = languageId;
//                    var serviceChannelVersioned = GetServiceLocationId(unitOfWork, sl.Oid, logInfo);
//                    if (serviceChannelVersioned != null)
//                    {
//                        sl.Id = serviceChannelVersioned.Id;
//
//                        // check organization id
//                        if (sl.OrganizationId != serviceChannelVersioned.OrganizationId)
//                        {
//                            jobLogger.LogSchedulerWarn(logInfo, $"OrganizationId has been changed for ServiceChannelVersionedId: {serviceChannelVersioned.Id}. Previous OrganizationId: {serviceChannelVersioned.OrganizationId}");
//                        }
//
//                        // check name
//                        var name = serviceChannelVersioned.ServiceChannelNames.FirstOrDefault(n => n.LocalizationId == languageId)?.Name;
//                        if (!string.IsNullOrEmpty(name) && name != sl.Name)
//                        {
//                            sl.AlternateName = name;
//                        }
//                    }
//
//                    sl.LanguagesAvailabilities = sl.Id.IsAssigned() ? null : languageAvailabilities;
//                    FillInAddressInfo(unitOfWork, sl.ContactInfo?.Address);
//                });
//                var serviceLocationEntities = vmToEntityTranslator.TranslateAll<VmJsonSoteServiceLocation, ServiceChannelVersioned>(serviceLocations, unitOfWork);
//
//                // translate services
//                var services = GetServicesForOrganizations(unitOfWork, organizationEntities.Select(o => o.UnificRootId));
//                var serviceEntities = TranslationManagerToEntity.TranslateAll<VmServiceInput, ServiceVersioned>(services, unitOfWork);
//
//                // save all
//                unitOfWork.Save(SaveMode.AllowAnonymous, userName: logInfo.UserName);
//
//                // log created entities
//                jobLogger.LogSchedulerInfo(logInfo, $"{organizationEntities.Count} organization(s) has been processed. Id(s): [{string.Join(", ", organizationEntities.Select(sl => sl.Id))}]");
//                jobLogger.LogSchedulerInfo(logInfo, $"{serviceLocationEntities?.Count} service location(s) has been processed. Id(s): [{string.Join(", ", serviceLocationEntities?.Select(sl => sl.Id))}]");
//                jobLogger.LogSchedulerInfo(logInfo, $"{serviceEntities?.Count} service(s) has been processed. Id(s): [{string.Join(", ", serviceEntities?.Select(sl => sl.Id))}]");
//            });
//        }
//
//        public Dictionary<string, Guid> GetOrganizationIdsByOid(VmJobLogEntry logInfo)
//        {
//            var result = new Dictionary<string, Guid>();
//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                var allowedStatuses = new List<Guid> {publishingStatusCache.Get(PublishingStatus.Published) /*, publishingStatusCache.Get(PublishingStatus.Modified)*/, publishingStatusCache.Get(PublishingStatus.Draft)};
//                var list = unitOfWork.CreateRepository<IOrganizationVersionedRepository>()
//                    .All()
//                    .Where(o => allowedStatuses.Contains(o.PublishingStatusId) && o.Oid != null && o.Oid != string.Empty)
//                    .Select(o => new {o.Oid, o.UnificRootId, o.PublishingStatusId, o.PublishingStatus.PriorityFallback})
//                    .Distinct()
//                    .ToList();
//
//                // check, that oid is unique (one oid could be used for just one published/draft unificRootId)
//                // log not valid oids and skip them
//                var groupByOid = list
//                    .GroupBy(ov => ov.Oid, ov => ov.UnificRootId)
//                    .ToDictionary(k => k.Key, v => v.Select(x => x).Distinct());
//
//                var moreUnificRootIdsForOneOid = groupByOid
//                    .Select(grp => new {oid = grp.Key, Cnt = grp.Value.Distinct().Count()})
//                    .Where(x => x.Cnt > 1)
//                    .Select(x => x.oid)
//                    .ToList();
//
//                if (moreUnificRootIdsForOneOid.Any())
//                {
//                    list.RemoveAll(o => moreUnificRootIdsForOneOid.Contains(o.Oid));
//                    moreUnificRootIdsForOneOid.ForEach(oid =>
//                        jobLogger.LogSchedulerError(logInfo, $"Oid '{oid}' multiple usage! Oid is used for organizations with UnificRootIds: [{string.Join(", ", groupByOid[oid])}]", null));
//                }
//
//                result = list
//                    .GroupBy(k => k.Oid, v => v.UnificRootId)
//                    .ToDictionary(k => k.Key, v => v.Select(x => x).Distinct().Single());
//            });
//
//            return result;
//        }
//
//        private OrganizationVersioned GetOrganization(IUnitOfWork unitOfWork, string oid, VmJobLogEntry logInfo)
//        {
//            var organizations = unitOfWork.CreateRepository<IOrganizationVersionedRepository>()
//                .All()
//                .Where(o => o.Oid != null && o.Oid.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
//                .ToDictionary(o => o.Id);
//
//            // no organization with the oid
//            if (organizations.IsNullOrEmpty()) return null;
//
//            // check, that unificRootId is unique for given oid
//            var uniqueUnificRoots = organizations.Values.Select(o => o.UnificRootId).Distinct();
//            if (uniqueUnificRoots.Count() > 1)
//            {
//                jobLogger.LogSchedulerError(logInfo, $"Oid '{oid}' multiple usage! Oid is used for organizations with UnificRootIds: [{string.Join(", ", organizations.Values.Select(o => o.UnificRootId).Distinct())}]", null);
//                return null;
//            }
//
//            // get organization version
//            var organizationVersion = VersioningManager.GetLastPublishedModifiedDraftVersion<OrganizationVersioned>(unitOfWork, organizations.First().Value.UnificRootId);
//            return organizationVersion == null
//                ? null
//                : organizations[organizationVersion.EntityId];
//        }
//
//        private ServiceChannelVersioned GetServiceLocationId(IUnitOfWork unitOfWork, string oid, VmJobLogEntry logInfo)
//        {
//            if (oid.IsNullOrEmpty()) return null;
//
//            var serviceLocations = unitOfWork.CreateRepository<IServiceChannelSocialHealthCenterRepository>()
//                .All()
//                .Where(s => s.Oid.Equals(oid, StringComparison.InvariantCultureIgnoreCase))
//                .Distinct()
//                .Select(s => s.ServiceChannelId)
//                .ToList();
//
//            // no service location with the oid
//            if (serviceLocations.IsNullOrEmpty()) return null;
//
//            // check, that oid is unique
//            if (serviceLocations.Count() > 1)
//            {
//                jobLogger.LogSchedulerError(logInfo, $"Oid '{oid}' multiple usage! Oid is used for service channels with UnificRootIds: [{string.Join(", ", serviceLocations)}]", null);
//                return null;
//            }
//
//            // get service location version
//            var slVersion = VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceChannelVersioned>(unitOfWork, serviceLocations.First());
//            if (slVersion == null) return null;
//
//            return unitOfWork.CreateRepository<IServiceChannelVersionedRepository>()
//                .All()
//                .Include(i => i.ServiceChannelNames)
//                .FirstOrDefault(s => s.Id == slVersion.EntityId);
//        }
//
//        private IEnumerable<VmServiceInput> GetServicesForOrganizations(IUnitOfWork unitOfWork, IEnumerable<Guid> organizations)
//        {
//            var allowedTypes = new List<Guid>
//            {
//                typesCache.Get<GeneralDescriptionType>(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct.ToString()),
//                typesCache.Get<GeneralDescriptionType>(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote.ToString())
//            };
//
//            var gdRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
//            var generalDescriptions = unitOfWork.ApplyIncludes(
//                    gdRepository
//                        .All()
//                        .Where(gd => allowedTypes.Contains(gd.GeneralDescriptionTypeId) && gd.PublishingStatusId == PublishingStatusCache.Get(PublishingStatus.Published)),
//                    i => i.Include(x => x.PublishingStatus)
//                        .Include(x => x.TargetGroups)
//                        .Include(x => x.Names)
//                        .Include(x => x.LanguageAvailabilities))
//                .ToList();
//
//            var result = new List<VmServiceInput>();
//            generalDescriptions.ForEach(generalDescription =>
//            {
//                var publishedLanguagesIds = generalDescription.LanguageAvailabilities.Where(x => x.StatusId == PublishingStatusCache.Get(PublishingStatus.Published)).Select(x => x.LanguageId).ToList();
//                generalDescription.Names = generalDescription.Names.Where(x => publishedLanguagesIds.Contains(x.LocalizationId)).ToList();
//                var header = TranslationManagerToVm.Translate<IBaseInformation, VmEntityHeaderBase>(generalDescription);
//
//                organizations.ForEach(organizationId =>
//                {
//                    result.Add(new VmServiceInput
//                    {
//                        Organization = organizationId,
//                        LanguagesAvailabilities = header.LanguagesAvailabilities,
//                        Name = header.Name,
//                        GeneralDescriptionId = generalDescription.UnificRootId,
//                        GeneralDescriptionServiceTypeId = generalDescription.TypeId,
//                        GeneralDescriptionChargeTypeId = generalDescription.ChargeTypeId,
//                        GeneralDescriptionTargetGroups = generalDescription.TargetGroups?.Select(x => x.TargetGroupId).ToList(),
//                        GeneralDescriptionName = header.Name,
//                        AreaInformation = new VmAreaInformation {AreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString())},
//                        Languages = generalDescription.LanguageAvailabilities.Select(x => x.LanguageId).ToList(),
//                        Laws = new List<VmLaw>(),
//                        ServiceProducers = new List<VmServiceProducer>(),
//                        ServiceVoucherInUse = false,
//                        ServiceVouchers = new Dictionary<string, List<VmServiceVoucher>>(),
//                        TargetGroups = new List<Guid>(),
//                        Keywords = new Dictionary<string, List<Guid>>()
//                    });
//                });
//            });
//
//            return result;
//        }
//
//        private Dictionary<string, Guid> LoadPostalCodes()
//        {
//            return contextManager.ExecuteReader(unitOfWork =>
//            {
//                var postalCodeRep = unitOfWork.CreateRepository<IPostalCodeRepository>();
//                var result = postalCodeRep.All().Where(pc => pc.IsValid).Select(pc => new {pc.Id, pc.Code});
//                return result.ToDictionary(key => key.Code, value => value.Id);
//            });
//       }
//
//        private Dictionary<string, Guid> LoadMunicipalityByName()
//        {
//            return contextManager.ExecuteReader(unitOfWork =>
//            {
//                var municipalityNameRep = unitOfWork.CreateRepository<IMunicipalityNameRepository>();
//                return municipalityNameRep
//                    .All()
//                    .Where(m => m.LocalizationId == languageCache.Get(DefaultLanguageCode))
//                    .Select(m => new {m.Name, m.MunicipalityId})
//                    .GroupBy(x => new {x.Name, x.MunicipalityId})
//                    .ToDictionary(k => k.Key.Name, v => v.Key.MunicipalityId);
//            });
//        }
//
//        private void FillInAddressInfo(IUnitOfWork unitOfWork, VmJsonSoteAddress address)
//        {
//            if (address == null) return;
//
//            address.PostalCodeId = postalCodes.ContainsKey(address.PostalCode)
//                ? postalCodes[address.PostalCode]
//                : (Guid?) null;
//
//            address.MunicipalityId = !address.MunicipalityName.IsNullOrEmpty() && municipalityCodes.ContainsKey(address.MunicipalityName)
//                ? municipalityCodes[address.MunicipalityName]
//                : (Guid?) null;
//
//            var streetName = address.StreetAddress?.StreetName;
//            if (!string.IsNullOrWhiteSpace(streetName))
//            {
//                var streetNameRepo = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
//                var streetNameEntity = streetNameRepo.All()
//                    .Include(sn => sn.ClsAddressStreet)
//                    .ThenInclude(s => s.StreetNumbers)
//                    .FirstOrDefault(sn => sn.Name == streetName
//                                 && sn.ClsAddressStreet.MunicipalityId == address.MunicipalityId);
//                address.StreetAddress.StreetId = streetNameEntity?.ClsAddressStreetId;
//
//                var streetRangeId = streetService.GetStreetNumberRangeId(unitOfWork,
//                    address.StreetAddress?.StreetNumber, streetNameEntity?.ClsAddressStreetId ?? Guid.Empty);
//                address.StreetAddress.StreetRangeId = streetRangeId;
//            }
//        }
//    }
//}
