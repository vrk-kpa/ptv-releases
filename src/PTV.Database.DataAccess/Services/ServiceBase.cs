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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Exceptions;

namespace PTV.Database.DataAccess.Services
{
    internal abstract class ServiceBase
    {
        protected IPublishingStatusCache PublishingStatusCache;
        protected ITranslationEntity TranslationManagerToVm { get; }
        protected ITranslationViewModel TranslationManagerToEntity { get; }
        protected IVersioningManager VersioningManager { get; }

        private const string EntityNotFoundMessage = ".Exception.NotFound";
        private IUserOrganizationChecker userOrganizationChecker;

        protected ServiceBase(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity, IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker, IVersioningManager versioningManager)
        {
            TranslationManagerToVm = translationManagerToVm;
            TranslationManagerToEntity = translationManagerToEntity;
            PublishingStatusCache = publishingStatusCache;
            VersioningManager = versioningManager;
            this.userOrganizationChecker = userOrganizationChecker;
        }

        protected EntityNotFoundException CreateEntityNotFound<TEntity>(Guid? id)
        {
            return new EntityNotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found", typeof(TEntity).Name.Replace("Versioned", string.Empty) + EntityNotFoundMessage, new[] { id.ToString() });
        }

        internal TEntity GetEntity<TEntity>(Guid? id, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain) where TEntity : class, IEntityIdentifier
        {
            if (id.IsAssigned())
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = unitOfWork.ApplyIncludes(repository.All(), includeChain, true).SingleOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    throw CreateEntityNotFound<TEntity>(id);
                }
                return entity;
            }
            return null;
        }

        protected TModel GetModel<TEntity, TModel>(TEntity entity, IUnitOfWork unitOfWork) where TModel : VmEntityBase, new() where TEntity: class
        {
            if (entity != null)
            {
                var model = TranslationManagerToVm.Translate<TEntity, TModel>(entity);
                SetOrganizationSecurity(model as IVmEntitySecurity, entity, unitOfWork);
                return model;
            }
            return new TModel();
        }

        private void SetOrganizationSecurity<TEntity>(IVmEntitySecurity model, TEntity entity, IUnitOfWork unitOfWork)
        {
            if (model != null)
            {
                model.Security = userOrganizationChecker.CheckEntity(entity, unitOfWork);
            }
        }

        protected bool GetOrganizationSecurity<TEntity>(TEntity entity, IUnitOfWork unitOfWork)
        {
            return userOrganizationChecker.CheckEntity(entity, unitOfWork).IsOwnOrganization;
        }

        /// <summary>
        /// Get the filter for fetching only valid (in datetime from/to) entities from database.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> ValidityFilter<TEntity>() where TEntity : class, IValidity
        {
            var now = DateTime.UtcNow;
            return p => ((p.ValidFrom <= now && p.ValidTo >= now) || (p.ValidFrom == null && p.ValidTo == null));
        }

        /// <summary>
        /// Get the filter for fetching only published entities from database.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> PublishedFilter<TEntity>() where TEntity : class, IPublishingStatus
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            return p => p.PublishingStatusId == publishedId;
            //return p => p.PublishingStatus.Code == PublishingStatus.Published.ToString();
        }

        /// <summary>
        /// Get the filter for fetching only archived entities from database. Item is archived if no published version exist. If item has only Draft version it is not archived.
        /// So we need to get items that have either Deleted or OldPublished version but no Published version.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Published filter</returns>
        internal Expression<Func<TEntity, bool>> ArchivedFilter<TEntity, TRoot>() where TEntity : class, IPublishingStatus, IVersionedVolume<TRoot> where TRoot : VersionedRoot<TEntity>
        {
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var deletedId = PublishingStatusCache.Get(PublishingStatus.Deleted);
            var oldPublishedId = PublishingStatusCache.Get(PublishingStatus.OldPublished);

            return p => (p.PublishingStatusId == deletedId || p.PublishingStatusId == oldPublishedId) &&
                !p.UnificRoot.Versions.Any(x => x.PublishingStatusId == publishedId);
        }

        private List<OrganizationVersioned> LoadOrganizationTreeRecursiveFromBottom(IQueryable<OrganizationVersioned> inputQuery, List<Guid> leafRootIds)
        {
            if (leafRootIds.IsNullOrEmpty())
            {
                return new List<OrganizationVersioned>();
            }
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var orgs = inputQuery.Where(x => x .PublishingStatusId == psPublished && leafRootIds.Contains(x.UnificRootId)).Include(i => i.OrganizationNames).ToList();
            var parentRoots = orgs.Where(i => i.ParentId != null).Select(i => i.ParentId.Value).ToList();
            orgs.AddRange(LoadOrganizationTreeRecursiveFromBottom(inputQuery, parentRoots));
            return orgs;
        }

        internal List<T> LoadFintoTree<T>(IQueryable<T> inputQuery, int? levelsToLoad = null, ICollection<Guid> ids = null)
            where T : IHierarchy<T>, IEntityIdentifier, IIsValid
        {
            var result = LoadFintoTreeRecursive(inputQuery.Where(x => x.IsValid), levelsToLoad, ids);
            if (result.IsNullOrEmpty())
            {
                return new List<T>();
            }
            var map = result.ToDictionary(i => i.Id, i => i);
            result.Where(i => i.ParentId != null).ForEach(i =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                i.Parent = map.TryGet(i.ParentId.Value);
            });
            result.ForEach(i =>
            {
                i.Children = result.Where(j => j.ParentId == i.Id).ToList();
            });
            return ids == null ? result.Where(i => i.ParentId == null).ToList() : result.Where(i => i.ParentId == null || (i.ParentId != null && ids.Contains(i.ParentId.Value))).ToList();
        }

        internal IQueryable<T> GetIncludesForFinto<T, TName>(IUnitOfWork unitOfWork, IQueryable<T> inputQuery)
            where T : class, IFintoItemNames<TName>, IEntityIdentifier where TName  : NameBase
        {
            return unitOfWork.ApplyIncludes(inputQuery, q => q.Include(i => i.Names).ThenInclude(i => i.Localization));
        }

        private List<T> LoadFintoTreeRecursive<T>(IQueryable<T> inputQuery, int? levelsToLoad = null, ICollection<Guid> ids = null) where T : IHierarchy<T>, IEntityIdentifier
        {
            var query = inputQuery;
            if (ids == null)
            {
                query = query.Where(x => x.ParentId == null);
            }
            else if (ids.Count > 0)
            {
                var idsNullable = ids.Cast<Guid?>().ToList();
                query = query.Where(x => idsNullable.Contains(x.ParentId));
            }
            else
            {
                return new List<T>();
            }
            var result = query.ToList();
            if (result.Count > 0 && (!levelsToLoad.HasValue || levelsToLoad > 0))
            {
                result.AddRange(LoadFintoTreeRecursive(inputQuery, --levelsToLoad, result.Select(x => x.Id).ToList()));
            }
            return result;
        }

        internal List<T> CreateList<T>(IEnumerable<IFintoItemBase> data, Func<T, string> orderBy) where T : VmListItem
        {
            var result = TranslationManagerToVm.TranslateAll<IFintoItemBase, T>(data.ToList()).OrderBy(orderBy).ToList();
            return result;
        }

        internal IEnumerable<TOut> CreateTree<TTranslate, TOut>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem where TOut : IVmTreeItem
        {
            return CreateTreeInternal(data, orderBy).Cast<TOut>().ToList();
        }

        internal IEnumerable<TTranslate> CreateTree<TTranslate>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem
        {
            return CreateTreeInternal(data, orderBy).ToList();
        }

        private List<TTranslate> CreateTreeInternal<TTranslate>(IEnumerable<IFintoItem> data, Func<TTranslate, string> orderBy) where TTranslate : class, IVmTreeItem
        {
            return TranslationManagerToVm.TranslateAll<IFintoItem, TTranslate>(data.ToList()).OrderBy(orderBy).ToList();
        }

        internal List<VmTreeItem> CreateTree<T>(IEnumerable<OntologyTerm> data) where T : VmTreeItem
        {
            var result = data.Select( x => TranslationManagerToVm.Translate<OntologyTerm, T>(x) as VmTreeItem).OrderBy(x => x.Name).ToList();
            return result;
        }

        internal List<IVmTreeItem> CreateTree<T>(IEnumerable<OrganizationTreeItem> data) where T : class, IVmTreeItem
        {
            return TranslationManagerToVm.TranslateAll<OrganizationTreeItem, T>(data).OrderBy(x => x.Name).Cast<IVmTreeItem>().ToList();
        }
//
//        internal IEnumerable<Guid> GetAllHierarchyChildrenFlatten<T,Tp,Tch>(IQueryable<T> inputQuery, List<Guid> ids) where T : IHierarchy<Tp, Tch> where Tp : IVersionedRoot where Tch : IVersionedVolume
//        {
//            var idsNullable = ids.Cast<Guid?>().ToList();
//            var result = inputQuery.Where(x => x.ParentId.HasValue && idsNullable.Contains(x.ParentId)).Select(x => x.Id).ToList();
//
//            if (ids.Any())
//            {
//                result.AddRange(GetAllChildrenFlatten(inputQuery, result));
//            }
//
//            return result.Distinct();
//        }

        internal List<OrganizationVersioned> GetOrganizationsFlatten(ICollection<OrganizationTreeItem> organizationTreeItems)
        {
            var result = organizationTreeItems.Select(i => i.Organization);
            var children = organizationTreeItems.SelectMany(i => i.Children).ToList();
            return (children.Any() ? result.Union(GetOrganizationsFlatten(children)) : result).ToList();
        }

        internal IEnumerable<Guid> GetAllChildrenFlatten<T>(IQueryable<T> inputQuery, List<Guid> ids) where T : IHierarchy<T>, IEntityIdentifier
        {
            var idsNullable = ids.Cast<Guid?>().ToList();
            var result = inputQuery.Where(x => x.ParentId.HasValue && idsNullable.Contains(x.ParentId)).Select(x => x.Id).ToList();

            if (ids.Any())
            {
                result.AddRange(GetAllChildrenFlatten(inputQuery, result));
            }

            return result.Distinct();
        }

        private List<T> GetTreeFlatten<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            var newResult = foundQuery.ToList();

            if (newResult.Count > 0)
            {
                var parentIds = newResult.Where(x => x.ParentId.HasValue && x.ParentId != x.Id).Select(x => x.ParentId).Distinct().Cast<Guid>().ToList();
                var allParents = inputQuery.Where(x => parentIds.Contains(x.Id));
                var subResult = GetTreeFlatten<T>(inputQuery, allParents);
                newResult.AddRange(subResult.Where(j => !newResult.Select(i => i.Id).Contains(j.Id)));
            }

            return newResult;
        }

        internal IEnumerable<T> SearchTree<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            return SearchFintoFlattenTree(inputQuery, foundQuery).Where(x => !x.ParentId.HasValue);
        }

        internal IEnumerable<T> SearchFintoFlattenTree<T>(IQueryable<T> inputQuery, IEnumerable<T> foundQuery) where T : IHierarchy<T>, IEntityIdentifier
        {
            var result = GetTreeFlatten(inputQuery, foundQuery);
            if(result.IsNullOrEmpty())
            {
                return new List<T>();
            }
            var map = result.ToDictionary(i => i.Id, i => i);
            result.Where(i => i.ParentId != null).ForEach(i =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                i.Parent = map.TryGet(i.ParentId.Value);
            });
            result.ForEach(i =>
            {
                i.Children = result.Where(j => j.ParentId == i.Id).ToList();
            });
            return result;
        }

        internal void FillEnumEntities(IEnumCollection model, params Func<KeyValuePair<string, IEnumerable<IVmBase>>>[] getEnumsFuncs)
        {
            model.EnumCollection = new VmDictionaryItemsData<IEnumerable<IVmBase>>(getEnumsFuncs.Select(getData => getData()).ToDictionary(x => x.Key, x => x.Value));
        }

        internal KeyValuePair<string, IEnumerable<IVmBase>> GetEnumEntityCollectionModel(string key, IEnumerable<IVmBase> collection)
        {
            return new KeyValuePair<string, IEnumerable<IVmBase>>(key, collection);
        }

		/// <summary>
        /// Set the data for ExternalSource table.
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="sourceId">External source id</param>
        /// <param name="PTVId">PTV object id</param>
        /// <param name="unitOfWork"></param>
        internal void SetExternalSource<TEntity>(TEntity entity, string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (string.IsNullOrEmpty(sourceId) || string.IsNullOrEmpty(userId))
            {
                throw new Exception(CoreMessages.OpenApi.ExternalSourceMalFormatted);
            }

            if (ExternalSourceExists<TEntity>(sourceId, userId, unitOfWork))
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceForOtherExists, sourceId, typeof(TEntity).Name));
            }

            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            rep.Add(new ExternalSource()
            {
                Id = Guid.NewGuid(),
                SourceId = sourceId,
                PTVId = entity.Id,
                ObjectType = typeof(TEntity).Name,
                RelationId = userId
            });
        }

        internal void UpdateExternalSource<TEntity>(Guid entityId, string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (string.IsNullOrEmpty(sourceId))
            {
                return;
            }
            var objectType = typeof(TEntity).Name;
            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            var currentExternalSource = rep.All().Where(e => e.RelationId == userId && e.ObjectType == objectType && e.SourceId == sourceId).FirstOrDefault();
            if (currentExternalSource != null && currentExternalSource.PTVId != entityId)
            {
                throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExistsUpdate, sourceId, currentExternalSource.PTVId));
            }
            var externalSource = rep.All().Where(e => e.PTVId == entityId && e.ObjectType == objectType && e.RelationId == userId).FirstOrDefault();

            if (externalSource != null)
            {
                externalSource.SourceId = sourceId;
            }
            else
            {
                rep.Add(new ExternalSource()
                {
                    Id = Guid.NewGuid(),
                    SourceId = sourceId,
                    PTVId = entityId,
                    ObjectType = typeof(TEntity).Name,
                    RelationId = userId
                });
            }
        }

        internal bool ExternalSourceExists<TEntity>(string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            if (!string.IsNullOrEmpty(sourceId))
            {
                var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var objectType = typeof(TEntity).Name;
                var externalSource = rep.All().Where(e => e.SourceId == sourceId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == objectType).FirstOrDefault();
                return externalSource != null;
            }

            return false;
        }

        internal Guid GetPTVId<TEntity>(string sourceId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            var id = Guid.Empty;
            var entityName = typeof(TEntity).Name;
            if (!string.IsNullOrEmpty(sourceId))
            {
                var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
                var externalSource = rep.All().Where(e => e.SourceId == sourceId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == entityName).FirstOrDefault();
                id = externalSource != null ? externalSource.PTVId : Guid.Empty;
            }
            if (id == Guid.Empty)
            {
                throw new ExternalSourceNotFoundException(string.Format(CoreMessages.OpenApi.EntityNotFound, entityName, sourceId));
            }
            return id;
        }

        internal string GetSourceId<TEntity>(Guid ptvId, string userId, IUnitOfWork unitOfWork) where TEntity : class, IEntityIdentifier
        {
            var rep = unitOfWork.CreateRepository<IExternalSourceRepository>();
            var objectType = typeof(TEntity).Name;
            var externalSource = rep.All().Where(e => e.PTVId == ptvId && e.RelationId.ToLower() == userId.ToLower() && e.ObjectType == objectType).FirstOrDefault();
            return externalSource != null ? externalSource.SourceId : null;
        }

        //        protected Guid SetTranslatorLanguage(IVmLocalized model = null)
        //        {
        //            var languageId = languageCache.Get(model?.Language ?? DomainConstants.DefaultLanguage);
        //            TranslationManagerToVm.SetLanguage(languageId);
        //            TranslationManagerToEntity.SetLanguage(languageId);
        //            return languageId;
        //        }

        //        protected LanguageCode SetTranslatorLanguage(IVmMultiLocalized model = null)
        //        {
        //            var languageCode = model?.Languages?.FirstOrDefault() ?? "fi";
        //            TranslationManagerToVm.SetLanguage(languageCode);
        //            TranslationManagerToEntity.SetLanguage(languageCode);
        //            return languageCode;
        //        }

        

        /// <summary>
        /// Get the requested open api version from the model.
        /// The base model has version number 0.
        /// Example: Let's assume that we are supporting versions 1 - 4. User has requested to get version 2. First we have the base model (version number 0).
        /// We'll get the previous version from base, which in this case would be version 4. Version 4 is not yet the requested one so we get the previous one (version 3).
        /// Still the version 3 is not the requested one so we get the previous one which would be version 2. This is the requested one so let's return that view model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="vm"></param>
        /// <param name="openApiVersion"></param>
        /// <returns></returns>
        internal TModel GetEntityByOpenApiVersion<TModel>(TModel vm, int openApiVersion) where TModel : IOpenApiVersionBase<TModel>
        {
            var version = vm.VersionNumber();

            if (openApiVersion == 0 && version == 0) // Base version is requested
            {
                return vm;
            }

            if (version != 0 && version <= openApiVersion)
            {
                return vm;
            }

            return GetEntityByOpenApiVersion(vm.PreviousVersion(), openApiVersion);
        }

        /// <summary>
        /// Return Guid from string name
        /// </summary>
        /// <param name="name">Name of service, channel, GD</param>
        /// <returns></returns>
        internal Guid? GetRootIdFromString(string name)
        {
            Guid result;
            if (Guid.TryParse(name.Trim(), out result))
            {
                return result;
            }
            return null;
        }

        internal Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> GetConnectionIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceServiceChannelDescriptions)
                .Include(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                .Include(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ExtraSubType).ThenInclude(i => i.Names)
                .Include(i => i.ServiceServiceChannelExtraTypes).ThenInclude(i => i.ServiceServiceChannelExtraTypeDescriptions)
                .Include(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.ServiceServiceChannelServiceHours).ThenInclude(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceServiceChannelEmails).ThenInclude(i => i.Email)
                .Include(i => i.ServiceServiceChannelPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.ServiceServiceChannelWebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address)
                    .ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.ServiceServiceChannelAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations);
        }
                
        internal void MapConnection(ServiceServiceChannel ssc, IVmOpenApiServiceServiceChannelBase vmssc, ITypesCache typesCache, ILanguageCache languageCache)
        {
            if (ssc == null || vmssc == null) return;

            // ServiceChargeType values changed into new ones (PTV-2184)
            var type = ssc.ChargeTypeId.IsAssigned() ? typesCache.GetByValue<ServiceChargeType>(ssc.ChargeTypeId.Value) : null;
            vmssc.ServiceChargeType = string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceChargeTypeEnum>();

            // manually map items
            // connection description
            vmssc.Description = GetDescriptions(ssc.ServiceServiceChannelDescriptions, typesCache, languageCache);

            // servicehours
            if (ssc.ServiceServiceChannelServiceHours != null && ssc.ServiceServiceChannelServiceHours.Count > 0)
            {
                ssc.ServiceServiceChannelServiceHours.ForEach(srcHour =>
                {
                    //// use the translator
                    //var translated = TranslationManagerToVm.Translate<ServiceServiceChannelServiceHours, V4VmOpenApiServiceHour>(srcHour);

                    //if (translated != null)
                    //{
                    //    vmssc.ServiceHours.Add(translated);
                    //}

                    // manual mapping                    
                    if (srcHour != null && srcHour.ServiceHours != null)
                    {
                        var vmHours = srcHour.ServiceHours.GetOpenApiModel(typesCache, languageCache);
                        if (vmHours != null)
                        {
                            vmssc.ServiceHours.Add(vmHours);
                        }                        
                    }
                    // end manual mapping
                });
            }
        }

        internal List<V9VmOpenApiExtraType> GetExtraTypes(ServiceServiceChannel ssc, ITypesCache typesCache, ILanguageCache languageCache)
        {
            var list = new List<V9VmOpenApiExtraType>();

            if (ssc.ServiceServiceChannelExtraTypes != null && ssc.ServiceServiceChannelExtraTypes.Count > 0)
            {
                ssc.ServiceServiceChannelExtraTypes.ForEach(srcExtras =>
                {
                    V9VmOpenApiExtraType extra = new V9VmOpenApiExtraType()
                    {
                        ChannelGuid = srcExtras.ServiceChannelId,
                        Code = typesCache.GetByValue<ExtraSubType>(srcExtras.ExtraSubTypeId),
                        ServiceGuid = srcExtras.ServiceId,
                        Type = typesCache.GetByValue<ExtraType>(srcExtras.ExtraSubType.ExtraTypeId)
                    };

                    if (srcExtras.ExtraSubType?.Names?.Count > 0)
                    {
                        List<VmOpenApiLanguageItem> nameList = new List<VmOpenApiLanguageItem>(srcExtras.ExtraSubType.Names.Count);
                        srcExtras.ExtraSubType.Names.ForEach(srcName =>
                        {
                            VmOpenApiLanguageItem name = new VmOpenApiLanguageItem()
                            {
                                Language = languageCache.GetByValue(srcName.LocalizationId),
                                Value = srcName.Name
                            };

                            nameList.Add(name);
                        });

                        // set the names list
                        extra.Name = nameList;
                    }

                    if (srcExtras.ServiceServiceChannelExtraTypeDescriptions?.Count > 0)
                    {
                        List<VmOpenApiLanguageItem> descriptionList = new List<VmOpenApiLanguageItem>(srcExtras.ServiceServiceChannelExtraTypeDescriptions.Count);

                        srcExtras.ServiceServiceChannelExtraTypeDescriptions.ForEach(srcDesc =>
                        {
                            VmOpenApiLanguageItem desc = new VmOpenApiLanguageItem()
                            {
                                Language = languageCache.GetByValue(srcDesc.LocalizationId),
                                Value = srcDesc.Description
                            };

                            descriptionList.Add(desc);
                        });

                        // set the description list
                        extra.Description = descriptionList;
                    }

                    list.Add(extra);
                });
            }
            return list;
        }

        internal List<V4VmOpenApiFintoItem> GetDigitalAuthorizations(ServiceServiceChannel ssc, ILanguageCache languageCache)
        {
            if (ssc.ServiceServiceChannelDigitalAuthorizations?.Count > 0)
            {
                List<V4VmOpenApiFintoItem> authorizations = new List<V4VmOpenApiFintoItem>(ssc.ServiceServiceChannelDigitalAuthorizations.Count);

                ssc.ServiceServiceChannelDigitalAuthorizations.ForEach(srcAuthorization =>
                {
                    //// With translator
                    //var vmAuthorization = TranslationManagerToVm.Translate<DigitalAuthorization, V4VmOpenApiFintoItem>(srcAuthorization.DigitalAuthorization);

                    //if (vmAuthorization != null)
                    //{
                    //    authorizations.Add(vmAuthorization);
                    //}

                    // Manually map
                    if (srcAuthorization != null && srcAuthorization.DigitalAuthorization != null)
                    {                        
                        authorizations.Add(srcAuthorization.DigitalAuthorization.GetOpenApiModel(languageCache));
                    }
                });

                return authorizations;
            }

            return new List<V4VmOpenApiFintoItem>();
        }

        internal TModel GetContactDetails<TModel>(ServiceServiceChannel connection, ITypesCache typesCache, ILanguageCache languageCache)
            where TModel : IVmOpenApiContactDetailsVersionBase, new()
        {
            //// Translator
            //return TranslationManagerToVm.Translate<ServiceServiceChannel, VmOpenApiContactDetails>(connection);

            // Manually map
            if (connection == null) return default(TModel);

            return GetContactDetails<TModel, ServiceServiceChannelAddress>(connection.ServiceServiceChannelAddresses, connection.ServiceServiceChannelEmails.Select(e => e.Email).ToList(),
                connection.ServiceServiceChannelPhones.Select(p => p.Phone).ToList(), connection.ServiceServiceChannelWebPages.Select(w => w.WebPage).ToList(), typesCache, languageCache);
            
        }

        internal TModel GetContactDetails<TModel, TAddress>(ICollection<TAddress> addresses, ICollection<Email> emails, ICollection<Phone> phones,
            ICollection<WebPage> webPages, ITypesCache typesCache, ILanguageCache languageCache)
            where TModel : IVmOpenApiContactDetailsVersionBase, new() where TAddress : IAddressCharacter
        {            
            var vm = new TModel();
            if (addresses?.Count > 0)
            {
                addresses.OrderBy(a => a.CharacterId).ThenBy(a => a.Address.OrderNumber).ForEach(a =>
                {
                    // TODO - map manually!
                    var vmAddress = TranslationManagerToVm.Translate<Address, V7VmOpenApiAddressContact>(a.Address);                                       
                    vmAddress.Type = typesCache.GetByValue<AddressCharacter>(a.CharacterId);
                    vm.Addresses.Add(vmAddress);
                });
            }

            if (emails?.Count > 0)
            {
                emails.OrderByDescending(e => e.LocalizationId).ThenBy(e => e.OrderNumber).ForEach(e =>
                {
                    if (e != null) { vm.Emails.Add(e.GetOpenApiModel(languageCache)); }
                });
            }

            if (phones?.Count > 0)
            {
                phones.OrderBy(p => p.TypeId).ThenByDescending(p => p.LocalizationId).ThenBy(p => p.OrderNumber).ForEach(p =>
                {
                    if (p != null) { vm.PhoneNumbers.Add(p.GetOpenApiModel(typesCache, languageCache)); }
                });
            }

            if (webPages?.Count > 0)
            {
                webPages.OrderByDescending(w => w.LocalizationId).ThenBy(w => w.OrderNumber).ForEach(w =>
                {
                    if (w != null) { vm.WebPages.Add(w.GetOpenApiModel(languageCache)); }
                });
            }

            if (vm.Addresses?.Count > 0 || vm.Emails?.Count > 0 || vm.PhoneNumbers?.Count > 0 || vm.WebPages?.Count > 0)
            {
                return vm;
            }

            return default(TModel);
        }

        internal List<VmOpenApiLocalizedListItem> GetDescriptions<TEntity>(ICollection<TEntity> descriptions, ITypesCache typesCache, ILanguageCache languageCache) where TEntity : IDescription
        {
            var list = new List<VmOpenApiLocalizedListItem>();
            if (descriptions == null) return list;

            descriptions.ForEach(d => list.Add(new VmOpenApiLocalizedListItem
            {
                Language = languageCache.GetByValue(d.LocalizationId),
                Type = typesCache.GetByValue<DescriptionType>(d.TypeId),
                Value = string.IsNullOrEmpty(d.Description) ? null : d.Description
            }));

            return list;
        }

        /// <summary>
        /// Tries to get name in the following order: FI, SV and then EN
        /// Returns only published service names. (PTV-3689)
        /// </summary>
        /// <param name="names">List of names</param>
        /// <param name="languageIds">List of published languages</param>
        /// <returns>name or null</returns>
        internal string GetNameWithFallback<TName>(ICollection<TName> names, List<Guid> languageIds, ITypesCache typesCache, ILanguageCache languageCache)
            where TName : IName
        {
            if (names == null || names.Count == 0)
            {
                return null;
            }

            if (languageIds == null || languageIds.Count == 0)
            {
                return null;
            }

            Guid nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            string sname = null;
            // first try to get finnish name
            var fiStr = "fi";
            if (languageCache.AllowedLanguageCodes.Contains(fiStr))
            {
                var fiGuid = languageCache.Get(fiStr);
                if (languageIds.Contains(fiGuid))
                {
                    sname = GetName(names, languageCache.Get(fiStr), nameTypeId);
                    // did we find FI name
                    if (!string.IsNullOrWhiteSpace(sname))
                    {
                        return sname;
                    }
                }
            }

            // try to get swedish name
            var svStr = "sv";
            if (languageCache.AllowedLanguageCodes.Contains(fiStr))
            {
                var svGuid = languageCache.Get(svStr);
                if (languageIds.Contains(svGuid))
                {
                    sname = GetName(names, languageCache.Get(svStr), nameTypeId);
                    // did we find SV name
                    if (!string.IsNullOrWhiteSpace(sname))
                    {
                        return sname;
                    }
                }
            }

            // We have not yet found any name for item so let's take the first allowed language available.
            foreach (var allowedLangugageId in languageCache.AllowedLanguageIds)
            {
                if (!languageIds.Contains(allowedLangugageId)) continue;
                sname = GetName(names, allowedLangugageId, nameTypeId);
                // did we find name
                if (!string.IsNullOrWhiteSpace(sname))
                {
                    return sname;
                }
            }

            return sname;
        }

        /// <summary>
        /// Get name with defined language.
        /// </summary>
        /// <param name="names">List of names</param>
        /// <param name="languageId">what language to get</param>
        /// <param name="nameTypeId">what type of name to get</param>
        /// <returns>name or null</returns>
        private static string GetName<TName>(ICollection<TName> names, Guid languageId, Guid nameTypeId)
            where TName : IName
        {
            if (names == null || names.Count == 0)
            {
                return null;
            }

            return names.FirstOrDefault(sn => sn.LocalizationId == languageId && sn.TypeId == nameTypeId)?.Name;
        }

        internal void CheckAddress<TStreetAddress>(IUnitOfWork unitOfWork, IVmOpenApiAddressInBase<TStreetAddress> vm) where TStreetAddress : IVmOpenApiAddressStreetIn
        {
            if (vm.StreetAddress != null && !vm.StreetAddress.PostalCode.IsNullOrEmpty() && vm.StreetAddress.Municipality.IsNullOrEmpty())
            {
                vm.StreetAddress.Municipality = GetMunicipalityCodeByPostalCode(unitOfWork, vm.StreetAddress.PostalCode);
            }
            else if (vm.PostOfficeBoxAddress != null && !vm.PostOfficeBoxAddress.PostalCode.IsNullOrEmpty() && vm.PostOfficeBoxAddress.Municipality.IsNullOrEmpty())
            {
                vm.PostOfficeBoxAddress.Municipality = GetMunicipalityCodeByPostalCode(unitOfWork, vm.PostOfficeBoxAddress.PostalCode);
            }
        }

        private string GetMunicipalityCodeByPostalCode(IUnitOfWork unitOfWork, string code)
        {
            var result = string.Empty;
            var rep = unitOfWork.CreateRepository<IPostalCodeRepository>();
            var postalCodeQry = rep.All().Where(x => x.Code == code && x.IsValid);
            var postalCodes = unitOfWork.ApplyIncludes(postalCodeQry, i => i.Include(j => j.Municipality)).ToList();
            result = (postalCodes?.Count > 0) ? postalCodes.FirstOrDefault(c => c.Municipality != null && c.Municipality.IsValid == true)?.Municipality?.Code : null;
            return result;
        }

        protected Guid? NormalizePublishingStatusId<TEntity>(IUnitOfWork unitOfWork, Guid unificRootId,
            Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            // if it is modified version and exist published one skip validation info, 
            if (PublishingStatusCache.Get(PublishingStatus.Modified) != publishingStatusId) 
                return publishingStatusId;
            
            var publishedVersion = VersioningManager.GetLastPublishedVersion<TEntity>(unitOfWork, unificRootId);
            if (publishedVersion != null) 
                return PublishingStatusCache.Get(PublishingStatus.Published);

            // if it is modified (after restore from archived) - published version should not exist and validation info should be provided, get configuration for draft
            return PublishingStatusCache.Get(PublishingStatus.Draft);

        }

        protected DateTime? GetLifeTime<TEntity>(IUnitOfWork unitOfWork, Guid publishingStatusId)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var entityTypeName = typeof(TEntity).Name.Replace("Versioned", "");
            var configurationRepository = unitOfWork.CreateRepository<IVTasksConfigurationRepository>();
               
            var timeWhenEntityExpires = configurationRepository.All().Where(x =>
                    x.Entity == entityTypeName && x.PublishingStatusId == publishingStatusId)
                .OrderBy(x => x.Interval).FirstOrDefault()?.Interval;

            return timeWhenEntityExpires;
        }

        protected DateTime CalculateExpirationTime(DateTime modifiedDate, DateTime lifeTime, DateTime? utcNow = null)
        {
            if (!utcNow.HasValue)
                utcNow = DateTime.UtcNow;
            
            return utcNow.Value.Add(modifiedDate - lifeTime);
        }

        protected DateTime? GetExpirationTime<TEntity>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null, DateTime? lastChangeDate = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, new()
        {
            var normalizedStatusId = NormalizePublishingStatusId<TEntity>(unitOfWork, entity.UnificRootId, entity.PublishingStatusId);
            if (normalizedStatusId == null) 
                return null;

            var lifeTime = GetLifeTime<TEntity>(unitOfWork, normalizedStatusId.Value);
            if (lifeTime == null)
                return null;
            
            return CalculateExpirationTime(lastChangeDate ?? entity.Modified, lifeTime.Value, utcNow);
        }
    }
}