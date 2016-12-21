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
    [RegisterService(typeof(ITranslator<ServiceChannel, VmPrintableFormChannelStep1>), RegisterType.Transient)]
    internal class PrintableFormChannelMainStep1Translator : Translator<ServiceChannel, VmPrintableFormChannelStep1>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ITypesCache typesCache;

        public PrintableFormChannelMainStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            this.typesCache = typesCache;
        }

        public override VmPrintableFormChannelStep1 TranslateEntityToVm(ServiceChannel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatus)
                .AddPartial(i => i.PrintableFormChannels?.FirstOrDefault());

            definitionHelper.AddPhoneDefinition(definition);
            definitionHelper.AddEmailDefinition(definition);
            definitionHelper.AddAttachmentsDefinition(definition, RequestLanguageCode);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmPrintableFormChannelStep1 vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceChannel>(vModel)
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id);

            definitionHelper.AddEmailDefinition(transaltionDefinition);
            definitionHelper.AddPhoneDefinition(transaltionDefinition);
            SetStep1Translation(transaltionDefinition, vModel);
            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

        private void SetStep1Translation(ITranslationDefinitions<VmPrintableFormChannelStep1, ServiceChannel> definition, VmPrintableFormChannelStep1 model)
        {
            definition
                .AddNavigationOneMany(i => i, o => o.PrintableFormChannels)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()), o => o.TypeId);
            definitionHelper.AddChannelDescriptionsDefinition(definition);
            definitionHelper.AddAttachmentsDefinition(definition, model, model.Id);
        }
    }
}