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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmHolidayHours>), RegisterType.Transient)]
    internal class HolidayServiceHoursHolidayTranslator : Translator<ServiceHours, VmHolidayHours>
    {
        private readonly Channels.ServiceChannelTranslationDefinitionHelper definitionHelper;
        private readonly ITypesCache typesCache;

        public HolidayServiceHoursHolidayTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, Channels.ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            typesCache = cacheManager.TypesCache;
        }

        public override VmHolidayHours TranslateEntityToVm(ServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmOpeningHour)
                .AddSimple(input => input.IsClosed, output => output.IsClosed)
                .AddSimple(input => true, output => output.Active)
                .AddNavigation(input => typesCache.GetByValue<Holiday>(input.HolidayServiceHour.HolidayId), output => output.Code)
                .AddCollection(input => input.DailyOpeningTimes, output => output.Intervals);

            return definition.GetFinal();
        }

        public override ServiceHours TranslateVmToEntity(VmHolidayHours vModel)
        {
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .AddPartial(input => input as VmOpeningHour)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddNavigation(i => new VmHolidayCode()
                {
                    Code = i.Code,
                    HolidayId = typesCache.Get<Holiday>(i.Code.ToString()),
                    OwnerReferenceId = i.Id
                }, o => o.HolidayServiceHour);

                if (vModel.IsClosed)
                {
                    definition.AddCollection(input => new List<VmDailyHourCommon>(), output => output.DailyOpeningTimes, TranslationPolicy.FetchData);
                }
                else
                {
                    definition.AddCollection(i => i.Intervals, output => output.DailyOpeningTimes, TranslationPolicy.FetchData);
                }
                return definition.GetFinal();
        }
    }
}
