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


using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<DailyOpeningTime, V2VmOpenApiDailyOpeningTime>), RegisterType.Transient)]
    internal class OpenApiDailyOpeningHourTranslator : Translator<DailyOpeningTime, V2VmOpenApiDailyOpeningTime>
    {
        public OpenApiDailyOpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V2VmOpenApiDailyOpeningTime TranslateEntityToVm(DailyOpeningTime entity)
        {
            string dayFrom = ((WeekDayEnum)entity.DayFrom).ToString();
            string dayTo = entity.DayTo.HasValue ? ((WeekDayEnum)entity.DayTo).ToString() : string.Empty;
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => dayFrom, o => o.DayFrom)
                .AddNavigation(i => dayTo, o => o.DayTo)
                .AddNavigation(i => i.From.ToString(), o => o.From)
                .AddNavigation(i => i.To.ConvertToString(), o => o.To)
                .AddSimple(i => i.Order > 0 ? true : false, o => o.IsExtra)
                .GetFinal();
        }

        public override DailyOpeningTime TranslateVmToEntity(V2VmOpenApiDailyOpeningTime vModel)
        {
            WeekDayEnum dayFrom, dayTo;
            if (!Enum.TryParse(vModel.DayFrom, out dayFrom))
                throw new Exception($"{vModel.DayFrom} is not a valid day of week");

            TimeSpan from, to;
            if (!TimeSpan.TryParse(vModel.From, out from))
                throw new Exception($"{vModel.From} is not a valid time");
            if (!TimeSpan.TryParse(vModel.To, out to))
                throw new Exception($"{vModel.To} is not a valid time");


            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o =>
                    i.OwnerReferenceId.Value == o.OpeningHourId && (int)dayFrom == o.DayFrom, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => (int)dayFrom, o => o.DayFrom)
                .AddSimple(i => from, o => o.From)
                .AddSimple(i => to, o => o.To)
                .AddSimple(i => i.Order, o => o.Order);

            if (!string.IsNullOrEmpty(vModel.DayTo))
            {
                if (!Enum.TryParse(vModel.DayTo, out dayTo))
                    throw new Exception($"{vModel.DayTo} is not a valid day of week");

                definition.AddSimple(i => (int)dayTo, o => o.DayTo);
            }

            return definition.GetFinal();
        }
    }
}
