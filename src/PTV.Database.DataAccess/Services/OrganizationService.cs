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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;

using System.Net;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models;
using System.Linq.Expressions;
using PTV.Domain.Logic.Channels;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Caches;
using Microsoft.AspNetCore.Http;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IOrganizationService), RegisterType.Transient)]
    public class OrganizationService : ServiceBase, IOrganizationService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private ICommonService commonService;
        private OrganizationLogic organizationLogic;
        private ITypesCache typesCache;
        private IAddressService addressService;

        public OrganizationService(IContextManager contextManager,
                                   ITranslationEntity translationEntToVm,
                                   ITranslationViewModel translationVmtoEnt,
                                   ILogger<OrganizationService> logger,
                                   OrganizationLogic organizationLogic,
                                   ServiceUtilities utilities,
                                   DataUtils dataUtils,
                                   ICommonService commonService,
                                   IHttpContextAccessor ctxAccessor,
                                   IAddressService addressService,
                                   IPublishingStatusCache publishingStatusCache) : base(translationEntToVm, translationVmtoEnt, publishingStatusCache)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.organizationLogic = organizationLogic;
            this.dataUtils = dataUtils;
            this.addressService = addressService;

            // ITypesCache cannot be injected in constructor because it uses internal access modifier
            // get the typescache from requestservices (IServiceProvider) basically using Locator pattern here
            typesCache = ctxAccessor.HttpContext.RequestServices.GetService(typeof(ITypesCache)) as ITypesCache;
        }

        public void TestMethod()
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                //var rep = unitOfWork.CreateRepository<IOrganizationRepository>();
                //var all = rep.All();
                //var res = translationEntToVm.TranslateAll<IVmOrganization>(all);
            });
        }

        public IVmOpenApiGuidPage GetOrganizationIds(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var pagingVm = new VmOpenApiGuidPage(pageNumber, pageSize);

            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                var query = organizationRep.All()
                    .Where(PublishedFilter<Organization>()).Where(ValidityFilter<Organization>());
                if (date.HasValue)
                {
                    query = query.Where(o => o.Modified > date.Value);
                }

                pagingVm.SetPageCount(query.Count());
                if (pagingVm.IsValidPageNumber())
                {
                    if (pagingVm.PageCount > 1)
                    {
                        query = query.OrderBy(o => o.Created).Skip(pagingVm.GetSkipSize()).Take(pagingVm.GetTakeSize());
                    }
                    pagingVm.GuidList = query.ToList().Select(o => o.Id).ToList();
                }
            });
            return pagingVm;
        }

        public IVmOpenApiOrganization GetOrganization(Guid id, bool getOnlyPublished = true)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiOrganization, VmOpenApiOrganization>(V2GetOrganization(id, getOnlyPublished) as V2VmOpenApiOrganization);
        }

        public IV2VmOpenApiOrganization V2GetOrganization(Guid id, bool getOnlyPublished = true)
        {
            try
            {
                Expression<Func<Organization, bool>> filter = o => o.Id.Equals(id);
                return GetOrganizationsWithDetails(filter, getOnlyPublished).FirstOrDefault();
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public IList<VmOpenApiOrganization> GetOrganizationsByBusinessCode(string code)
        {
            return TranslationManagerToVm.TranslateAll<V2VmOpenApiOrganization, VmOpenApiOrganization>(V2GetOrganizationsByBusinessCode(code)).ToList();
        }

        public IList<V2VmOpenApiOrganization> V2GetOrganizationsByBusinessCode(string code)
        {
            try
            {
                //Expression<Func<Organization, bool>> filter = o => o.Business.Code != null && o.Business.Code.Equals(code);
                //return GetOrganizationsWithDetails(filter);

                // Performance fix, replace above code. Locally the above code executed 1300ms and now in ~150ms (excluding the first call "warm up")
                // in training env it took 9 to 10 seconds
                // the query is very slow when using navigation property to filter
                // first get list of organization ids and use those to fetch the information

                IList<Guid> orgIds = GetOrganizationIds(code);

                if (orgIds == null || orgIds.Count == 0)
                {
                    return new List<V2VmOpenApiOrganization>();
                }

                Expression<Func<Organization, bool>> filter = o => orgIds.Contains(o.Id);
                return GetOrganizationsWithDetails(filter);
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with code {0}. {1}", code, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiOrganization GetOrganizationByOid(string oid)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiOrganization, VmOpenApiOrganization>(V2GetOrganizationByOid(oid) as V2VmOpenApiOrganization);
        }

        public IV2VmOpenApiOrganization V2GetOrganizationByOid(string oid)
        {
            try
            {
                Expression<Func<Organization, bool>> filter = o => o.Oid.Equals(oid);
                return GetOrganizationsWithDetails(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with Oid {0}. {1}", oid, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        /// <summary>
        /// Gets a list of organization ids matching the businessId parameter (same business id can be used by multiple organizations).
        /// </summary>
        /// <param name="businessId">business id (Y-tunnus)</param>
        /// <returns>List of matching organization ids or an empty list if there were not matches</returns>
        public IList<Guid> GetOrganizationIds(string businessId)
        {
            if (string.IsNullOrWhiteSpace(businessId))
            {
                return new List<Guid>();
            }

            List<Guid> orgIds = new List<Guid>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var bidRepo = unitOfWork.CreateRepository<IBusinessRepository>();
                var businessIds = bidRepo.All().Where(bid => bid.Code != null && bid.Code.Equals(businessId)).Select(b => b.Id).ToList();

                if (businessIds.Count > 0)
                {
                    var orgRepo = unitOfWork.CreateRepository<IOrganizationRepository>();
                    // the contains is evaluated in client because EF doesn't currently handle the nullable type
                    // https://github.com/aspnet/EntityFramework/issues/4114 (closed, we use this same solution .HasValue but still evaluated locally)
                    // https://github.com/aspnet/EntityFramework/issues/4247 relates to the previous and is currently labeled as enhancement
                    orgIds = orgRepo.All().Where(o => o.BusinessId.HasValue && businessIds.Contains(o.BusinessId.Value)).Select(o => o.Id).ToList();
                }
            });

            return orgIds;
        }

        public Guid GetOrganizationIdByOid(string oid)
        {
            var guid = Guid.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                var org = orgRep.All().FirstOrDefault(o => o.Oid.Equals(oid));
                if (org != null)
                {
                    guid = org.Id;
                }
            });
            return guid;
        }

        public Guid GetOrganizationIdBySource(string sourceId)
        {
            var guid = Guid.Empty;
            var userId = utilities.GetRelationIdForExternalSource();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var extRep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var ext = extRep.All().FirstOrDefault(e => e.SourceId == sourceId && e.ObjectType == "Organization" && e.RelationId == userId);
                if (ext != null)
                {
                    guid = ext.PTVId;
                }
            });
            return guid;
        }

        public IVmOpenApiOrganization AddOrganization(IVmOpenApiOrganizationInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInBase, V2VmOpenApiOrganizationInBase>(vm);
            return TranslationManagerToVm.Translate<V2VmOpenApiOrganization, VmOpenApiOrganization>(V2AddOrganization(vm2, allowAnonymous) as V2VmOpenApiOrganization);
        }

        public IV2VmOpenApiOrganization V2AddOrganization(IV2VmOpenApiOrganizationInBase vm, bool allowAnonymous, bool version1 = false)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new Organization();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists
                if (ExternalSourceExists<Organization>(vm.SourceId, userId, unitOfWork))
                {
                    throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
                }

                organization = TranslationManagerToEntity.Translate<IV2VmOpenApiOrganizationInBase, Organization>(vm, unitOfWork);

                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                organizationRep.Add(organization);

                // Create the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    SetExternalSource(organization, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode);
            });

            // Update the map coordinates for addresses
            if (organization?.OrganizationAddresses?.Count > 0)
            {
                var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }

            if (version1)
            {
                return GetOrganization(organization.Id, false);
            }
            return V2GetOrganization(organization.Id, false); ;
        }

        public bool OrganizationExists(Guid id)
        {
            var result = false;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                if (organizationRep.All().FirstOrDefault(o => o.Id.Equals(id)) != null)
                {
                    result = true;
                }
            });
            return result;
        }

        public IVmListItemsData<IVmListItem> GetOrganizations(string searchText)
        {
            IReadOnlyList<VmListItem> result = new List<VmListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                result = commonService.GetOrganizationNames(unitOfWork, searchText, false);
            });

            return new VmListItemsData<IVmListItem>(result);

        }

        public IVmGetOrganizationSearch GetOrganizationSearch()
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            string statusDeletedCode = PublishingStatus.Deleted.ToString();

            var result = new VmGetOrganizationSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //var userOrganization = utilities.GetUserOrganization(unitOfWork);


				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //result.OrganizationId = userOrganization?.Id

                var publishingStatuses = commonService.GetPublishingStatuses(unitOfWork);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses)
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode).Select(x => x.Id).ToList();

            });

            return result;
        }

        public IVmOrganizationSearchResult SearchOrganizations(IVmOrganizationSearch vmOrganizationSearch, bool takeAll = false)
        {
            IReadOnlyList<IVmOrganizationListItem> result = new List<VmOrganizationListItem>();
            var count = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                var resultTemp = orgRep.All();

                if (vmOrganizationSearch.OrganizationId.HasValue)
                {
                    var allChilds = GetAllChildrenFlatten(resultTemp, new List<Guid>() {vmOrganizationSearch.OrganizationId.Value});
                    resultTemp = resultTemp.Where(x => allChilds.Contains(x.Id));
                }

                if (!string.IsNullOrEmpty(vmOrganizationSearch.OrganizationName))
                {
                    var searchText = vmOrganizationSearch.OrganizationName.ToLower();
                    resultTemp = resultTemp.Where(x => x.OrganizationNames.Any(y => y.TypeId == x.DisplayNameTypeId && y.Name.ToLower().Contains(searchText)));
                }

                if (vmOrganizationSearch.SelectedPublishingStatuses != null)
                {
                    resultTemp = resultTemp.WherePublishingStatusIn(vmOrganizationSearch.SelectedPublishingStatuses);
                }

                count = resultTemp.Count();
                resultTemp = resultTemp.OrderBy(i => i.Id);

                resultTemp = !takeAll ? vmOrganizationSearch.PageNumber > 0
                    ? resultTemp.Skip(vmOrganizationSearch.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : resultTemp.Take(CoreConstants.MaximumNumberOfAllItems) : resultTemp;

                resultTemp = unitOfWork.ApplyIncludes(resultTemp, q =>
                    q.Include(i => i.OrganizationNames).ThenInclude(i => i.Type)
                    .Include(i => i.Parent).ThenInclude(i => i.OrganizationNames).ThenInclude(i => i.Type)
                    .Include(i => i.Children).ThenInclude(i => i.OrganizationNames).ThenInclude(i => i.Type));

                result = TranslationManagerToVm.TranslateAll<Organization, VmOrganizationListItem>(resultTemp);
            });

            return new VmOrganizationSearchResult() { Organizations = result, PageNumber = ++vmOrganizationSearch.PageNumber, Count = count };
        }

        public IVmOrganizationStep1 GetOrganizationStep1(Guid? organizationId)
        {
            var result = new VmOrganizationStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organization = GetEntity<Organization>(organizationId, unitOfWork,
                    q => q
                        .Include(x => x.OrganizationNames)
                        .Include(x => x.OrganizationDescriptions)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.Municipality)
                        .Include(x => x.Business)
                        .Include(x => x.OrganizationEmails).ThenInclude(x => x.Email)
                        .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone)
                        .Include(x => x.OrganizationWebAddress).ThenInclude(x => x.WebPage)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.StreetNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.PostalCode)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                        );

                result = GetModel<Organization, VmOrganizationStep1>(organization);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", commonService.GetOrganizationTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes(unitOfWork)),
                    () => GetEnumEntityCollectionModel("WebPageTypes", commonService.GetPhoneChargeTypes(unitOfWork))
                );

                //result.OrganizationTypes = commonService.GetOrganizationTypes(unitOfWork);
                //result.Organizations = commonService.GetOrganizationNames(unitOfWork);
                //result.PhoneChargeTypes = commonService.GetPhoneChargeTypes(unitOfWork);

                //if (result.PhoneChargeTypes.Count > 0)
                //{
                //    result.PhoneChargeTypes.First().IsSelected = true;
                //}

                //result.WebPageTypes = commonService.GetWebPageTypes(unitOfWork);
            });

            return result;
        }

        public IVmOrganizationStep1 SaveOrganizationStep1(VmOrganizationModel model)
        {
            Guid? organizationId = null;
            Organization organization = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                organizationLogic.PrefilterViewModel(model.Step1Form);
                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
                if (!string.IsNullOrEmpty(model.Step1Form.OrganizationId) && organizationRep.All().Any(x => (x.Id != model.Id) && (x.Oid == model.Step1Form.OrganizationId)))
                {
                    throw new ArgumentException("", model.Step1Form.OrganizationId);
                }

                var nameCode = model.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
                model.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;

                organization = TranslationManagerToEntity.Translate<VmOrganizationModel, Organization>(model, unitOfWork);

                //Removing emails
                var emailRep = unitOfWork.CreateRepository<IOrganizationEmailRepository>();
                var emailIds = organization.OrganizationEmails.Select(x => x.EmailId).ToList();
                var emailsToRemove = emailRep.All().Where(x => x.OrganizationId == organization.Id && !emailIds.Contains(x.EmailId));
                emailsToRemove.ForEach(x => emailRep.Remove(x));

                //Removing phones
                var phoneRep = unitOfWork.CreateRepository<IOrganizationPhoneRepository>();
                var phoneIds = organization.OrganizationPhones.Select(x => x.PhoneId).ToList();
                var phonesToRemove = phoneRep.All().Where(x => x.OrganizationId == organization.Id && !phoneIds.Contains(x.PhoneId));
                phonesToRemove.ForEach(x => phoneRep.Remove(x));

                //Removing webPages
                var webPageRep = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
                var wpIds = organization.OrganizationWebAddress.Select(x => x.WebPage.Id).ToList();
                var webPagesToRemove = webPageRep.All().Where(x => x.OrganizationId == organization.Id && !wpIds.Contains(x.WebPageId));
                webPagesToRemove.ForEach(x => webPageRep.Remove(x));

                //Removing Address
                var addressRep = unitOfWork.CreateRepository<IOrganizationAddressRepository>();
                var addressIds = organization.OrganizationAddresses.Select(x => x.AddressId).ToList();
                var addressesToRemove = addressRep.All().Where(x => x.OrganizationId == organization.Id && !addressIds.Contains(x.AddressId));
                addressesToRemove.ForEach(x => addressRep.Remove(x));

                unitOfWork.Save(parentEntity: organization);
                organizationId = organization.Id;

            });
            var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            return GetOrganizationStep1(organizationId);
        }

        public IVmEntityBase AddApiOrganization(VmOrganizationModel model)
        {
            Organization organization = null;
            var result = new VmEnumEntityStatusBase();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                organization = AddOrganization(unitOfWork, model);
                unitOfWork.Save();
                FillEnumEntities(result,
                  () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList())
              );
            });

            var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            result.Id = organization.Id;
            result.PublishingStatus = commonService.GetDraftStatusId();
            return result;
        }

        private Organization AddOrganization(IUnitOfWorkWritable unitOfWork, VmOrganizationModel vm)
        {
            var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();

            if (!string.IsNullOrEmpty(vm.Step1Form.OrganizationId) && organizationRep.All().Any(x => x.Oid == vm.Step1Form.OrganizationId))
            {
                throw new ArgumentException("", vm.Step1Form.OrganizationId);
            }

            vm.PublishingStatus = commonService.GetDraftStatusId();
            var nameCode = vm.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
            vm.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;
            organizationLogic.PrefilterViewModel(vm.Step1Form);

            var organization = TranslationManagerToEntity.Translate<VmOrganizationModel, Organization>(vm, unitOfWork);
            organizationRep.Add(organization);
            return organization;
        }

        private IList<V2VmOpenApiOrganization> GetOrganizationsWithDetails(Expression<Func<Organization, bool>> filter, bool getOnlyPublished = true)
        {
            var result = new List<V2VmOpenApiOrganization>();
            var resultTemp = new List<Organization>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
                var query = organizationRep.All().Where(filter);
                if (getOnlyPublished)
                {
                    query = query.Where(PublishedFilter<Organization>()).Where(ValidityFilter<Organization>()); // Get only published organizations
                }

                resultTemp = unitOfWork.ApplyIncludes(query, q =>
                    q.Include(i => i.Business)
                        .Include(i => i.Type)
                        .Include(i => i.Municipality)
                        .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                        .Include(i => i.OrganizationNames)
                        .Include(i => i.OrganizationDescriptions)
                        .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone)
                        .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                        .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.PostalCode)
                        .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Municipality)
                        .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                        .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.StreetNames)
                        .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                        .Include(i => i.PublishingStatus)
                        .Include(i => i.OrganizationServices).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                        .Include(i => i.OrganizationServices).ThenInclude(i => i.AdditionalInformations)
                    ).ToList();

                // Attach only published services, published phone numbers, published email addresses ,published web pages and addresses into an organization.
                // Have to use a workaround because of EF 'feature'. See e.g. https://github.com/aspnet/EntityFramework/issues/5672
                resultTemp.ForEach(organization =>
                {
                    if (organization.OrganizationServices.Count > 0)
                    {
                        var serviceRep = unitOfWork.CreateRepository<IServiceRepository>();
                        var organizationServices = organization.OrganizationServices.Select(s => s.ServiceId).ToList();
                        var publishedServices = serviceRep.All().Where(s => organizationServices.Contains(s.Id)).Where(PublishedFilter<Service>()).Where(ValidityFilter<Service>()).Select(s => s.Id).ToList();
                        organization.OrganizationServices = organization.OrganizationServices.Where(s => publishedServices.Contains(s.ServiceId)).ToList();
                    }
                });
            });

            result = TranslationManagerToVm.TranslateAll<Organization, V2VmOpenApiOrganization>(resultTemp).ToList();

            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            return result;
        }

        public IVmEntityBase GetOrganizationStatus(Guid? organizationId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetOrganizationStatus(unitOfWork, organizationId);
            });
            return new VmEntityStatusBase { PublishingStatus = result.Id };
        }

        private VmPublishingStatus GetOrganizationStatus(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var serviceRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var service = serviceRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == organizationId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }

        public IVmEntityBase PublishOrganization(Guid? entityId)
        {
            Organization result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = PublishOrganization(unitOfWork, entityId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatus = result.PublishingStatusId };

        }

        private Organization PublishOrganization(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Published.ToString(), unitOfWork);

            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var organization = organizationRep.All().Single(x => x.Id == entityId.Value);
            organization.PublishingStatus = publishStatus;
            return organization;
        }

        public IVmEntityBase DeleteOrganization(Guid? serviceId)
        {
            Organization result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteOrganization(unitOfWork, serviceId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase() { Id = result.Id, PublishingStatus = result.PublishingStatusId };
        }

        private Organization DeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id)
        {
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);

            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var organization = organizationRep.All().Single(x => x.Id == id.Value);
            organization.PublishingStatus = publishStatus;

            return organization;
        }

        public IVmOpenApiOrganization SaveOrganization(IVmOpenApiOrganizationInBase vm, bool allowAnonymous)
        {
            var vm2 = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInBase, V2VmOpenApiOrganizationInBase>(vm);
            return V2SaveOrganization(vm2, allowAnonymous, true) as VmOpenApiOrganization;
        }

        public IV2VmOpenApiOrganization V2SaveOrganization(IV2VmOpenApiOrganizationInBase vm, bool allowAnonymous, bool version1 = false)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new Organization();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                vm.Id = vm.Id ?? GetPTVId<Organization>(vm.SourceId, userId, unitOfWork);
                organization = TranslationManagerToEntity.Translate<IV2VmOpenApiOrganizationInBase, Organization>(vm, unitOfWork);
                if (vm.DeleteAllPhones || (vm.PhoneNumbers != null && vm.PhoneNumbers.Count > 0))
                {
                    // Remove the phones that were not included in vm
                    organization.OrganizationPhones = UpdateOrganizationCollectionWithRemove<OrganizationPhone, Phone>(unitOfWork, organization.Id,
                        organization.OrganizationPhones, e => e.PhoneId);
                }
                if (vm.DeleteAllEmails || (vm.EmailAddresses != null && vm.EmailAddresses.Count > 0))
                {
                    // Remove all emails that were not included in vm
                    organization.OrganizationEmails = UpdateOrganizationCollectionWithRemove<OrganizationEmail, Email>(unitOfWork, organization.Id,
                        organization.OrganizationEmails, e => e.EmailId);
                }
                if (vm.DeleteAllWebPages || (vm.WebPages != null && vm.WebPages.Count > 0))
                {
                    // Remove all web pages that were not included in vm
                    organization.OrganizationWebAddress = UpdateOrganizationCollectionWithRemove<OrganizationWebPage, WebPage>(unitOfWork, organization.Id,
                        organization.OrganizationWebAddress, e => e.WebPageId);
                }
                if (vm.DeleteAllAddresses || (vm.Addresses != null && vm.Addresses.Count > 0))
                {
                    // Remove all adresses that were not included in vm
                    organization.OrganizationAddresses = UpdateOrganizationCollectionWithRemove<OrganizationAddress, Address>(unitOfWork, organization.Id,
                        organization.OrganizationAddresses, e => e.AddressId);
                }

                // Update the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    UpdateExternalSource(organization, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode, organization);
            });

            // Update the map coordinates for addresses
            if (organization?.OrganizationAddresses?.Count > 0)
            {
                var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }

            if (version1)
            {
                return GetOrganization(vm.Id.Value, false);
            }
            return V2GetOrganization(vm.Id.Value, false);
        }

        public string GetOrganizationAddressType(string sourceId)
        {
            OrganizationAddress address = new OrganizationAddress();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var externalRep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var id = GetPTVId<Address>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    var rep = unitOfWork.CreateRepository<IOrganizationAddressRepository>();
                    address = unitOfWork.ApplyIncludes(rep.All().Where(x => x.AddressId.Equals(id)), q =>
                        q.Include(i => i.Type)).FirstOrDefault();
                }
            });

            return address != null && address.Type != null ? address.Type.Code : null;
        }

        public string GetOrganizationWebPageType(string sourceId)
        {
            OrganizationWebPage webPage = new OrganizationWebPage();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var externalRep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var id = GetPTVId<WebPage>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    var rep = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
                    webPage = unitOfWork.ApplyIncludes(rep.All().Where(x => x.WebPageId.Equals(id)), q =>
                        q.Include(i => i.Type)).FirstOrDefault();
                }
            });

            return webPage != null && webPage.Type != null ? webPage.Type.Code : null;
        }

        private ICollection<T> UpdateOrganizationCollectionWithRemove<T, TEntity>(IUnitOfWorkWritable unitOfWork, Guid organizationId, ICollection<T> collection, Func<T, Guid> getSelectedIdFunc)
            where T : IOrganization  where TEntity : IEntityIdentifier
        {
            // Remove all organization related entities that were not included in collection
            var updatedEntities = collection.Select(getSelectedIdFunc).ToList();
            var rep = unitOfWork.CreateRepository<IRepository<T>>();
            var currentOrganizationEntities = rep.All().Where(e => e.OrganizationId == organizationId).Select(getSelectedIdFunc).ToList();
            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            currentOrganizationEntities.Where(e => !updatedEntities.Contains(e)).ForEach(e => {
                var entity = entityRep.All().FirstOrDefault(r => r.Id == e);
                if (entity != null)
                {
                    entityRep.Remove(entity);
                }
                });

            return collection;
        }

        private void SetOrganizationListAndExternalSource<TEntity, TModel>(IList<TModel> vmlist, ICollection<TEntity> entityList, string userId, IUnitOfWorkWritable unitOfWork)
            where TModel : class, IVmOpenApiBase, IVmOpenApiSource, new() where TEntity : class, IEntityIdentifier
        {
            vmlist.ForEach(item =>
            {
                var entity = TranslationManagerToEntity.Translate<TModel, TEntity>(item, unitOfWork);
                if (!item.Id.HasValue)
                {
                    // We have added a new entity so add also mapping into ExternalSource table
                    entityList.Add(entity);
                    SetExternalSource(entity, item.SourceId, userId, unitOfWork);
                }
            });
        }
    }
}

