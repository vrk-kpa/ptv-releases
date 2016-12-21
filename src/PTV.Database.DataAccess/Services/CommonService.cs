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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Framework.Extensions;
using IMapServiceProvider = PTV.Framework.Interfaces.IMapServiceProvider;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICommonService), RegisterType.Transient)]
    internal class CommonService : ServiceBase, ICommonService
    {
        private IContextManager contextManager;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private List<string> translationLanguageCodes = new List<string>() { LanguageCode.fi.ToString(), LanguageCode.sv.ToString(), LanguageCode.en.ToString() };

        public CommonService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IContextManager contextManager,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            IPublishingStatusCache publishingStatusCache) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
        }

        public List<VmSelectableItem> GetPhoneChargeTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ServiceChargeType, VmSelectableItem>(unitOfWork);
        }

        public List<VmListItem> GetWebPageTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<WebPageType, VmListItem>(unitOfWork);
        }

        public List<VmSelectableItem> GetServiceTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ServiceType, VmSelectableItem>(unitOfWork);
        }

        public List<VmListItem> GetProvisionTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ProvisionType, VmListItem>(unitOfWork);
        }

        public List<VmListItem> GetServiceCoverageTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ServiceCoverageType, VmListItem>(unitOfWork);
        }

        public List<VmListItem> GetPrintableFormUrlTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<PrintableFormChannelUrlType, VmListItem>(unitOfWork);
        }

        public List<VmSelectableItem> GetPhoneTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<PhoneNumberType, VmSelectableItem>(unitOfWork);
        }

        public List<VmSelectableItem> GetServiceHourTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ServiceHourType, VmSelectableItem>(unitOfWork);
        }

        public VmListItemsData<VmListItem> GetPublishingStatuses()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return new VmListItemsData<VmListItem>(GetPublishingStatuses(unitOfWork)); 
            });
        }

        public List<VmPublishingStatus> GetPublishingStatuses(IUnitOfWork unitOfWork)
        {
            var repository = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>();
            return TranslationManagerToVm.TranslateAll<PublishingStatusType, VmPublishingStatus>(repository.All().OrderBy(x => x.OrderNumber)).ToList();
        }

        internal List<TModel> GetTypes<TEntity, TModel>(IUnitOfWork unitOfWork) where TEntity : TypeBase
            where TModel : class
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            return
                TranslationManagerToVm.TranslateAll<TypeBase, TModel>(repository.All().OrderBy(x => x.OrderNumber))
                    .ToList();
        }

        public IReadOnlyList<VmListItem> GetOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true)
        {
			// get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var serviceNameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            var nameCode = NameTypeEnum.Name.ToString();
            var deletedPublishingStatus = PublishingStatus.Deleted.ToString();

            var resultTemp = serviceNameRep.All()
                .Where(x => x.Organization.PublishingStatus.Code != deletedPublishingStatus)
                .Where(x => x.TypeId == x.Organization.DisplayNameTypeId);

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                resultTemp = resultTemp.Where(x => x.Name.ToLower().Contains(searchText));
            }

            resultTemp = resultTemp.OrderBy(x => x.Name);

            if (!takeAll)
            {
                resultTemp = resultTemp.Take(CoreConstants.MaximumNumberOfAllItems);
            }

            return TranslationManagerToVm.TranslateAll<OrganizationName, VmListItem>(resultTemp.ToList());
        }

        public List<VmTreeItem> GetOrganizations(IUnitOfWork unitOfWork)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var deletedStatus = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var organizations = unitOfWork
                .ApplyIncludes(organizationRep.All().Where(x => x.PublishingStatusId != deletedStatus), query =>
                query.Include(organization => organization.OrganizationNames).ThenInclude(name => name.Type)
                .Include(organization => organization.OrganizationNames)
                .Include(organization => organization.Children).ThenInclude(organization => organization.Children));
            return CreateTree<VmTreeItem>(LoadFintoTree(organizations));
        }

        public IReadOnlyList<VmListItem> GetLanguages(IUnitOfWork unitOfWork)
        {
            var languageRep = unitOfWork.CreateRepository<ILanguageRepository>();
            var languages = unitOfWork.ApplyIncludes(languageRep.All(), i => i.Include(j => j.LanguageNames));
            return TranslationManagerToVm.TranslateAll<Language, VmListItem>(languages).OrderBy(i => i.Name).ToList();
        }

        public IReadOnlyList<VmListItem> GetTranslationLanguages(IUnitOfWork unitOfWork)
        {
            var languageRep = unitOfWork.CreateRepository<ILanguageRepository>();
            var languages = unitOfWork.ApplyIncludes(languageRep.All().Where(x=> translationLanguageCodes.Contains(x.Code)), i => i.Include(j => j.LanguageNames));
            return TranslationManagerToVm.TranslateAll<Language, VmListItem>(languages).OrderBy(i => i.Name).ToList();
        }

        public List<VmSelectableItem> GetServiceChannelTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<ServiceChannelType, VmSelectableItem>(unitOfWork);
        }


        public IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            return TranslationManagerToVm.TranslateAll<Municipality, VmListItem>(municipalityRep.All().OrderBy(x => x.Name)).ToList();
        }


        public IReadOnlyList<VmListItem> GetOrganizationNamesWithoutSetOfOrganizations(IUnitOfWork unitOfWork, IList<Guid?> organizationSet)
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            var nameCode = NameTypeEnum.Name.ToString();
            var organizationNameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            return
                TranslationManagerToVm.TranslateAll<OrganizationName, VmListItem>(
                    organizationNameRep.All().Where(x => !organizationSet.Contains(x.OrganizationId))
                    .Where(x => x.Organization.PublishingStatus.Code != PublishingStatus.Deleted.ToString())
                    .Where(x => x.TypeId == x.Organization.DisplayNameTypeId))
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
                    var orgRepRelId = unitOfWork.CreateRepository<IOrganizationRepository>().All().First(x => x.Id == organizationGuid).Oid;

                    var connectedUser = connectionRep.All().FirstOrDefault(u => u.UserId == userId);

                    if (connectedUser != null)
                    {
                        connectedUser.RelationId = orgRepRelId;
                    }
                    else
                    {
                        connectionRep.Add(new UserOrganization() { UserId = userId, RelationId = orgRepRelId, UserName = userName});
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
                IQueryable<Organization> organizations = unitOfWork.CreateRepository<IOrganizationRepository>().All().Include(i => i.OrganizationNames).ThenInclude(i => i.Localization);
                var user = !string.IsNullOrEmpty(userName) ? userOrgRep.All().FirstOrDefault(i => i.UserName.ToLower() == userName.ToLower()) : null;
                if (user == null && !string.IsNullOrEmpty(userName)) return new List<SelectListItem>();
                if (user != null)
                {
                    organizations = organizations.Where(i => i.Oid == user.RelationId);
                }
                return organizations.ToList().Select(organizaton => new SelectListItem() { Value = organizaton.Id.ToString(),  Text = organizaton.OrganizationNames.FirstOrDefault(j => j.TypeId == organizaton.DisplayNameTypeId && j.Localization.Code == LanguageCode.fi.ToString())?.Name}).OrderBy(x => x.Text).ToList();
            });
        }

        public List<Guid> GetCoUsersOfUser(string userName)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var oid = userOrgRep.All().Where(i => i.UserName.ToLower() == userName.ToLower()).Select(i => i.RelationId).FirstOrDefault();
                if (string.IsNullOrEmpty(oid)) return new List<Guid>();
                return userOrgRep.All().Where(i => i.RelationId == oid).Select(i => i.UserId).ToList();
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
                return userOrgRep.All().Any(i => i.UserName.ToLower() == userName.ToLower() && (i.RelationId != null));
            });
        }

        public string GetLozalizadion(Guid? languageId)
        {
            if (languageId.HasValue)
            {
                return languageCache.GetByValue(languageId.Value);
            }
            return LanguageCode.fi.ToString();
        }

        public List<VmListItem> GetOrganizationTypes(IUnitOfWork unitOfWork)
        {
            return GetTypes<OrganizationType, VmListItem>(unitOfWork);
        }
    }
}
