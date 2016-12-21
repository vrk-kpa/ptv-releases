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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class NormalOpeningHourTranslator : Translator<ServiceChannelServiceHours, VmNormalHours>
    {
        private ITypesCache typesCache;
        private ServiceChannelTranslationDefinitionHelper definitionHelper;

        public NormalOpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmNormalHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity);
//                .AddNavigation<ServiceChannelServiceHours, VmHours>(input => input, output => output, true)
            definitionHelper.AddOpeningHoursDefinition(definition);

            var result = definition
                .AddCollection(input => input.DailyOpeningTimes, output => output.DailyHours)
                .AddSimple(input => input.DailyOpeningTimes.Count == 0, output => output.Nonstop)
                .GetFinal();

            result.DailyHours = result.DailyHours.GroupBy(x => x.DayFrom)
                .Select(x => x.OrderBy(d => d.Extra).Aggregate((d1, d2) =>
                {
                    d1.TimeFromExtra = d2.TimeFrom;
                    d1.TimeToExtra = d2.TimeTo;
                    d1.Extra = d2.Extra;
                    return d1;
                })).ToList();
            return result;
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null)
            {
                return null;
            }
            if (vModel.Id.IsAssigned())
            {
                vModel.DailyHours.ForEach(x => x.OwnerReferenceId = vModel.Id);
            }
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel));
            definitionHelper.AddOpeningHoursDefinition(definition);

            var entity = definition.GetFinal();
            if (vModel.Nonstop)
            {
                definition.AddCollection(input => new List<VmNormalHours>(), output => output.DailyOpeningTimes);
            }
            else
            {

                definition.AddCollection(input => input.DailyHours, output => output.DailyOpeningTimes);

                var extra = vModel.DailyHours.Where(x => x.Extra == DailyHoursExtraTypes.Vissible)
                    .Select(x => new VmExtraHours {Hours = x})
                    .ToList();
                if (extra.Count > 0)
                {
                    var list = new List<DailyOpeningTime>(entity.DailyOpeningTimes);

                    var extraHours = CreateViewModelEntityDefinition(vModel)
                            .AddCollection(i => extra, o => o.DailyOpeningTimes)
                            .GetFinal().DailyOpeningTimes;
                    list.AddRange(extraHours);
                    entity.DailyOpeningTimes = list;
                }
            }
            return entity;
        }
    }
}