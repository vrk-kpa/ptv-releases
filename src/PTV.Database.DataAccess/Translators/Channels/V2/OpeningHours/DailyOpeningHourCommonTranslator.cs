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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<DailyOpeningTime, VmDailyHourCommon>), RegisterType.Transient)]
    internal class DailyOpeningHourCommonTranslator : Translator<DailyOpeningTime, VmDailyHourCommon>
    {

        public DailyOpeningHourCommonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDailyHourCommon TranslateEntityToVm(DailyOpeningTime entity)
        {
//            bool isExtra = entity.IsExtra;
            var definition = CreateEntityViewModelDefinition(entity)
//                .AddSimple(input => true, output => output.Day)
                    .AddSimple(input => input.OpeningHourId, output => output.OwnerReferenceId)
                    .AddSimple(input => (WeekDayEnum) Enum.ToObject(typeof(WeekDayEnum), input.DayFrom), output => output.DayFrom)
                    .AddSimple(input => (WeekDayEnum) Enum.ToObject(typeof(WeekDayEnum), input.DayTo ?? input.DayFrom), output => output.DayTo)
//                .AddSimple(input => input.IsExtra ? DailyHoursExtraTypes.Vissible : DailyHoursExtraTypes.Hidden, output => output.Extra)
                ;
            if(entity != null)
            {
                definition
                    .AddSimple(input => input.From.ToEpochTimeOfDay(), output => output.TimeFrom)
                    .AddSimple(input => input.To.ToEpochTimeOfDay(), output => output.TimeTo);
            }
            return definition.GetFinal();
        }

        public override DailyOpeningTime TranslateVmToEntity(VmDailyHourCommon vModel)
        {
            var dayFrom = vModel.DayFrom.HasValue ? (int)vModel.DayFrom.Value : (int)WeekDayEnum.Monday;
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => vModel.OwnerReferenceId.IsAssigned(), input => output =>
                    input.OwnerReferenceId == output.OpeningHourId && dayFrom == output.DayFrom && input.Order == output.Order, def => def.UseDataContextCreate(i => true))
                .UseDataContextCreate(input => !vModel.OwnerReferenceId.IsAssigned())
                .AddSimple(input => dayFrom, output => output.DayFrom)
                .AddSimple(input => input.Order, output => output.Order)
                .AddSimple(input => (int?)input.DayTo, output => output.DayTo)
                .AddSimple(input => input.TimeFrom?.FromEpochTimeOfDay() ?? new TimeSpan(0), output => output.From)
                .AddSimple(input => input.TimeTo?.FromEpochTimeOfDay() ?? new TimeSpan(0), output => output.To)
                .GetFinal();
            return entity;
        }
    }
}