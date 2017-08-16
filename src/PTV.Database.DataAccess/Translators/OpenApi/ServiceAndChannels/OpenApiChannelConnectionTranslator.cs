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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, V7VmOpenApiChannelServicesIn>), RegisterType.Transient)]
    internal class OpenApiChannelConnectionTranslator : Translator<ServiceChannel, V7VmOpenApiChannelServicesIn>
    {
        public OpenApiChannelConnectionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V7VmOpenApiChannelServicesIn TranslateEntityToVm(ServiceChannel entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiChannelConnectionTranslator!");
        }

        public override ServiceChannel TranslateVmToEntity(V7VmOpenApiChannelServicesIn vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                    .UseDataContextUpdate(i => true, i => o => i.ChannelId == o.Id);

            if (vModel.DeleteAllServiceRelations || vModel.ServiceRelations?.Count > 0)
            {
                var list = new List<VmOpenApiServiceServiceChannelInVersionBase>();
                vModel.ServiceRelations.ForEach(r => list.Add(r as VmOpenApiServiceServiceChannelInVersionBase));
                definition.AddCollection(i => list, o => o.ServiceServiceChannels, false);

                if (vModel.ServiceRelations?.Count > 0)
                {
                    var extraTypes = new List<VmOpenApiExtraType>();
                    if (vModel.ServiceRelations?.Count > 0)
                    {
                        vModel.ServiceRelations.ForEach(r => extraTypes.AddRange(r.ExtraTypes));
                    }
                    if (extraTypes.Count > 0)
                    {
                        definition.AddCollection(i => extraTypes, o => o.ServiceServiceChannelExtraTypes, false);
                    }
                }
                else
                {
                    definition.AddCollection(i => new List<VmOpenApiExtraType>(), o => o.ServiceServiceChannelExtraTypes, false);
                }
            }
            return definition.GetFinal();
        }
    }
}
