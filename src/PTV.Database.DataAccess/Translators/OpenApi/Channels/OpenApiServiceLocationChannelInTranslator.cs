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
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannel, VmOpenApiServiceLocationChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelInTranslator : Translator<ServiceLocationChannel, VmOpenApiServiceLocationChannelInVersionBase>
    {
        public OpenApiServiceLocationChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}
        public override VmOpenApiServiceLocationChannelInVersionBase TranslateEntityToVm(ServiceLocationChannel entity)
        {
            throw new NotImplementedException();
        }
        public override ServiceLocationChannel TranslateVmToEntity(VmOpenApiServiceLocationChannelInVersionBase vModel)
        {
            var definition = CreateViewModelEntityDefinition<ServiceLocationChannel>(vModel)
               .DisableAutoTranslation()
               .UseDataContextCreate(i => !i.Id.IsAssigned())
               .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelVersionedId, def => def.UseDataContextCreate(i => true));

            // Set addresses
            if (vModel.Addresses != null && vModel.Addresses.Count > 0)
            {
                var order = 1;
                vModel.Addresses.ForEach(item => item.OrderNumber = order++);
                definition.AddCollection(i => i.Addresses, o => o.Addresses, false);
            }
            return definition.GetFinal();
        }
    }
}
