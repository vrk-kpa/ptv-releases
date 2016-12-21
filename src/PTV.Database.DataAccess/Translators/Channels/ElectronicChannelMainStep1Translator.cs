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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, VmElectronicChannelStep1>), RegisterType.Transient)]
    internal class ElectronicChannelMainStep1Translator : Translator<ServiceChannel, VmElectronicChannelStep1>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;

        public ElectronicChannelMainStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmElectronicChannelStep1 TranslateEntityToVm(ServiceChannel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatus)
                .AddSimple(input => input.Id, outupt => outupt.Id)
                .AddLocalizable(input => input.ElectronicChannels?.FirstOrDefault()?.LocalizedUrls, output => output.WebPage)
                .AddSimple(input => input.ElectronicChannels?.FirstOrDefault()?.RequiresSignature ?? false, output => output.IsOnLineSign)
                .AddSimple(input => input.ElectronicChannels?.FirstOrDefault()?.RequiresAuthentication ?? false, output => output.IsOnLineAuthentication)
                .AddSimple(input => input.ElectronicChannels?.FirstOrDefault()?.SignatureQuantity ?? 0, output => output.NumberOfSigns);

            definitionHelper.AddEmailDefinition(definition);
            definitionHelper.AddPhoneDefinition(definition);
            definitionHelper.AddAttachmentsDefinition(definition, RequestLanguageCode);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmElectronicChannelStep1 vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddNavigationOneMany(i => i, o => o.ElectronicChannels)
                .AddNavigation(i => ServiceChannelTypeEnum.EChannel.ToString(), o => o.Type);
            definitionHelper.AddEmailDefinition(definition);
            definitionHelper.AddPhoneDefinition(definition);
            definitionHelper.AddAttachmentsDefinition(definition, vModel, vModel.Id);
            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }
    }
}
