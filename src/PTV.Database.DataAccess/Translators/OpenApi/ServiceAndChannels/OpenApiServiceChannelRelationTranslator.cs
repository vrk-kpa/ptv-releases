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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, V2VmOpenApiServiceAndChannel>), RegisterType.Transient)]
    internal class OpenApiServiceChannelRelationTranslator : Translator<ServiceServiceChannel, V2VmOpenApiServiceAndChannel>
    {
        public OpenApiServiceChannelRelationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V2VmOpenApiServiceAndChannel TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(V2VmOpenApiServiceAndChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => true, i => o => i.ChannelGuid == o.ServiceChannelId && i.ServiceGuid == o.ServiceId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.ServiceGuid, o => o.ServiceId)
                .AddSimple(i => i.ChannelGuid, o => o.ServiceChannelId);

            if (!string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                definition.AddNavigation(i => vModel.ServiceChargeType, o => o.ServiceChargeType);
            }

            if (vModel.Description?.Count > 0)
            {
                var entity = definition.GetFinal();
                if(entity.Created != DateTime.MinValue)
                {
                    // We are updating existing entity
                    vModel.Description.ForEach(d =>
                    {
                        d.OwnerReferenceId = vModel.ServiceGuid;
                        d.OwnerReferenceId2 = vModel.ChannelGuid;
                    });
                }
                definition.AddCollection(i => i.Description, o => o.ServiceServiceChannelDescriptions);
            }
            return definition.GetFinal();
        }
    }
}
