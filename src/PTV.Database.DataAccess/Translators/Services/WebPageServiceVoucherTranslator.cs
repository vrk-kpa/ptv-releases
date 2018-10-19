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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<WebPage, VmServiceVoucher>), RegisterType.Transient)]
    internal class WebPageServiceVoucherTranslator : Translator<WebPage, VmServiceVoucher>
    {
        public WebPageServiceVoucherTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmServiceVoucher TranslateEntityToVm(WebPage entity)
        {
            return CreateEntityViewModelDefinition<VmServiceVoucher>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Url, o => o.UrlAddress)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.AdditionalInformation)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .GetFinal();
        }

        public override WebPage TranslateVmToEntity(VmServiceVoucher vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            var exists = vModel.Id.IsAssigned();
            var translationDefinitions = CreateViewModelEntityDefinition<WebPage>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .AddNavigation(i => i.UrlAddress, o => o.Url)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddSimple(i => i.LocalizationId.Value, o => o.LocalizationId)
                .AddNavigation(i => i.AdditionalInformation, o => o.Description)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable);

            return translationDefinitions.GetFinal();
        }
    }
}
