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

using System.Security.Policy;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceWebPage, VmServiceVoucher>), RegisterType.Transient)]
    internal class ServiceWebPageServiceVoucherTranslator : Translator<ServiceWebPage, VmServiceVoucher>
    {
        public ServiceWebPageServiceVoucherTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override VmServiceVoucher TranslateEntityToVm(ServiceWebPage entity)
        {
            return CreateEntityViewModelDefinition<VmServiceVoucher>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddPartial(i => i.WebPage, o => o as VmUrl)
                .AddSimple(i => i.LocalizationId, o => o.LocalizationId)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.AdditionalInformation)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();
        }

        public override ServiceWebPage TranslateVmToEntity(VmServiceVoucher vModel)
        {
            var exists = vModel.Id.IsAssigned();
            var languageId = vModel.LocalizationId.IsAssigned()
                ? vModel.LocalizationId.Value
                : RequestLanguageId;

            return CreateViewModelEntityDefinition<ServiceWebPage>(vModel)
                .UseDataContextCreate(x => !exists)
                .UseDataContextUpdate(x => exists, i => o => i.Id == o.Id)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddNavigation(i => i.UrlAddress ?? string.Empty, o => o.WebPage)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.AdditionalInformation, o => o.Description)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }
    }
}
