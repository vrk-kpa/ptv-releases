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
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Translators.Addresses
{
     [RegisterService(typeof(ITranslator<OrganizationEInvoicingAdditionalInformation, VmLocalizedAdditionalInformation>), RegisterType.Transient)]
     internal class ElectronicInvoicingAddressAdditionalInformationTranslator : Translator<OrganizationEInvoicingAdditionalInformation, VmLocalizedAdditionalInformation>
     {
        public ElectronicInvoicingAddressAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
      
        public override VmLocalizedAdditionalInformation TranslateEntityToVm(OrganizationEInvoicingAdditionalInformation entity)
        {
          throw new NotImplementedException();
        }
      
        public override OrganizationEInvoicingAdditionalInformation TranslateVmToEntity(VmLocalizedAdditionalInformation vModel)
        {
          if (vModel.LocalizationId.IsAssigned())
          {
            SetLanguage(vModel.LocalizationId);
          }
       
          return CreateViewModelEntityDefinition<OrganizationEInvoicingAdditionalInformation>(vModel)
           .UseDataContextCreate(i => !i.OwnerReferenceId.HasValue)
           .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.HasValue, i => o => (i.OwnerReferenceId == o.OrganizationEInvoicingId),
            def => def.UseDataContextCreate(i => i.OwnerReferenceId.IsAssigned()))
           .AddNavigation(i => i.Content, o => o.Text)
           .AddRequestLanguage(output => output)
           .GetFinal();
        }
     }


    [RegisterService(typeof(ITranslator<OrganizationEInvoicingAdditionalInformation, string>), RegisterType.Transient)]
    internal class ElectronicInvoicingAddressAdditionalInformationStringTranslator : Translator<OrganizationEInvoicingAdditionalInformation, string>
    {
        public ElectronicInvoicingAddressAdditionalInformationStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(OrganizationEInvoicingAdditionalInformation entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Text, output => output)
                .GetFinal();
        }

        public override OrganizationEInvoicingAdditionalInformation TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
