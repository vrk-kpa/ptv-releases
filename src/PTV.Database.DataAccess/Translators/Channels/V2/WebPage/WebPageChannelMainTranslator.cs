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
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;

namespace PTV.Database.DataAccess.Translators.Channels.V2.WebPage
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmWebPageChannel>), RegisterType.Transient)]
    internal class WebPageChannelMainTranslator : Translator<ServiceChannelVersioned, VmWebPageChannel>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;

        public WebPageChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmWebPageChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input.WebpageChannels?.FirstOrDefault(), output => output);

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddAccessibilityClassificationsDefinition(definition, entity);
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmWebPageChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddSimple(i => definitionHelper.CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()), o => o.TypeId)
                    .AddNavigationOneMany(i => i, o => o.WebpageChannels)
                ;
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddAccessibilityClassificationDefinition(definition, vModel, vModel.Id, vModel.LanguagesAvailabilities.Select(x => x.LanguageId).ToList());
            return definition.GetFinal();
        }

    }
}
