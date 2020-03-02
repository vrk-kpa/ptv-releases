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

using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiPrintableFormChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiPrintableFormChannelMainInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiPrintableFormChannelInVersionBase>
    {
        public OpenApiPrintableFormChannelMainInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {}

        public override VmOpenApiPrintableFormChannelInVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiPrintableFormChannelInVersionBase vModel)
        {
            var definitions = base.CreateVmToChannelDefinitions(vModel)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()), o => o.TypeId)
                .AddNavigationOneMany(i => i, o => o.PrintableFormChannels);

            if (vModel.DeleteAllAttachments || vModel.Attachments?.Count > 0)
            {
                var orderNumber = 1;
                vModel.Attachments.ForEach(a => a.OrderNumber = orderNumber++);
                definitions.AddCollectionWithRemove(i => i.Attachments, o => o.Attachments, x => true);
            }
            if (vModel.DeleteAllDeliveryAddresses || vModel.DeliveryAddresses != null)
            {
                var orderNumber = 0;
                vModel.DeliveryAddresses?.ForEach(a => a.OrderNumber = ++orderNumber);
                definitions.AddCollectionWithRemove(i => vModel.DeliveryAddresses ?? new List<V8VmOpenApiAddressDeliveryIn>(), o => o.Addresses, x => true);
            }


            return definitions.GetFinal();
        }
    }
}
