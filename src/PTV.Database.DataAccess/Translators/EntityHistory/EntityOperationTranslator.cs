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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.EntityHistory
{
    [RegisterService(typeof(ITranslator<Versioning, VmEntityOperation>), RegisterType.Transient)]
    internal class EntityOperationTranslator : Translator<Versioning, VmEntityOperation>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly List<HistoryAction> archiveActions = new List<HistoryAction>
        {
            HistoryAction.Delete,
            HistoryAction.OldPublished,
            HistoryAction.Expired,
            HistoryAction.MassArchive,
            HistoryAction.ArchivedViaOrganization,
            HistoryAction.ArchivedViaScheduling
        };

    public EntityOperationTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            typesCache = cacheManager.TypesCache;
            languageOrderCache = cacheManager.LanguageOrderCache;
        }

        private PublishingStatus GetOperationStatusId (Versioning versioning)
        {
            if (versioning.VersionMinor == 0)
            {
                return PublishingStatus.Published;
            }
            return PublishingStatus.Draft;
        }

        public override VmEntityOperation TranslateEntityToVm(Versioning entity)
        {
            if (entity?.Ignored == true) return null;
            var meta = GetMetaData(entity);
            var translation = CreateEntityViewModelDefinition<VmEntityOperation>(entity)
                .AddSimple(i=>i.Created, o=>o.Created)
                .AddSimple(GetOperationStatusId, o => o.PublishingStatus)
                .AddSimple(i => meta?.HistoryAction ?? HistoryAction.Save, o => o.HistoryAction)
                .AddSimple(i => meta?.TemplateId, o => o.TemplateId)
                .AddSimple(i => meta?.TemplateOrganizationId, o => o.TemplateOrganizationId)
                .AddSimple(i => meta?.SourceLanguageId, o => o.SourceLanguageId)
                .AddNavigation(i => meta?.ExpirationPeriod, o => o.ExpirationPeriod)
                .AddSimple(i => meta != null && archiveActions.Contains(meta.HistoryAction), o => o.ShowLink)
                // HACK: cannot convert to List<Guid>, so using List<string> instead
                .AddCollection(i => meta?.TargetLanguageIds?.Select(x => x.ToString()) ?? new List<string>(), o => o.TargetLanguageIds);

            AddEntityData(entity, translation);
            
            var resultModel =  translation.GetFinal();
            resultModel.SubOperations = CreateSubOperations(entity, resultModel).Where(i => i != null).OrderByDescending(x=>x.Created).ToList();
            return resultModel;
        }

        private void AddEntityData(Versioning input, ITranslationDefinitions<Versioning, VmEntityOperation> translation)
        {
            if (input.Services != null && input.Services.Count != 0)
            {
                var entity = input.Services.First();
                translation.AddCollection(
                    i => GetLanguageAvailabilities<ServiceVersioned, ServiceLanguageAvailability>(i, entity),
                    o => o.LanguagesAvailabilities);
                translation.AddDictionary(i => entity.ServiceNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), 
                    o => o.Name,
                    name => languageCache.GetByValue(name.LocalizationId));
                translation.AddSimple(i => EntityTypeEnum.Service, o => o.EntityType);
                translation.AddNavigation(i => entity.TypeId == null 
                        ? EntityTypeEnum.Service.ToString() 
                        : typesCache.GetByValue<ServiceType>(entity.TypeId.Value), 
                    o => o.SubEntityType);
                translation.AddSimple(i => entity.Id, o => o.EntityId);
            }
            else if (input.Channels != null && input.Channels.Count != 0)
            {
                var entity = input.Channels.First();
                translation.AddCollection(
                    i => GetLanguageAvailabilities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(i, entity),
                    o => o.LanguagesAvailabilities);
                translation.AddDictionary(i => entity.ServiceChannelNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), 
                    o => o.Name,
                    name => languageCache.GetByValue(name.LocalizationId));
                translation.AddSimple(i => EntityTypeEnum.Channel, o => o.EntityType);
                translation.AddNavigation(i => typesCache.GetByValue<ServiceChannelType>(entity.TypeId), o => o.SubEntityType);
                translation.AddSimple(i => entity.Id, o => o.EntityId);
            }
            else if (input.Organizations != null && input.Organizations.Count != 0)
            {
                var entity = input.Organizations.First();
                translation.AddCollection(
                    i => GetLanguageAvailabilities<OrganizationVersioned, OrganizationLanguageAvailability>(i, entity),
                    o => o.LanguagesAvailabilities);
                translation.AddDictionary(i => entity.OrganizationNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), 
                    o => o.Name,
                    name => languageCache.GetByValue(name.LocalizationId));
                translation.AddSimple(i => EntityTypeEnum.Organization, o => o.EntityType);
                translation.AddNavigation(i => EntityTypeEnum.Organization.ToString(), o => o.SubEntityType);
                translation.AddSimple(i => entity.Id, o => o.EntityId);
            }
            else if (input.ServiceCollections != null && input.ServiceCollections.Count != 0)
            {
                var entity = input.ServiceCollections.First();
                translation.AddCollection(
                    i => GetLanguageAvailabilities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(i, entity),
                    o => o.LanguagesAvailabilities);
                translation.AddDictionary(i => entity.ServiceCollectionNames
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), 
                    o => o.Name,
                    name => languageCache.GetByValue(name.LocalizationId));
                translation.AddSimple(i => EntityTypeEnum.ServiceCollection, o => o.EntityType);
                translation.AddNavigation(i => EntityTypeEnum.ServiceCollection.ToString(), o => o.SubEntityType);
                translation.AddSimple(i => entity.Id, o => o.EntityId);
            }
            else if (input.GeneralDescriptions != null && input.GeneralDescriptions.Count != 0)
            {
                var entity = input.GeneralDescriptions.First();
                translation.AddCollection(
                    i => GetLanguageAvailabilities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(i, entity),
                    o => o.LanguagesAvailabilities);
                translation.AddDictionary(i => entity.Names
                        .Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), 
                    o => o.Name,
                    name => languageCache.GetByValue(name.LocalizationId));
                translation.AddSimple(i => EntityTypeEnum.GeneralDescription, o => o.EntityType);
                translation.AddNavigation(i => EntityTypeEnum.GeneralDescription.ToString(), o => o.SubEntityType);
                translation.AddSimple(i => entity.Id, o => o.EntityId);
            }
        }

        private IEnumerable<ILanguageAvailability> GetLanguageAvailabilities<T, TLanguageAvail>(Versioning versioning, T entity)
            where TLanguageAvail : class, ILanguageAvailability, new()
            where T : IMultilanguagedEntity<TLanguageAvail>
        {
            IEnumerable<TLanguageAvail> result = null;
            var deletedPs = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            if (versioning.Meta != null)
            {
                var meta = GetMetaData(versioning);
                if (meta != null)
                {
                    var langs = meta.LanguagesMetaData.Select(
                        lang => new TLanguageAvail
                        {
                            LanguageId = lang.LanguageId,
                            StatusId = deletedPs == meta.EntityStatusId
                                ? deletedPs
                                : lang.EntityStatusId
                        }).ToList();
                    result = langs.OrderBy(x => languageOrderCache.Get(x.LanguageId));
                }
            }

            return result ?? entity.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId));
        }

        private VmHistoryMetaData GetMetaData(Versioning entity)
        {
            return entity?.Meta == null ? null : JsonConvert.DeserializeObject<VmHistoryMetaData>(entity.Meta);
        }

        private List<VmEntityOperation> CreateSubOperations(Versioning entity, VmEntityOperation model)
        {
            var result = new List<VmEntityOperation>();
            if (entity.Meta == null || entity?.Ignored == true) return result;
            var meta = GetMetaData(entity);
            meta.LanguagesMetaData
                .Where(x=>x.Reviewed.HasValue && x.PublishedAt.HasValue)
                .GroupBy(x=>x.Reviewed.Value.ToString("F") + x.PublishedAt.Value.ToString("D"))
                .ForEach(langs => CreateScheduledPublishSubOperation(entity, model, langs, result));
            meta.LanguagesMetaData
                .Where(x=>x.Archived.HasValue && x.ArchivedAt.HasValue)
                .GroupBy(x=>x.Archived.Value.ToString("F") + x.ArchivedAt.Value.ToString("D"))
                .ForEach(langs => CreateScheduledArchiveSubOperation(entity, model, langs, result));
            return result;
        }

        private void CreateScheduledArchiveSubOperation(Versioning entity, VmEntityOperation model, IGrouping<string, VmHistoryMetaDataLanguage> langs,
            List<VmEntityOperation> result)
        {
            var lang = langs.First();
            var newLanguages = new List<VmLanguageAvailabilityInfo>();
            model.LanguagesAvailabilities.ForEach(x =>
            {
                var scheduledLanguage = CreateSubOperationLanguageAvailability(x, x.ValidFrom, null);
                if (langs.Select(l => l.LanguageId).Contains(x.LanguageId))
                {
                    scheduledLanguage.ValidTo = lang.ArchivedAt.Value.ToEpochTime();
                }

                newLanguages.Add(scheduledLanguage);
            });
            if (lang.ArchivedAt.HasValue)
            {
                result.Add(new VmEntityOperation
                {
                    VersionMajor = entity.VersionMajor,
                    VersionMinor = entity.VersionMinor,
                    PublishingStatus = GetOperationStatusId(entity),
                    HistoryAction = HistoryAction.ScheduledArchive,
                    Id = (entity.Id.ToString() + lang.LanguageId.ToString() + HistoryAction.ScheduledArchive).GetGuid(),
                    Created = lang.Archived.Value.ToEpochTime(),
                    CreatedBy = lang.ArchivedBy,
                    ActionDate = lang.ArchivedAt.Value.ToEpochTime(),
                    LanguagesAvailabilities = newLanguages,
                    Name = model.Name,
                    EntityType = model.EntityType,
                    SubEntityType = model.SubEntityType,
                    EntityId = model.EntityId,
                    TargetLanguageIds = newLanguages.Where(x => x.ValidTo.HasValue).Select(x => x.LanguageId.ToString())
                        .ToList()
                });
            }
        }

        private void CreateScheduledPublishSubOperation(Versioning entity, VmEntityOperation model, IGrouping<string, VmHistoryMetaDataLanguage> langs,
            List<VmEntityOperation> result)
        {
            var lang = langs.First();
            var newLanguages = new List<VmLanguageAvailabilityInfo>();
            model.LanguagesAvailabilities.ForEach(x =>
            {
                var scheduledLanguage = CreateSubOperationLanguageAvailability(x, null, x.ValidTo);
                if (langs.Select(l => l.LanguageId).Contains(x.LanguageId))
                {
                    scheduledLanguage.ValidFrom = lang.PublishedAt.Value.ToEpochTime();
                }

                newLanguages.Add(scheduledLanguage);
            });
            if (lang.PublishedAt.HasValue)
            {
                result.Add(new VmEntityOperation
                {
                    PublishingStatus = GetOperationStatusId(entity),
                    HistoryAction = HistoryAction.ScheduledPublish,
                    Id = (entity.Id.ToString() + lang.LanguageId.ToString() + HistoryAction.ScheduledPublish).GetGuid(),
                    Created = lang.Reviewed.Value.ToEpochTime(),
                    CreatedBy = lang.ReviewedBy,
                    ActionDate = lang.PublishedAt.Value.ToEpochTime(),
                    LanguagesAvailabilities = newLanguages,
                    Name = model.Name,
                    EntityType = model.EntityType,
                    SubEntityType = model.SubEntityType,
                    EntityId = model.EntityId,
                    TargetLanguageIds = newLanguages.Where(x => x.ValidFrom.HasValue).Select(x => x.LanguageId.ToString())
                        .ToList()
                });
            }
        }

        public VmLanguageAvailabilityInfo CreateSubOperationLanguageAvailability(VmLanguageAvailabilityInfo parent,
            long? validFrom, long? validTo)
        {
            return new VmLanguageAvailabilityInfo
            {
                LanguageId = parent.LanguageId,
                Modified = parent.Modified,
                StatusId = parent.StatusId,
                ModifiedBy = parent.ModifiedBy,
                ValidTo = validTo,
                ValidFrom =  validFrom
            };
        }

        public override Versioning TranslateVmToEntity(VmEntityOperation vModel)
        {
            throw new NotImplementedException();
        }
    }
}
