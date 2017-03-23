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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<OrganizationServiceAdditionalInformation, VmServiceProducerDetail>), RegisterType.Transient)]
    internal class OrganizationServiceAdditionalInformationTranslator : Translator<OrganizationServiceAdditionalInformation, VmServiceProducerDetail>
    {
        public OrganizationServiceAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmServiceProducerDetail TranslateEntityToVm(OrganizationServiceAdditionalInformation entity)
        {
            return CreateEntityViewModelDefinition<VmServiceProducerDetail>(entity)
                .AddNavigation(input => input.Text, output => output.FreeDescription)
                .GetFinal();
        }

        public override OrganizationServiceAdditionalInformation TranslateVmToEntity(VmServiceProducerDetail vModel)
        {
            return CreateViewModelEntityDefinition<OrganizationServiceAdditionalInformation>(vModel)
                .UseDataContextLocalizedUpdate(input => input.OwnerReferenceId.IsAssigned(),
                        input => output => input.OwnerReferenceId == output.OrganizationServiceId,
                        definition => definition.UseDataContextCreate(input => true, output => output.Id, input => Guid.NewGuid()))
                .UseDataContextCreate(input => !input.OwnerReferenceId.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .AddNavigation(input => input.FreeDescription, output => output.Text)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationServiceAdditionalInformation, string>), RegisterType.Transient)]
    internal class OrganizationServiceAdditionalInformationStringTranslator : Translator<OrganizationServiceAdditionalInformation, string>
    {
        public OrganizationServiceAdditionalInformationStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(OrganizationServiceAdditionalInformation entity)
        {
            return CreateEntityViewModelDefinition(entity)
               .AddNavigation(i => i.Text, o => o)
               .GetFinal();
        }

        public override OrganizationServiceAdditionalInformation TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
