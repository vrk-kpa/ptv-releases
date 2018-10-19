﻿using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmPrintableFormOutput>), RegisterType.Transient)]
    internal class PrintableFormOutputMainTranslator : Translator<ServiceChannelVersioned, VmPrintableFormOutput>
    {
        private readonly ILanguageCache languageCache;
        private readonly ServiceChannelTranslationDefinitionHelper definitionHelper;
        private readonly EntityDefinitionHelper entityDefinitionHelper;

        public PrintableFormOutputMainTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManger,
            ServiceChannelTranslationDefinitionHelper definitionHelper,
            EntityDefinitionHelper entityHelper
        ) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManger.LanguageCache;
            this.definitionHelper = definitionHelper;
            entityDefinitionHelper = entityHelper;
        }

        public override VmPrintableFormOutput TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.Addresses, i => i.DeliveryAddresses)
                .AddDictionary(
                    input => input.PrintableFormChannels.FirstOrDefault()?.FormIdentifiers.Select(x => x),
                    output => output.FormIdentifier,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddDictionaryList(
                    input => input.PrintableFormChannels.FirstOrDefault()?.ChannelUrls,
                    output => output.FormFiles,
                    k => languageCache.GetByValue(k.LocalizationId)
                );
            definitionHelper
                .AddAttachmentsDefinition(definition, entity)
                .AddChannelBaseDefinition(definition);

            definitionHelper.AddOrderedDictionaryList(
                definition,
                input => input.PrintableFormChannels.FirstOrDefault()?.ChannelUrls,
                o => o.FormFiles,
                k => languageCache.GetByValue(k.LocalizationId)
            );
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmPrintableFormOutput vModel)
        {
            throw new NotImplementedException();
        }
    }
}
