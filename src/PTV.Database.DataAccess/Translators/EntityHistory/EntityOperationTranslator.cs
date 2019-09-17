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
            var resultModel = CreateEntityViewModelDefinition<VmEntityOperation>(entity)
                .AddSimple(i=>i.Created, o=>o.Created)
                .AddCollection(
                    i => {
                        IEnumerable<ILanguageAvailability> result = getLanguageAvailabilities<ServiceLanguageAvailability>(i);
                        if (result == null)
                        {
                            if (i.Services != null && i.Services.Count != 0)
                            {
                                result = i.Services.First().LanguageAvailabilities;
                            }
                            else if (i.Channels != null && i.Channels.Count != 0)
                            {
                                result = i.Channels.First().LanguageAvailabilities;
                            }
                            else if (i.Organizations != null && i.Organizations.Count != 0)
                            {
                                result = i.Organizations.First().LanguageAvailabilities;
                            }
                            else if (i.ServiceCollections != null && i.ServiceCollections.Count != 0)
                            {
                                result = i.ServiceCollections.First().LanguageAvailabilities;
                            }
                            else if (i.GeneralDescriptions != null && i.GeneralDescriptions.Count != 0)
                            {
                                result = i.GeneralDescriptions.First().LanguageAvailabilities;
                            }
                        }

                        return result?.OrderBy(x => languageOrderCache.Get(x.LanguageId));
                    },
                    o => o.LanguagesAvailabilities
                )
                .AddSimple(GetOperationStatusId, o => o.PublishingStatus)
                .AddSimple(i => meta?.HistoryAction ?? HistoryAction.Save, o => o.HistoryAction)
                .AddSimple(i => meta?.TemplateId, o => o.TemplateId)
                .AddSimple(i => meta?.TemplateOrganizationId, o => o.TemplateOrganizationId)
                .AddSimple(i => meta?.SourceLanguageId, o => o.SourceLanguageId)
                .AddNavigation(i => meta?.ExpirationPeriod, o => o.ExpirationPeriod)
                // HACK: cannot convert to List<Guid>, so using List<string> instead
                .AddCollection(i => meta?.TargetLanguageIds?.Select(x => x.ToString()) ?? new List<string>(), o => o.TargetLanguageIds)
                .GetFinal();
            resultModel.SubOperations = CreateSubOperations(entity, resultModel).Where(i => i != null).OrderByDescending(x=>x.Created).ToList();
            return resultModel;
        }

        private IEnumerable<TLanguageAvail> getLanguageAvailabilities<TLanguageAvail>(Versioning entity)
            where TLanguageAvail : class, ILanguageAvailability, new()
        {
            var deletedPs = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            if (entity.Meta != null)
            {
                var meta = GetMetaData(entity);
                if (meta != null)
                {
                    var langs = meta.LanguagesMetaData.Select(
                        lang => new TLanguageAvail()
                        {
                            LanguageId = lang.LanguageId, 
                            StatusId = deletedPs == meta.EntityStatusId 
                                ? deletedPs
                                : lang.EntityStatusId
                        }).ToList();
                    return langs;
                }
            }

            return null;
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
                .ForEach(langs =>
                {
                    var lang = langs.First();
                    var newLanguages = new List<VmLanguageAvailabilityInfo>();
                    model.LanguagesAvailabilities.ForEach(x =>
                    {
                        var scheduledLanguage = new VmLanguageAvailabilityInfo
                        {
                            LanguageId = x.LanguageId,
                            Modified = x.Modified,
                            StatusId = x.StatusId,
                            ModifiedBy = x.ModifiedBy,
                            ValidTo = x.ValidTo
                        };
                        if(langs.Select(l=>l.LanguageId).Contains(x.LanguageId))
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
                            TargetLanguageIds = newLanguages.Where(x=>x.ValidFrom.HasValue).Select(x=>x.LanguageId.ToString()).ToList()
                        });
                    }
                });
                meta.LanguagesMetaData
                .Where(x=>x.Archived.HasValue && x.ArchivedAt.HasValue)
                .GroupBy(x=>x.Archived.Value.ToString("F") + x.ArchivedAt.Value.ToString("D"))                
                .ForEach(langs =>
                {
                    var lang = langs.First();
                    var newLanguages = new List<VmLanguageAvailabilityInfo>();
                    model.LanguagesAvailabilities.ForEach(x =>
                    {
                        var scheduledLanguage = new VmLanguageAvailabilityInfo
                        {
                            LanguageId = x.LanguageId,
                            Modified = x.Modified,
                            StatusId = x.StatusId,
                            ModifiedBy = x.ModifiedBy,
                            ValidFrom = x.ValidFrom
                        };
                        if(langs.Select(l=>l.LanguageId).Contains(x.LanguageId))
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
                            TargetLanguageIds = newLanguages.Where(x=>x.ValidTo.HasValue).Select(x=>x.LanguageId.ToString()).ToList()
                        });
                    }
                });
            return result;
        }

        public override Versioning TranslateVmToEntity(VmEntityOperation vModel)
        {
            throw new NotImplementedException();
        }
    }
}
