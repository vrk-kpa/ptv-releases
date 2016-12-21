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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, VmPhoneChannelStep1>), RegisterType.Transient)]
    internal class PhoneChannelMainStep1Translator : Translator<ServiceChannel, VmPhoneChannelStep1>
    {
        private ILanguageCache languageCache;
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ITypesCache typesCache;

        public PhoneChannelMainStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache, ServiceChannelTranslationDefinitionHelper definitionHelper, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
            this.definitionHelper = definitionHelper;
            this.typesCache = typesCache;
        }

        public override VmPhoneChannelStep1 TranslateEntityToVm(ServiceChannel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddLocalizable(input => input.Phones.Select(x => x.Phone), output => output.PhoneNumber)
                .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatus)
                .AddLocalizable(i => i.WebPages.Select(x=>x.WebPage), o => o.WebPage);

            definitionHelper.AddLanguagesDefinition(definition);
            definitionHelper.AddEmailDefinition(definition);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmPhoneChannelStep1 vModel)
        {

            if (vModel == null)
            {
                return null;
            }
            vModel.WebPage.SafeCall(i => i.OwnerReferenceId = vModel.Id);
            var webPages = new List<VmWebPage>();
            if (vModel.WebPage?.UrlAddress != null)
            {
                webPages.Add(vModel.WebPage);
            }
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddCollection(input => new List<VmPhone> { vModel.PhoneNumber }, output => output.Phones)
                .AddCollection(i => webPages, o => o.WebPages);
            definitionHelper.AddLanguagesDefinition(definition);
            definitionHelper.AddEmailDefinition(definition);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }
    }
}