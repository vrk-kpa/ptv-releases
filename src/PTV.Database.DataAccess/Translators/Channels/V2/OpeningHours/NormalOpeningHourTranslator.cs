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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class NormalOpeningHourTranslator : Translator<ServiceHours, VmNormalHours>
    {
        private Channels.ServiceChannelTranslationDefinitionHelper definitionHelper;

        public NormalOpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, Channels.ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmNormalHours TranslateEntityToVm(ServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmOpeningHour)
                .AddDictionary(GetHours, output => output.DailyHours, key => key.Day.ToString())
                .AddSimple(input => input.IsNonStop, output => output.IsNonStop)
                .AddSimple(input => input.IsReservation, output => output.IsReservation);
            var result = definition.GetFinal();
            if (!result.IsNonStop && !result.IsReservation)
            {
                Enum.GetNames(typeof(WeekDayEnum)).ForEach(x =>
                {
                    if (!result.DailyHours.ContainsKey(x))
                    {
                        result.DailyHours.Add(x, null);
                    }
                });
            }
            return result;
        }

        private IEnumerable<KeyValuePair<string, VmNormalDailyOpeningHour>> FilterAvaliableDays(VmNormalHours vModel)
        {
            if (!vModel.IsNonStop && !vModel.IsReservation && vModel.IsPeriod && vModel.DailyHours?.Any() == true && vModel.DateFrom.HasValue && vModel.DateTo.HasValue)
            {
                var period = (vModel.DateTo.Value - vModel.DateFrom.Value).FromEpochTimeDuration().Days;
                if (period < 6)
                {
                    var daysOfTheWeek = Enum.GetNames(typeof(DayOfWeek));
                    var from = vModel.DateFrom.Value.FromEpochTime();
                    var to = vModel.DateTo.Value.FromEpochTime();

                    var avaliableDays = daysOfTheWeek.Where(day =>
                        {
                            var dayValue = (DayOfWeek) Enum.Parse(typeof(DayOfWeek), day, true);
                            return @from.DayOfWeek < to.DayOfWeek
                                ? @from.DayOfWeek <= dayValue && to.DayOfWeek >= dayValue
                                : @from.DayOfWeek <= dayValue || to.DayOfWeek >= dayValue;
                        }
                    ).Select(day => (WeekDayEnum) Enum.Parse(typeof(WeekDayEnum), day, true)).ToList();

                    return vModel.DailyHours.Where(x =>
                        x.Value != null && x.Value.Active &&
                        Enum.TryParse(x.Key, true, out WeekDayEnum day) &&
                        avaliableDays.Contains(day)
                    );
                }
            }

            return vModel.DailyHours?.Where(x => x.Value != null && x.Value.Active);
        }

        private static IEnumerable<DailyOpeningHoursEntitiesModel> GetHours(ServiceHours input)
        {
            return input.DailyOpeningTimes
                .GroupBy(x => x.DayFrom)
                .Select(x => new DailyOpeningHoursEntitiesModel {
                    Day = (WeekDayEnum)Enum.ToObject(typeof(WeekDayEnum), x.Key),
                    Hours = x.Select(i => i).ToList()
                });
        }

        public override ServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null) return null;

            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .AddPartial(i => i as VmOpeningHour)
                .AddSimple(i => i.IsNonStop, o => o.IsNonStop)
                .AddSimple(i => i.IsReservation, o => o.IsReservation);

            if (vModel.IsNonStop || vModel.IsReservation)
            {
                definition
                    .AddCollectionWithRemove(
                        input => new List<VmNormalDailyOpeningHour>(),
                        output => output.DailyOpeningTimes,
                        TranslationPolicy.FetchData,
                        x => x.OpeningHourId == vModel.Id
                    );
            }
            else
            {
                definition
                    .AddCollectionWithRemove(
                        input => FilterAvaliableDays(input)?
                            .SelectMany(x =>
                            {
                                if (Enum.TryParse(x.Key, true, out WeekDayEnum day))
                                {
                                    int order = 0;
                                    x.Value.Intervals = x.Value.Intervals
                                        .Where(interval => interval != null)
                                        .ToList();
                                    x.Value.Intervals.ForEach(interval =>
                                    {
                                        interval.DayFrom = day;
                                        interval.OwnerReferenceId = input.Id;
                                        interval.OrderNumber = order++;
                                    });
                                }
                                return x.Value.Intervals;
                            }
                        ),
                        output => output.DailyOpeningTimes,
                        x => true
                    );
            }
            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class ServiceChannelNormalOpeningHourTranslator2 : Translator<ServiceChannelServiceHours, VmNormalHours>
    {
        public ServiceChannelNormalOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmNormalHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition< ServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.ServiceHoursId)
                .AddNavigation(input => input, output => output.ServiceHours)
                .GetFinal();
        }
    }
}
