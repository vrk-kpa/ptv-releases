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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;

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

        private PublishingStatus GetOprationStatusId (Versioning versioning)
        {
            if (versioning.VersionMinor == 0)
            {
                return PublishingStatus.Published;
            }
            return PublishingStatus.Draft;
        }

        public override VmEntityOperation TranslateEntityToVm(Versioning entity)
        {
            return CreateEntityViewModelDefinition<VmEntityOperation>(entity)
                .AddSimple(i=>i.Created, o=>o.Created)
                .AddCollection(
                    i => {
                        IEnumerable<ILanguageAvailability> result = getLanguageAvailabilities<ServiceLanguageAvailability>(i);
                        if (result == null)
                        {
                            if (i.Services != null && i.Services.Count != 0)
                            {
                                result = i.Services.FirstOrDefault().LanguageAvailabilities;
                            }
                            else if (i.Channels != null && i.Channels.Count != 0)
                            {
                                result = i.Channels.FirstOrDefault().LanguageAvailabilities;
                            }
                            else if (i.Organizations != null && i.Organizations.Count != 0)
                            {
                                result = i.Organizations.FirstOrDefault().LanguageAvailabilities;
                            }
                            else if (i.ServiceCollections != null && i.ServiceCollections.Count != 0)
                            {
                                result = i.ServiceCollections.FirstOrDefault().LanguageAvailabilities;
                            }
                            else if (i.GeneralDescriptions != null && i.GeneralDescriptions.Count != 0)
                            {
                                result = i.GeneralDescriptions.FirstOrDefault().LanguageAvailabilities;
                            }
                        }

                        return result.OrderBy(x => languageOrderCache.Get(x.LanguageId));
                    },
                    o => o.LanguagesAvailabilities
                )
                .AddSimple(GetOprationStatusId, o => o.PublishingStatus)
                .AddSimple(i =>
                {
                    if (entity.Meta == null) return HistoryAction.Save;
                    var meta = GetMetaData(entity);
                    return meta.HistoryAction;

                }, o => o.HistoryAction)
                .GetFinal();
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
            return JsonConvert.DeserializeObject<VmHistoryMetaData>(entity.Meta);
        }

        public override Versioning TranslateVmToEntity(VmEntityOperation vModel)
        {
            throw new NotImplementedException();
        }
    }
}
