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
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmLocationChannel>), RegisterType.Transient)]
    internal class LocationChannelMainTranslator : Translator<ServiceChannelVersioned, VmLocationChannel>
    {
        private ITypesCache typesCache;
        private EntityDefinitionHelper definitionHelper;

        public LocationChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, EntityDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmLocationChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmLocationChannel vModel)
        {
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .DisableAutoTranslation()
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(o => o)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()), o => o.TypeId)
                .AddPartial(i => i.Step1Form)
                .AddPartial(i => i.Step4Form);
            return definition
                .GetFinal();

        }
    }
}
