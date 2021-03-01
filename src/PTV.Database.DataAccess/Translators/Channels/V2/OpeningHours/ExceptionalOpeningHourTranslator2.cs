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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmExceptionalHours>), RegisterType.Transient)]
    internal class ExceptionalOpeningHourTranslator2 : Translator<ServiceHours, VmExceptionalHours>
    {
        private Channels.ServiceChannelTranslationDefinitionHelper definitionHelper;

        public ExceptionalOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, Channels.ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmExceptionalHours TranslateEntityToVm(ServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmOpeningHour)
                .AddSimple(input => input.IsClosed, output => output.ClosedForPeriod)
                .AddNavigation(input => input.DailyOpeningTimes.FirstOrDefault(), output => output.OpeningPeriod);

            return definition.GetFinal();
        }

        public override ServiceHours TranslateVmToEntity(VmExceptionalHours vModel)
        {
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .AddPartial(input => input as VmOpeningHour)
                .AddSimple(input => input.ClosedForPeriod, output => output.IsClosed);
            if (vModel.ClosedForPeriod)
            {
                definition.AddCollection(input => new List<VmDailyHourCommon>(), output => output.DailyOpeningTimes, TranslationPolicy.FetchData);
            }
            else
            {
                definition.AddCollection(GetDayilyHours, output => output.DailyOpeningTimes, TranslationPolicy.FetchData);
            }


            return definition.GetFinal();
        }

        private List<VmDailyHourCommon> GetDayilyHours(VmExceptionalHours model)
        {
            model.OpeningPeriod.OwnerReferenceId = model.Id;
            return new List<VmDailyHourCommon>
            {
                model.OpeningPeriod
            };
        }

    }

    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmExceptionalHours>), RegisterType.Transient)]
    internal class ServiceChannelExceptionalOpeningHourTranslator2 : Translator<ServiceChannelServiceHours, VmExceptionalHours>
    {
        public ServiceChannelExceptionalOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmExceptionalHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmExceptionalHours vModel)
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
