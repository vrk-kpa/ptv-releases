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

using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmSpecialHours>), RegisterType.Transient)]
    internal class SpecialOpeningHourTranslator2 : Translator<ServiceHours, VmSpecialHours>
    {
        private ITypesCache typesCache;
        private EntityDefinitionHelper definitionHelper;

        public SpecialOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, EntityDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmSpecialHours TranslateEntityToVm(ServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmOpeningHour);
            
            if (entity.DailyOpeningTimes?.Count > 0)
            {
                definition.AddNavigation(input => input.DailyOpeningTimes.FirstOrDefault(), output => output.OpeningPeriod);
                return definition.GetFinal();
            }
            
            // Let's add some default values since special opening hours requires that DailyOpeningTime is always set. PTV-3586 & PTV-3589             
            
            var result = definition.GetFinal();
            result.OpeningPeriod = new VmDailyHourCommon
            {
                DayFrom = WeekDayEnum.Monday,
                DayTo = WeekDayEnum.Monday,
            };

            return result;
        }

        public override ServiceHours TranslateVmToEntity(VmSpecialHours vModel)
        {
            var entity = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .AddPartial<VmOpeningHour>(input => input)
                .AddCollection(GetDayilyHours, output => output.DailyOpeningTimes, TranslationPolicy.FetchData)
                .GetFinal();
            return entity;
        }

        private List<VmDailyHourCommon> GetDayilyHours(VmSpecialHours model)
        {
            model.OpeningPeriod.OwnerReferenceId = model.Id;
            return new List<VmDailyHourCommon>
            {
                model.OpeningPeriod
//               new VmDailyHourCommon
//               {
//                   OwnerReferenceId = model.Id,
//                   DayFrom = model.DayFrom,
//                   DayTo = model.DayTo ,
//                   TimeFrom = model.TimeFrom,
//                   TimeTo = model.TimeTo
//               }
            };
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmSpecialHours>), RegisterType.Transient)]
    internal class ServiceChannelSpecialOpeningHourTranslator2 : Translator<ServiceChannelServiceHours, VmSpecialHours>
    {
        public ServiceChannelSpecialOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmSpecialHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmSpecialHours vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition<ServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.ServiceHoursId)
                .AddNavigation(input => input, output => output.ServiceHours)
                .GetFinal();
        }        
    }
}