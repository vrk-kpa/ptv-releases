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
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<DailyOpeningTime, VmDailyHours>), RegisterType.Transient)]
    internal class DailyOpeningHourTranslator : Translator<DailyOpeningTime, VmDailyHours>
    {

        public DailyOpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDailyHours TranslateEntityToVm(DailyOpeningTime entity)
        {
            //            bool isExtra = entity.IsExtra;
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as IDailyHours)
                .AddSimple(input => true, output => output.Day);
//                .AddSimple(input => input.OpeningHourId, output => output.OwnerReferenceId)
//                .AddSimple(input => (DayOfWeek) Enum.ToObject(typeof(DayOfWeek), input.DayFrom), output => output.DayFrom)

//            if (isExtra)
//            {
//                definition
//                    .AddSimple(input => input.From.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeFromExtra)
//                    .AddSimple(input => input.To.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeToExtra);
//            }
//            else
//            {
//                definition
//                    .AddSimple(input => input.From.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeFrom)
//                    .AddSimple(input => input.To.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeTo);
//            }
            return definition.GetFinal();
        }

        public override DailyOpeningTime TranslateVmToEntity(VmDailyHours vModel)
        {
//            bool isExtra = vModel.Extra == DailyHoursExtraTypes.Vissible && vModel.IsExtra;
//            long? timeFrom = isExtra ? vModel.TimeFromExtra : vModel.TimeFrom;
//            long? timeTo = isExtra ? vModel.TimeToExtra : vModel.TimeTo;

            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => vModel.OwnerReferenceId.IsAssigned(), input => output =>
                    input.OwnerReferenceId == output.OpeningHourId && (int)input.DayFrom == output.DayFrom, def => def.UseDataContextCreate(i => true))
                .UseDataContextCreate(input => !vModel.OwnerReferenceId.IsAssigned())
                .AddPartial<IDailyHours>(input => input)
//                .AddSimple(input => (int)input.DayFrom, output => output.DayFrom)
//                .AddSimple(input => input.TimeFrom.FromEpochTimeOfDay(), output => output.From)
//                .AddSimple(input => input.TimeTo.FromEpochTimeOfDay(), output => output.To)
                .GetFinal();
            return entity;
        }
    }

    [RegisterService(typeof(ITranslator<DailyOpeningTime, IDailyHours>), RegisterType.Transient)]
    internal class DailyHoursTranslator : Translator<DailyOpeningTime, IDailyHours>
    {

        public DailyHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IDailyHours TranslateEntityToVm(DailyOpeningTime entity)
        {
//            bool isExtra = entity.IsExtra;
            var definition = CreateEntityViewModelDefinition(entity)
//                .AddSimple(input => true, output => output.Day)
                .AddSimple(input => input.OpeningHourId, output => output.OwnerReferenceId)
                .AddSimple(input => (WeekDayEnum) Enum.ToObject(typeof(WeekDayEnum), input.DayFrom), output => output.DayFrom)
                .AddSimple(input => input.DayTo.HasValue ?(WeekDayEnum) Enum.ToObject(typeof(WeekDayEnum), input.DayTo) : (WeekDayEnum?)null, output => output.DayTo)
//                .AddSimple(input => input.IsExtra ? DailyHoursExtraTypes.Vissible : DailyHoursExtraTypes.Hidden, output => output.Extra)
;

//            if (isExtra)
//            {
//                definition
//                    .AddSimple(input => input.From.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeFromExtra)
//                    .AddSimple(input => input.To.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeToExtra);
//            }
//            else
//            {
                definition
                    .AddSimple(input => input.From.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeFrom)
                    .AddSimple(input => input.To.ToEpochTimeOfDay() + 24*3600*1000, output => output.TimeTo);
//            }
            return definition.GetFinal();
        }

        public override DailyOpeningTime TranslateVmToEntity(IDailyHours vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => vModel.OwnerReferenceId.IsAssigned(), input => output =>
                    input.OwnerReferenceId == output.OpeningHourId && (int)input.DayFrom == output.DayFrom, def => def.UseDataContextCreate(i => true))
                .UseDataContextCreate(input => !vModel.OwnerReferenceId.IsAssigned())
                .AddSimple(input => (int)input.DayFrom, output => output.DayFrom)
                .AddSimple(input => (int?)input.DayTo, output => output.DayTo)
                .AddSimple(input => input.TimeFrom.Value.FromEpochTimeOfDay(), output => output.From)
                .AddSimple(input => input.TimeTo.Value.FromEpochTimeOfDay(), output => output.To)
//                .AddSimple(input => false, output => output.IsExtra)
                .GetFinal();
            return entity;
        }
    }

    [RegisterService(typeof(ITranslator<DailyOpeningTime, VmExtraHours>), RegisterType.Transient)]
    internal class DailyOpeningHourExtraTranslator : Translator<DailyOpeningTime, VmExtraHours>
    {

        public DailyOpeningHourExtraTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmExtraHours TranslateEntityToVm(DailyOpeningTime entity)
        {
            throw new NotSupportedException();
        }

        public override DailyOpeningTime TranslateVmToEntity(VmExtraHours vModel)
        {
            if (vModel == null || vModel.Hours.Extra != DailyHoursExtraTypes.Vissible)
            {
                return null;
            }

            if (!vModel.Hours.TimeFromExtra.HasValue)
            {
                throw new ArgumentNullException("Hours.TimeFromExtra", "Time is mandatory");
            }
            if (!vModel.Hours.TimeToExtra.HasValue)
            {
                throw new ArgumentNullException("Hours.TimeToExtra", "Time is mandatory");
            }
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => vModel.Hours.OwnerReferenceId.IsAssigned(), input => output =>
                    input.Hours.OwnerReferenceId == output.OpeningHourId && (int)input.Hours.DayFrom == output.DayFrom, def => def.UseDataContextCreate(i => true))
                .UseDataContextCreate(input => !vModel.Hours.OwnerReferenceId.IsAssigned())
                .AddPartial<IDailyHours>(input => input)
//                .AddSimple(input => (int)input.Hours.DayFrom, output => output.DayFrom)
//                .AddSimple(input => (input.Hours.TimeFromExtra ?? 0).FromEpochTimeOfDay(), output => output.From)
//                .AddSimple(input => (input.Hours.TimeToExtra ?? 0).FromEpochTimeOfDay(), output => output.To)
                .GetFinal();
            return entity;
        }
    }
}
