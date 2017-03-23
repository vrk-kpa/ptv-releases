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
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICommonService), RegisterType.Transient)]
    [RegisterService(typeof(ICommonServiceInternal), RegisterType.Transient)]
    internal class CommonService : ServiceBase, ICommonService, ICommonServiceInternal
    {
        private static readonly List<string> TranslationLanguageCodes = new List<string>() { LanguageCode.fi.ToString(), LanguageCode.sv.ToString(), LanguageCode.en.ToString() };
        private static readonly List<string> SelectedPublishingStatuses = new List<string>() { PublishingStatus.Draft.ToString(), PublishingStatus.Published.ToString(), PublishingStatus.Modified.ToString() };

        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IDataServiceFetcher dataServiceFetcher;
        private readonly ServiceUtilities utilities;
        private readonly IVersioningManager versioningManager;

        public CommonService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IContextManager contextManager,
            ITypesCache typesCache,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IDataServiceFetcher dataServiceFetcher,
            ServiceUtilities utilities,
            IVersioningManager versioningManager)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.dataServiceFetcher = dataServiceFetcher;
            this.utilities = utilities;
            this.versioningManager = versioningManager;
        }

        public IVmGetFrontPageSearch GetFrontPageSearch()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                var userOrganization = utilities.GetUserOrganization(unitOfWork);
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClasses = TranslationManagerToVm.TranslateAll<ServiceClass, VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork,serviceClassesRep.All().OrderBy(x => x.Label)));
                var targetGroups = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label))), x => x.Code);
                var result = new VmGetFrontPageSearch
                {
                    OrganizationId = userOrganization
                };
                var publishingStatuses = GetPublishingStatuses();
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", serviceClasses),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses),
                    () => GetEnumEntityCollectionModel("ServiceTypes", GetServiceTypes()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("PhoneNumberTypes", GetPhoneTypes()),
                    () => GetEnumEntityCollectionModel("ChannelTypes", GetServiceChannelTypes()),
                    () => GetEnumEntityCollectionModel("TargetGroups", targetGroups));
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => SelectedPublishingStatuses.Contains(x.Code)).Select(x => x.Id).ToList();
                return result;
            });
        }

        public IVmBase GetTypedData(IEnumerable<string> dataTypes)
        {
            return new VmListItemsData<IVmBase>(dataServiceFetcher.Fetch(dataTypes));
        }

        public VmListItemsData<VmListItem> GetPhoneChargeTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChargeType>();
        }

        public VmListItemsData<VmListItem> GetWebPageTypes()
        {
            return dataServiceFetcher.FetchType<WebPageType>();
        }

        public VmListItemsData<VmListItem> GetServiceTypes()
        {
            return dataServiceFetcher.FetchType<ServiceType>();
        }

        public VmListItemsData<VmListItem> GetProvisionTypes()
        {
            return dataServiceFetcher.FetchType<ProvisionType>();
        }

        public VmListItemsData<VmListItem> GetServiceCoverageTypes()
        {
            return dataServiceFetcher.FetchType<ServiceCoverageType>();
        }

        public VmListItemsData<VmListItem> GetPrintableFormUrlTypes()
        {
            return dataServiceFetcher.FetchType<PrintableFormChannelUrlType>();
        }

        public VmListItemsData<VmListItem> GetPhoneTypes()
        {
            return dataServiceFetcher.FetchType<PhoneNumberType>();
        }

        public VmListItemsData<VmListItem> GetServiceHourTypes()
        {
            return dataServiceFetcher.FetchType<ServiceHourType>();
        }

        public VmListItemsData<VmListItem> GetPublishingStatuses()
        {
            return new VmListItemsData<VmListItem>(dataServiceFetcher.FetchType<PublishingStatusType>().Select(i => new VmPublishingStatus()
            {
                Id = i.Id,
                Name = i.Name,
                Code = i.Code,
                OrderNumber = i.OrderNumber,
                Type = i.Code.Parse<PublishingStatus>()
            }));
        }

        public VmListItemsData<VmListItem> GetCoordinateTypes()
        {
            return dataServiceFetcher.FetchType<CoordinateType>();
        }

        public IReadOnlyList<VmListItem> GetOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true)
        {
			// get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var resultTemp = organizationRepository.All().Where(x => x.PublishingStatusId == psPublished);

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(n => n.Name.ToLower().Contains(searchText) && n.TypeId == x.DisplayNameTypeId));
            }

            if (!takeAll)
            {
                resultTemp = resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);
            }
            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.OrganizationNames).ThenInclude(i => i.Localization));
            return TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmListItem>(resultTemp);
        }

        public IReadOnlyList<VmLaw> GetLaws(IUnitOfWork unitOfWork, List<Guid> takeIds)
        {
            var lawRep = unitOfWork.CreateRepository<ILawRepository>();
            var resultTemp = lawRep.All()
                .Where(x => takeIds.Contains(x.Id))
                .Include(x => x.Names)
                .Include(x => x.WebPages).ThenInclude(w => w.WebPage);
            return TranslationManagerToVm.TranslateAll<Law, VmLaw>(resultTemp);
        }

        public List<VmTreeItem> GetOrganizations(IUnitOfWork unitOfWork)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var organizations = unitOfWork.ApplyIncludes(organizationRep.All().Where(x => x.PublishingStatusId == psPublished), query => query.Include(organization => organization.OrganizationNames));
            return CreateTree<VmTreeItem>(LoadOrganizationTree(organizations, 1));
        }

        public VmListItemsData<VmListItem> GetLanguages()
        {
            return dataServiceFetcher.FetchType<Language>();
        }

        public IReadOnlyList<VmListItem> GetTranslationLanguages()
        {
            return GetLanguages().Where(x => TranslationLanguageCodes.Contains(x.Code)).ToList();
        }

        public VmListItemsData<VmListItem> GetServiceChannelTypes()
        {
            return dataServiceFetcher.FetchType<ServiceChannelType>();
        }

        public IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var municipalities = unitOfWork.ApplyIncludes(municipalityRep.All(), i => i.Include(j => j.MunicipalityNames));
            return TranslationManagerToVm.TranslateAll<Municipality, VmListItem>(municipalities).OrderBy(x => x.Name).ToList();
        }

        public IReadOnlyList<VmListItem> GetOrganizationNamesWithoutSetOfOrganizations(IUnitOfWork unitOfWork, IList<Guid?> organizationSet)
        {
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var organizationNameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            return
                TranslationManagerToVm.TranslateAll<OrganizationName, VmListItem>(
                    organizationNameRep.All().Where(x => !organizationSet.Contains(x.OrganizationVersionedId))
                    .Where(x => x.OrganizationVersioned.PublishingStatusId == psDraft || x.OrganizationVersioned.PublishingStatusId == psPublished)
                    .Where(x => x.TypeId == x.OrganizationVersioned.DisplayNameTypeId))
                    .OrderBy(x => x.Name, StringComparer.CurrentCulture).ToList();
        }

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="organizationGuid"></param>
        public void MapUserToOrganization(Guid userId, string userName, Guid? organizationGuid)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (organizationGuid.HasValue)
                {
                    var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                    var connectedUser = connectionRep.All().FirstOrDefault(u => u.UserId == userId);

                    // we blindly trust that the organizationid guid is valid (so did the original code too, see file history)

                    if (connectedUser != null)
                    {
                        connectedUser.OrganizationId = organizationGuid.Value;
                        connectedUser.UserName = userName;
                    }
                    else
                    {
                        connectionRep.Add(new UserOrganization() { UserId = userId, OrganizationId = organizationGuid.Value, UserName = userName});
                    }

                    unitOfWork.Save();
                }
            });
        }

        public List<SelectListItem> GetOrganizationByUser(string userName)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                IQueryable<OrganizationVersioned> organizations = unitOfWork.CreateRepository<IOrganizationVersionedRepository>().All().Where(x => x.PublishingStatus.Code == PublishingStatus.Draft.ToString() || x.PublishingStatus.Code == PublishingStatus.Published.ToString()).Include(i => i.OrganizationNames).ThenInclude(i => i.Localization);
                var user = !string.IsNullOrEmpty(userName) ? userOrgRep.All().FirstOrDefault(i => i.UserName.ToLower() == userName.ToLower()) : null;

                // seems that the method is supposed to return all organizations if the username is null (see authentication server calling this method)
                // added condition here to shortcut and return empty list if we have user but no mapping to organization
                if ((user == null && !string.IsNullOrEmpty(userName)) || (user != null && !user.OrganizationId.HasValue))
                {
                    return new List<SelectListItem>();
                }

                if (user != null)
                {
                    organizations = organizations.Where(i => i.UnificRootId == user.OrganizationId.Value);
                }

                return organizations.ToList().Select(organizaton => new SelectListItem() { Value = organizaton.UnificRootId.ToString(), Text = organizaton.OrganizationNames.FirstOrDefault(j => j.TypeId == organizaton.DisplayNameTypeId && j.Localization.Code == LanguageCode.fi.ToString())?.Name}).OrderBy(x => x.Text).ToList();
            });
        }

        public List<Guid> GetCoUsersOfUser(string userName)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var userOrgid = userOrgRep.All().Where(i => i.UserName.ToLower() == userName.ToLower()).Select(i => i.OrganizationId).FirstOrDefault();
                if (!userOrgid.HasValue) return new List<Guid>();
                return userOrgRep.All().Where(i => i.OrganizationId == userOrgid).Select(i => i.UserId).ToList();
            });
        }

        public List<Guid> GetCoUsersOfUser(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var userOrgid = userOrgRep.All().Where(i => i.UserId == userId).Select(i => i.OrganizationId).FirstOrDefault();
                if (!userOrgid.HasValue) return new List<Guid>();
                return userOrgRep.All().Where(i => i.OrganizationId == userOrgid).Select(i => i.UserId).ToList();
            });
        }

        public Guid GetDraftStatusId()
        {
            return typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
        }

        public bool IsUserAssignedToOrganization(string userName)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                return userOrgRep.All().Any(i => i.UserName.ToLower() == userName.ToLower() && (i.OrganizationId != null));
            });
        }

        public string GetLozalizadion(Guid? languageId)
        {
            if (languageId.HasValue)
            {
                return typesCache.GetByValue<Language>(languageId.Value);
            }
            return LanguageCode.fi.ToString();
        }

        public PublishingResult PublishEntity<TEntity, TLanguageAvail>(VmPublishingModel model) where TEntity  : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability
        {
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            if (model.LanguagesAvailabilities.Select(i => i.StatusId).All(i => i != psPublished))
            {
                throw new PublishLanguageException();
            }
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = serviceRep.All().Single(x => x.Id == model.Id);
                var targetStatus = (entity.PublishingStatusId == psOldPublished || entity.PublishingStatusId == psDeleted) ? PublishingStatus.Modified : PublishingStatus.Published;
                var affected = versioningManager.PublishVersion(unitOfWork, entity, targetStatus);
                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguageAvail>(unitOfWork, entity, model.LanguagesAvailabilities);
                unitOfWork.Save();
                var processedEntityResults = affected.First(i => i.Id == model.Id);
                return new PublishingResult()
                {
                    AffectedEntities = affected,
                    Id = processedEntityResults.Id,
                    PublishingStatusOld = processedEntityResults.PublishingStatusOld,
                    PublishingStatusNew = processedEntityResults.PublishingStatusNew,
                    Version = new VmVersion() {  Major = entity.Versioning.VersionMajor, Minor = entity.Versioning.VersionMinor }
                };
            });
        }

        public void ExtendPublishingStatusesDeletedOldPublished(IList<Guid> statuses)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            if (statuses.Contains(psDeleted) && !statuses.Contains(psOldPublished)) statuses.Add(psOldPublished);
            if (statuses.Contains(psOldPublished) && !statuses.Contains(psDeleted)) statuses.Add(psDeleted);
        }

        public PublishingResult PublishAllAvailableLanguageVersions<TEntity, TLanguageAvail>(Guid Id, Expression<Func<TLanguageAvail, bool>> getSelectedIdFunc) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability
        {
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var vmLanguages = new List<VmLanguageAvailabilityInfo>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IRepository<TLanguageAvail>>();
                var languages = repository.All().Where(getSelectedIdFunc).Select(i => i.LanguageId).ToList();
                languages.ForEach(l => vmLanguages.Add(new VmLanguageAvailabilityInfo() { LanguageId = l, StatusId = psPublished }));
            });

            return PublishEntity<TEntity, TLanguageAvail>(new VmPublishingModel
            {
                Id = Id,
                LanguagesAvailabilities = vmLanguages
            });
        }

        public IList<PublishingAffectedResult> RestoreArchivedEntity<TEntity>(IUnitOfWorkWritable unitOfWork, Guid versionId) where TEntity : class, IEntityIdentifier, IVersionedVolume, new()
        {
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entity = rep.All().Single(x => x.Id == versionId);
            
            return versioningManager.PublishVersion(unitOfWork, entity, PublishingStatus.Modified);
        }
    }
}
