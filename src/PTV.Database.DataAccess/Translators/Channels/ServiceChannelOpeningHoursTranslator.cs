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
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpeningHoursStep>), RegisterType.Transient)]
    internal class ServiceChannelOpeningHoursTranslator : Translator<ServiceChannelVersioned, VmOpeningHoursStep>
    {
        private ITypesCache typesCache;
        public ServiceChannelOpeningHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmOpeningHoursStep TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var serviceHours = entity.ServiceChannelServiceHours.Select(x => x.ServiceHours).ToList();
            return CreateEntityViewModelDefinition<VmOpeningHoursStep>(entity)
                .AddSimple(i=>i.Id, o=>o.Id)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Standard), o => o.StandardHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception), o => o.ExceptionHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Special), o => o.SpecialHours)
                .GetFinal();
        }

        private IEnumerable<ServiceHours> GetHoursByType(ICollection<ServiceHours> openingHours, ServiceHoursTypeEnum type)
        {
            return openingHours
                .Where(j => typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created);
        }
        private IEnumerable<T> GetHoursWithOrder<T>(IEnumerable<T> openingHours) where T : VmHours
        {
            int order = 0;
            return openingHours.Select(x =>
            {
                x.OrderNumber = order++;
                return x;
            });
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpeningHoursStep vModel)
        {
            vModel.StandardHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Standard);
            vModel.ExceptionHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Exception);
            vModel.SpecialHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Special);
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(o => o)
                .AddCollectionWithRemove(i => GetHoursWithOrder(i.StandardHours), o => o.ServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString()))
                .AddCollectionWithRemove(i => GetHoursWithOrder(i.SpecialHours), o => o.ServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Special.ToString()))
                .AddCollectionWithRemove(i => GetHoursWithOrder(i.ExceptionHours), o => o.ServiceChannelServiceHours, r => r.ServiceHours != null && r.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString()));
            var entity = definition.GetFinal();
            return entity;
        }
    }
}
