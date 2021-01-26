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
    internal class DailyOpeningHoursEntitiesModel
    {
        public DailyOpeningHoursEntitiesModel()
        {
            Hours = new List<DailyOpeningTime>();
        }

        public WeekDayEnum Day { get; set; }
        public List<DailyOpeningTime> Hours { get; set; }
    }

    [RegisterService(typeof(ITranslator<DailyOpeningHoursEntitiesModel, VmNormalDailyOpeningHour>), RegisterType.Transient)]
    internal class NormalDailyOpeningHoursTranslator : Translator<DailyOpeningHoursEntitiesModel, VmNormalDailyOpeningHour>
    {

        public NormalDailyOpeningHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmNormalDailyOpeningHour TranslateEntityToVm(DailyOpeningHoursEntitiesModel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                    .AddSimple(input => true, output => output.Active)
                    .AddCollection(i => i.Hours, o => o.Intervals);
            return definition.GetFinal();
        }

        public override DailyOpeningHoursEntitiesModel TranslateVmToEntity(VmNormalDailyOpeningHour vModel)
        {
            throw new NotSupportedException();
        }
    }
}
