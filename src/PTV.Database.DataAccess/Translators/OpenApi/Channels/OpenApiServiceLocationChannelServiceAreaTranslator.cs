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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannelServiceArea, VmOpenApiMunicipality>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelServiceAreaTranslator : Translator<ServiceLocationChannelServiceArea, VmOpenApiMunicipality>
    {
        public OpenApiServiceLocationChannelServiceAreaTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiMunicipality TranslateEntityToVm(ServiceLocationChannelServiceArea entity)
        {
            return CreateEntityViewModelDefinition(entity)
               .AddNavigation(i => i.Municipality?.Code, o => o.Code)
               .AddCollection(i => i.Municipality?.MunicipalityNames, o => o.Name)
               .GetFinal();
        }

        public override ServiceLocationChannelServiceArea TranslateVmToEntity(VmOpenApiMunicipality vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceLocationChannelServiceAreaTranslator.");
        }
    }
}
