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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmHours>), RegisterType.Transient)]
    internal class ServiceChannelServiceHourTranslator : Translator<ServiceChannelServiceHours, VmHours>
    {
        private ITypesCache typesCache;

        public ServiceChannelServiceHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o=> o.Id)
                .AddSimple(input => input.OpeningHoursFrom.ToEpochTime(), output => output.ValidFrom)
                .AddSimple(input => input.OpeningHoursTo.ToEpochTime(), output => output.ValidTo)
                .AddSimple(input => input.OpeningHoursTo.HasValue, output => output.IsDateRange)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .AddLocalizable<IText, string>(input => input.AdditionalInformations, output => output.AdditionalInformation)
                .GetFinal();

            return result;
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmHours vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !vModel.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => vModel.Id.IsAssigned(), input => output => input.Id == output.Id)
                .AddLocalizable(input => new VmStringText { Text = input.AdditionalInformation, Id = input.Id ?? Guid.Empty }, output => output.AdditionalInformations)
                .AddSimple(input => input.ValidFrom.FromEpochTime() ?? DateTime.UtcNow.Date, output => output.OpeningHoursFrom)
                .AddSimple(input => input.IsDateRange ? input.ValidTo.FromEpochTime() : null, output => output.OpeningHoursTo)
                .AddSimple(input => typesCache.Get<ServiceHourType>(input.ServiceHoursType.ToString()), output => output.ServiceHourTypeId)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .GetFinal();

            return entity;
        }

    }
}
