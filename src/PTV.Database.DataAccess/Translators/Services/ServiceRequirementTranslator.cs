﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceRequirement, VmServiceRequirement>), RegisterType.Transient)]
    internal class ServiceRequirementTranslator : Translator<ServiceRequirement, VmServiceRequirement>
    {
        public ServiceRequirementTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override VmServiceRequirement TranslateEntityToVm(ServiceRequirement entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceRequirement TranslateVmToEntity(VmServiceRequirement vModel)
        {
            var exists = vModel.Id.IsAssigned();
            var languageId = vModel.LocalizationId.IsAssigned()
                ? vModel.LocalizationId.Value
                : RequestLanguageId;
            
            return CreateViewModelEntityDefinition<ServiceRequirement>(vModel)
                .UseDataContextUpdate(x => exists, i => o => i.Id == o.ServiceVersionedId && languageId == o.LocalizationId, def => def.UseDataContextCreate(i => exists))
                .AddNavigation(input => input.Requirement, output => output.Requirement)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceRequirement, string>), RegisterType.Transient)]
    internal class ServiceRequirementStringTranslator : Translator<ServiceRequirement, string>
    {
        public ServiceRequirementStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override string TranslateEntityToVm(ServiceRequirement entity)
        {
            return CreateEntityViewModelDefinition(entity)
              .AddNavigation(i => i.Requirement, o => o)
              .GetFinal();
        }

        public override ServiceRequirement TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
