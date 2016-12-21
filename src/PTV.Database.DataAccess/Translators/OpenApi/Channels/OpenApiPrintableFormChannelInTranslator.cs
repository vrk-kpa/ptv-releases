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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannel, IV2VmOpenApiPrintableFormChannelInBase>), RegisterType.Transient)]
    internal class OpenApiPrintableFormChannelInTranslator : Translator<PrintableFormChannel, IV2VmOpenApiPrintableFormChannelInBase>
    {
        public OpenApiPrintableFormChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override IV2VmOpenApiPrintableFormChannelInBase TranslateEntityToVm(PrintableFormChannel entity)
        {
            throw new NotImplementedException();
        }
        public override PrintableFormChannel TranslateVmToEntity(IV2VmOpenApiPrintableFormChannelInBase vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition<PrintableFormChannel>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.ServiceChannelId, def => def.UseDataContextCreate(i => true));

            if (!string.IsNullOrEmpty(vModel.FormIdentifier))
            {
                definition.AddNavigation(i => i.FormIdentifier, o => o.FormIdentifier);
            }

            if (!string.IsNullOrEmpty(vModel.FormReceiver))
            {
                definition.AddNavigation(i => i.FormReceiver, o => o.FormReceiver);
            }

            if (vModel.DeliveryAddress != null)
            {
                if (exists)
                {
                    var entity = definition.GetFinal();
                    if (entity.DeliveryAddressId.IsAssigned())
                    {
                        vModel.DeliveryAddress.Id = entity.DeliveryAddressId.Value;
                    }
                }
                definition.AddNavigation(i => i.DeliveryAddress, o => o.DeliveryAddress);
            }

            if (vModel.ChannelUrls != null && vModel.ChannelUrls.Count > 0)
            {
                definition.AddCollection(i => i.ChannelUrls, o => o.ChannelUrls);
            }

            return definition.GetFinal();
        }
    }
}
