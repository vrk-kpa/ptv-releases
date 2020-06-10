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
using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;
using VmPhoneChannel = PTV.Domain.Model.Models.V2.Channel.VmPhoneChannel;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Phone
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmPhoneChannel>), RegisterType.Transient)]
    internal class PhoneChannelMainTranslator : Translator<ServiceChannelVersioned, VmPhoneChannel>
    {
        private readonly ServiceChannelTranslationDefinitionHelper definitionHelper;

        public PhoneChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper) 
            : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmPhoneChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddDictionary(
                    input => input.WebPages,
                    output => output.WebPage,
                    key => languageCache.GetByValue(key.LocalizationId));

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmPhoneChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .DefineEntitySubTree(i => i.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours))
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddNavigation(i => ServiceChannelTypeEnum.Phone.ToString(), o => o.Type)
                    .AddCollection(i => i.WebPage?.Where(pair => !pair.Value.UrlAddress.IsNullOrWhitespace())
                        .Select(pair => new VmWebPage
                        {
                            Id = pair.Value.Id,
                            WebPageId = pair.Value.WebPageId,
                            UrlAddress = pair.Value.UrlAddress,
                            LocalizationId = languageCache.Get(pair.Key)
                        }), o => o.WebPages);
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

    }
}
