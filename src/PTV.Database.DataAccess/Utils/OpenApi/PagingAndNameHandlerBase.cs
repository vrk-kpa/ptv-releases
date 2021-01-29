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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Extensions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal abstract class PagingAndNameHandlerBase<TModel, TItemModel, TEntity, TName, TLanguageAvailability> : EntityPagingHandlerBase<TModel, TItemModel, TEntity>
        where TModel : IVmOpenApiModelWithPagingBase<TItemModel>, new()
        where TItemModel : IVmOpenApiItemBase, new()
        where TEntity : class, IEntityIdentifier, IAuditing
        where TName : IName
        where TLanguageAvailability : ILanguageAvailabilityBase
    {
        protected ITypesCache TypesCache;

        protected Dictionary<Guid, Dictionary<Guid, string>> Names { get; set; }

        public PagingAndNameHandlerBase(
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(pageNumber, pageSize)
        {
            TypesCache = typesCache;
        }

        protected void SetNames(IUnitOfWork unitOfWork, bool getPublished = false, Guid? publishedId = null)
        {
            var names = GetNames(unitOfWork)
                .GroupBy(i => i.OwnerReferenceId.Value)
                .ToDictionary(i => i.Key, i => i.ToDictionary(x => x.LocalizationId.Value, x => x.Name));

            // Do we need to filter out unpublished names? (PTV-3689)
            if (getPublished)
            {
                if (!publishedId.IsAssigned())
                {
                    throw new ArgumentNullException("publishedId");
                }

                var languageAvailabilities = GetLanguageAvailabilities(unitOfWork);
                if (languageAvailabilities != null)
                {
                    var publishedNames = new Dictionary<Guid, Dictionary<Guid, string>>();
                    names.ForEach(name =>
                    {
                        var publishedLanguageIds = languageAvailabilities.TryGetOrDefault(name.Key, new List<TLanguageAvailability>()).Where(l => l.StatusId == publishedId).Select(l => l.LanguageId).ToList();
                        publishedNames.Add(name.Key, name.Value.Where(n => publishedLanguageIds.Contains(n.Key)).ToDictionary(i => i.Key, i => i.Value));
                    });
                    Names = publishedNames;
                }
            }
            else
            {
                Names = names;
            }
        }

        private List<VmName> GetNames(IUnitOfWork unitOfWork)
        {
            var nameTypeId = TypesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            if (typeof(TName) == typeof(ServiceName))
            {
                return unitOfWork.CreateRepository<IRepository<ServiceName>>().All().Where(x => EntityIds.Contains(x.ServiceVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new VmName { OwnerReferenceId = i.ServiceVersionedId, Name = i.Name, LocalizationId = i.LocalizationId }).ToList();
            }

            if(typeof(TName) == typeof(ServiceChannelName))
            {
                return unitOfWork.CreateRepository<IRepository<ServiceChannelName>>().All().Where(x => EntityIds.Contains(x.ServiceChannelVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new VmName { OwnerReferenceId = i.ServiceChannelVersionedId, Name = i.Name, LocalizationId = i.LocalizationId }).ToList();
            }

            if (typeof(TName) == typeof(OrganizationName))
            {
                return unitOfWork.CreateRepository<IRepository<OrganizationName>>().All().Where(x => EntityIds.Contains(x.OrganizationVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new VmName { OwnerReferenceId = i.OrganizationVersionedId, Name = i.Name, LocalizationId = i.LocalizationId }).ToList();
            }

            if (typeof(TName) == typeof(StatutoryServiceName))
            {
                return unitOfWork.CreateRepository<IRepository<StatutoryServiceName>>().All().Where(x => EntityIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new VmName { OwnerReferenceId = i.StatutoryServiceGeneralDescriptionVersionedId, Name = i.Name, LocalizationId = i.LocalizationId }).ToList();
            }

            if (typeof(TName) == typeof(ServiceCollectionName))
            {
                return unitOfWork.CreateRepository<IRepository<ServiceCollectionName>>().All().Where(x => EntityIds.Contains(x.ServiceCollectionVersionedId) && x.TypeId == nameTypeId).OrderBy(i => i.Localization.OrderNumber)
                    .Select(i => new VmName { OwnerReferenceId = i.ServiceCollectionVersionedId, Name = i.Name, LocalizationId = i.LocalizationId }).ToList();
            }

            return new List<VmName>();
        }

        protected Dictionary<Guid, List<TLanguageAvailability>> GetLanguageAvailabilities(IUnitOfWork unitOfWork)
        {
            var langAvailabilitiesRep = unitOfWork.CreateRepository<IRepository<TLanguageAvailability>>();
            return langAvailabilitiesRep.All().WithEntityId(EntityIds).ToList()
                .GroupBy(i => i.Id).ToDictionary(i => i.Key, i => i.ToList());
        }
    }
}
