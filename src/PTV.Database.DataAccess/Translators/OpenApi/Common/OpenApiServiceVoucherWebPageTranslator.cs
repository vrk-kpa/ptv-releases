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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ServiceWebPage, V9VmOpenApiServiceVoucher>), RegisterType.Transient)]
    internal class OpenApiServiceVoucherWebPageTranslator : Translator<ServiceWebPage, V9VmOpenApiServiceVoucher>
    {

        public OpenApiServiceVoucherWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override V9VmOpenApiServiceVoucher TranslateEntityToVm(ServiceWebPage entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => i.Name, o => o.Value)
                .AddNavigation(i => i.Description, o => o.AdditionalInformation)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .AddSimple(i => i.ServiceVersionedId, o => o.OwnerReferenceId)
                .AddNavigation(i => i.WebPage, o => o.Url)
                .GetFinal();
        }

        public override ServiceWebPage TranslateVmToEntity(V9VmOpenApiServiceVoucher vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => exists,
                    i => o => i.Id == o.Id && i.OwnerReferenceId == o.ServiceVersionedId,
                    e => e.UseDataContextCreate(x => true))
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddNavigation(i => i.Value, o => o.Name)
                .AddNavigation(i => i.Url ?? string.Empty, o => o.WebPage)
                .AddNavigation(i => i.AdditionalInformation, o => o.Description)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId);

            return definition.GetFinal();
        }
    }
}
