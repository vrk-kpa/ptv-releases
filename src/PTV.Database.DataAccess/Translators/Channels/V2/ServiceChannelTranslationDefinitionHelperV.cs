using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using IAttachments = PTV.Domain.Model.Models.Interfaces.V2.IAttachments;
using VmServiceChannel = PTV.Domain.Model.Models.V2.Channel.VmServiceChannel;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ServiceChannelTranslationDefinitionHelper), RegisterType.Scope)]
    internal class ServiceChannelTranslationDefinitionHelper : EntityDefinitionHelper
    {
        private ILanguageCache languageCache;

        private ITypesCache typesCache;

        public ServiceChannelTranslationDefinitionHelper(ICacheManager cacheManager, ITranslationEntity translationToVm)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

//
//        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddWebPagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition, IWebPages model, Guid? referenceId) where TIn : IWebPages
//        {
//            model.WebPages?.ForEach(i => i.OwnerReferenceId = referenceId);
//            return definition.AddCollection(i => i.WebPages, o => o.WebPages, true);
//        }
//
//        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddWebPagesDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IWebPages
//        {
//            return definition.AddCollection(i => i.WebPages.Where(x => languageCache.Get(languageCode.ToString()) == x.WebPage.LocalizationId), o => o.WebPages);
//        }
//
        public ServiceChannelTranslationDefinitionHelper AddAttachmentsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition, IAttachments model, Guid? referenceId) where TIn : IAttachments
        {
            definition
                    .AddCollectionWithRemove(input => input.Attachments.SelectMany(pair =>
                    {
                        var localizationId = languageCache.Get(pair.Key);
                        var orderNumber = 0;
                        return pair.Value.Select(at =>
                        {
                            at.OwnerReferenceId = referenceId;
                            at.LocalizationId = localizationId;
                            at.OrderNumber = orderNumber++;
                            return at;
                        });
                    }), output => output.Attachments, x => true);
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddAttachmentsDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, ServiceChannelVersioned entity) where TOut : IAttachments
        {
            definition.AddDictionaryList(i => i.Attachments.Select(x => x.Attachment).OrderBy(x=>x.OrderNumber).ThenBy(x=>x.Created), o => o.Attachments,
                x => languageCache.GetByValue(x.LocalizationId));
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddChannelBaseDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : VmServiceChannel
        {
            definition.AddPartial<VmServiceChannel>(input => input);
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddChannelBaseDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : VmServiceChannel
        {
            definition.AddPartial<ServiceChannelVersioned, VmServiceChannel>(input => input, output => output);
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddOpeningHoursDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : IOpeningHours
        {
            definition.AddPartial(input => input.OpeningHours);
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddOpeningHoursDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : IOpeningHours
        {
            definition.AddNavigation(input => input, output => output.OpeningHours);
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddLanguagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : ILanguages
        {
            var languageCounter = 0;
            definition.AddCollection(
                i => i.Languages?.Select(x => new VmListItem {
                    Id = x,
                    OrderNumber = languageCounter++,
                    OwnerReferenceId = i.Id
                }),
                o => o.Languages, TranslationPolicy.FetchData
            );
            return this;
        }

        public ServiceChannelTranslationDefinitionHelper AddLanguagesDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : ILanguages
        {
            definition.AddSimpleList(
                input => input.Languages
                    .OrderBy(x => x.Order)
                    .Select(x => x.LanguageId),
                output => output.Languages
            );
            return this;
        }


//        public ITranslationDefinitions<TIn, ServiceChannelServiceHours> AddOpeningHoursDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelServiceHours> definition) where TIn : VmHours
//        {
//            return definition.AddPartial<VmHours>(input => input);
//        }
//
//        public ITranslationDefinitions<ServiceChannelServiceHours, TOut> AddOpeningHoursDefinition<TOut>(ITranslationDefinitions<ServiceChannelServiceHours, TOut> definition) where TOut : VmHours
//        {
//            return definition.AddNavigation<ServiceChannelServiceHours, VmHours>(input => input, output => output, true);
//        }
    }
}