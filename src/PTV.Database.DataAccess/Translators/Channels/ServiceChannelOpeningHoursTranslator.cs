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
    [RegisterService(typeof(ITranslator<ServiceChannel, VmOpeningHoursStep>), RegisterType.Transient)]
    internal class ServiceChannelOpeningHoursTranslator : Translator<ServiceChannel, VmOpeningHoursStep>
    {
        private ITypesCache typesCache;
        public ServiceChannelOpeningHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmOpeningHoursStep TranslateEntityToVm(ServiceChannel entity)
        {
            return CreateEntityViewModelDefinition<VmOpeningHoursStep>(entity)
                .AddSimple(i=>i.Id, o=>o.Id)
                .AddCollection(i => i.ServiceHours.Where(j => typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, ServiceHoursTypeEnum.Standard.ToString())), o => o.StandardHours)
                .AddCollection(i => i.ServiceHours.Where(j => typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, ServiceHoursTypeEnum.Exception.ToString())), o => o.ExceptionHours)
                .AddCollection(i => i.ServiceHours.Where(j => typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, ServiceHoursTypeEnum.Special.ToString())), o => o.SpecialHours)
                .GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmOpeningHoursStep vModel)
        {
            vModel.StandardHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Standard);
            vModel.ExceptionHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Exception);
            vModel.SpecialHours.ForEach(x => x.ServiceHoursType = ServiceHoursTypeEnum.Special);
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddCollection(i => i.StandardHours, o => o.ServiceHours);
            var entity = definition.GetFinal();
            var openingHoursList = new List<ServiceChannelServiceHours>(entity.ServiceHours);
            definition.AddCollection(i => i.SpecialHours, o => o.ServiceHours);
            openingHoursList.AddRange(entity.ServiceHours);
            definition.AddCollection(i => i.ExceptionHours, o => o.ServiceHours);
            openingHoursList.AddRange(entity.ServiceHours);
            entity.ServiceHours = openingHoursList;
            return entity;
        }
    }
}
