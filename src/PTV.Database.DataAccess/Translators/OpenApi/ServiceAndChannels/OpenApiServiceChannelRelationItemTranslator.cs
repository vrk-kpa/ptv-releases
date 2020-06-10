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

using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, V11VmOpenApiServiceServiceChannelAstiInBase>), RegisterType.Transient)]
    internal class OpenApiServiceChannelRelationItemTranslator : OpenApiBaseTranslator<ServiceServiceChannel, V11VmOpenApiServiceServiceChannelAstiInBase>
    {
        ITypesCache typeCache;
        public OpenApiServiceChannelRelationItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typeCache = cacheManager.TypesCache;
        }

        public override V11VmOpenApiServiceServiceChannelAstiInBase TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(V11VmOpenApiServiceServiceChannelAstiInBase vModel)
        {
            Guid channelId = vModel.ChannelGuid.IsAssigned() ? vModel.ChannelGuid : Guid.Parse(vModel.ServiceChannelId);

            vModel.Description.ForEach(d =>
            {
                d.OwnerReferenceId = vModel.ServiceGuid;
                d.OwnerReferenceId2 = channelId;
            });

            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .DefineEntitySubTree(i => i.Include(j => j.ServiceServiceChannelPhones).ThenInclude(j => j.Phone))
                .UseDataContextUpdate(i => true, i => o => channelId == o.ServiceChannelId && i.ServiceGuid == o.ServiceId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.ServiceGuid, o => o.ServiceId)
                .AddSimple(i => channelId, o => o.ServiceChannelId);

            // Update data only if they are set
            if (!vModel.ServiceChargeType.IsNullOrEmpty() || vModel.DeleteServiceChargeType)
            {
                Guid? typeId = null;
                if (!vModel.ServiceChargeType.IsNullOrEmpty()) typeId = typeCache.Get<ServiceChargeType>(vModel.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
                definition.AddSimple(i => typeId, o => o.ChargeTypeId);
            }

            if (vModel.Description?.Count > 0 || vModel.DeleteAllDescriptions)
            {
                definition.AddCollection(i => i.Description, o => o.ServiceServiceChannelDescriptions, false);
            }

            if (vModel.IsASTIConnection)
            {
                definition.AddSimple(i => i.IsASTIConnection, o => o.IsASTIConnection);

                // ExtraTypes are mapped only for ASTI connections!
                if (vModel.ExtraTypes?.Count > 0 || vModel.DeleteAllExtraTypes)
                {
                    definition.AddCollection(i => i.ExtraTypes, o => o.ServiceServiceChannelExtraTypes, false);
                }
            }

            if (vModel.ContactDetails != null)
            {
                vModel.ContactDetails.WebPages.ForEach(w => { w.OwnerReferenceId = vModel.ServiceGuid; w.OwnerReferenceId2 = vModel.ChannelGuid; });
                definition.AddPartial(i => i.ContactDetails);
            }

            if (vModel.DeleteAllServiceHours || vModel.ServiceHours?.Count > 0)
            {
                // Set service hours order
                var sortedServiceHours = vModel.ServiceHours == null ? new List<V11VmOpenApiServiceHour>() : vModel.ServiceHours.OrderBy(x => x, new ServiceHourOrderComparer<V8VmOpenApiDailyOpeningTime>()).ToList();

                // Append ordering number for each item
                var index = 1;
                foreach (var serviceHour in sortedServiceHours)
                {
                    serviceHour.OrderNumber = index++;
                    // Set order number for daily opening times
                    if (serviceHour.OpeningHour?.Count > 1)
                    {
                        // The first opening hour for a day has always order number 0.
                        var dailyOpeningHours = serviceHour.OpeningHour.GroupBy(o => o.DayFrom).ToDictionary(o => o.Key, o => o.ToList());
                        dailyOpeningHours.ForEach(day =>
                        {
                            var allHours = day.Value;
                            int order = 0;
                            allHours.ForEach(o => o.Order = order++);
                        });
                    }
                }

                definition.AddCollection(i => sortedServiceHours, o => o.ServiceServiceChannelServiceHours, false);
            }

            return definition.GetFinal();
        }
    }
}
