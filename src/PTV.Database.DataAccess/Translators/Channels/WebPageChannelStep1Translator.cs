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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmWebPageChannelStep1>), RegisterType.Transient)]
    internal class WebPageChannelStep1Translator : Translator<ServiceChannelVersioned, VmWebPageChannelStep1>
    {
        private ILanguageCache languageCache;
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private readonly ITypesCache typesCache;

        public WebPageChannelStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmWebPageChannelStep1 TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                // .AddSimpleList(i=>i.Languages.Select(x => x.LanguageId), o => o.Languages)
                .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatusId)
                .AddLocalizable(i => i.WebpageChannels?.FirstOrDefault()?.LocalizedUrls, o => o.WebPage)
                .AddSimple(i => i.ConnectionTypeId, o => o.ConnectionTypeId)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(input => input.LanguageAvailabilities, o => o.LanguagesAvailabilities);

            definitionHelper.AddEmailsDefinition(definition, RequestLanguageCode);
            definitionHelper.AddPhonesDefinition(definition, RequestLanguageCode);
            definitionHelper.AddLanguagesDefinition(definition);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmWebPageChannelStep1 vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(o => o)
                .AddNavigationOneMany(i => i, o => o.WebpageChannels)
                .AddNavigation(i => ServiceChannelTypeEnum.WebPage.ToString(), o => o.Type);
            // .AddCollection(i => i.Languages.Select(x => new VmListItem { Id = x}), o => o.Languages);

            definition.AddSimple(i => i.ConnectionTypeId.IsAssigned()
                    // ReSharper disable once PossibleInvalidOperationException
                    ? i.ConnectionTypeId.Value
                    : typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString())
                , o => o.ConnectionTypeId);

            definitionHelper.AddEmailsDefinition(definition);
            definitionHelper.AddPhonesDefinition(definition);
            definitionHelper.AddLanguagesDefinition(definition);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }
    }
}