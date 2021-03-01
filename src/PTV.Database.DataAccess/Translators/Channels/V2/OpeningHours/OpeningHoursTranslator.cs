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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpeningHours>), RegisterType.Transient)]
    internal class OpeningHoursTranslator : Translator<ServiceChannelVersioned, VmOpeningHours>
    {
        private ITypesCache typesCache;
        public OpeningHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmOpeningHours TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var serviceHours = entity.ServiceChannelServiceHours.Select(x => x.ServiceHours).ToList();

            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Standard), o => o.StandardHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception), o => o.ExceptionHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception, true), o => o.HolidayHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Special), o => o.SpecialHours)
                .GetFinal();
        }

        private IEnumerable<ServiceHours> GetHoursByType(ICollection<ServiceHours> openingHours, ServiceHoursTypeEnum type, bool isHolidayHours = false)
        {
            if (isHolidayHours)
            {
                return openingHours
                    .Where(j => j.HolidayServiceHour != null && typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                    .OrderBy(x => x.OrderNumber)
                    .ThenBy(x => x.Created);
            }

            return openingHours
                .Where(j => j.HolidayServiceHour == null && typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created);
        }
        private IEnumerable<T> GetHoursWithOrder<T>(IEnumerable<T> openingHours, ServiceHoursTypeEnum type) where T : VmOpeningHour
        {
            int order = 0;
            return openingHours.Select(x =>
            {
                x.ServiceHoursType = type;
                x.OrderNumber = order++;
                return x;
            });
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpeningHours vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddCollectionWithRemove(
                    i => GetHoursWithOrder(i.StandardHours, ServiceHoursTypeEnum.Standard),
                    o => o.ServiceChannelServiceHours,
                    r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString())
                )
                .AddCollectionWithRemove(
                    i => GetHoursWithOrder(i.SpecialHours, ServiceHoursTypeEnum.Special),
                    o => o.ServiceChannelServiceHours,
                    r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Special.ToString())
                )
                .AddCollectionWithRemove(
                    i => GetHoursWithOrder(i.ExceptionHours, ServiceHoursTypeEnum.Exception),
                    o => o.ServiceChannelServiceHours,
                    r => r.ServiceHours != null && r.ServiceHours.HolidayServiceHour == null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString())
                )
                .AddCollectionWithRemove(
                    i => GetHoursWithOrder(i.HolidayHours, ServiceHoursTypeEnum.Exception),
                    o => o.ServiceChannelServiceHours,
                    r => r.ServiceHours != null && r.ServiceHours.HolidayServiceHour != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString())
                );
            var entity = definition.GetFinal();
            return entity;
        }
    }
}
