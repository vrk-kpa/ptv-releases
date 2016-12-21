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
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmExceptionalHours>), RegisterType.Transient)]
    internal class ExceptionalOpeningHourTranslator : Translator<ServiceChannelServiceHours, VmExceptionalHours>
    {
        private ITypesCache typesCache;
        private ServiceChannelTranslationDefinitionHelper definitionHelper;

        public ExceptionalOpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmExceptionalHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity);
            var result = definitionHelper.AddOpeningHoursDefinition(definition)
                .AddSimple(input => input.IsClosed, output => output.Closed)
                .AddNavigation<DailyOpeningTime, IDailyHours>(input => input.DailyOpeningTimes.FirstOrDefault(), output => output, true)
                .GetFinal();

            return result;
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmExceptionalHours vModel)
        {
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel));
            definitionHelper.AddOpeningHoursDefinition(definition)
                .AddSimple(input => input.Closed, output => output.IsClosed);
            if (vModel.Closed)
            {
                definition.AddCollection(input => new List<IDailyHours>(), output => output.DailyOpeningTimes);
            }
            else
            {
                definition.AddNavigationOneMany<IDailyHours, DailyOpeningTime>(input => input, output => output.DailyOpeningTimes);
            }


            return definition.GetFinal();
        }
    }
}