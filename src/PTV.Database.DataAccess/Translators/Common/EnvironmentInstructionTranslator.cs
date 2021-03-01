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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<AppEnvironmentData, VmEnvironmentInstructionsBase>), RegisterType.Transient)]
    internal class EnvironmentInstructionTranslatorBase : Translator<AppEnvironmentData, VmEnvironmentInstructionsBase>
    {
        public EnvironmentInstructionTranslatorBase(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmEnvironmentInstructionsBase TranslateEntityToVm(AppEnvironmentData entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.FreeText, output => output.EnvironmentInstructions)
                .GetFinal();
        }

        public override AppEnvironmentData TranslateVmToEntity(VmEnvironmentInstructionsBase vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .AddNavigation(input => input.EnvironmentInstructions, output => output.FreeText)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AppEnvironmentData, VmEnvironmentInstructionsIn>), RegisterType.Transient)]
    internal class EnvironmentInstructionTranslatorIn : Translator<AppEnvironmentData, VmEnvironmentInstructionsIn>
    {
        private ITypesCache typesCache;
        public EnvironmentInstructionTranslatorIn(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmEnvironmentInstructionsIn TranslateEntityToVm(AppEnvironmentData entity)
        {
            throw new NotImplementedException();
        }

        public override AppEnvironmentData TranslateVmToEntity(VmEnvironmentInstructionsIn vModel)
        {
            var instructionType = typesCache.Get<AppEnvironmentDataType>(AppEnvironmentDataTypeEnum.EnvironmentInstruction.ToString());
            return CreateViewModelEntityDefinition(vModel)
                .AddPartial(input => input as VmEnvironmentInstructionsBase)
                .AddSimple(input => instructionType, output => output.TypeId)
                .AddSimple(input => input.Version, output => output.Version)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AppEnvironmentData, VmEnvironmentInstruction>), RegisterType.Transient)]
    internal class EnvironmentInstructionTranslator : Translator<AppEnvironmentData, VmEnvironmentInstruction>
    {
        public EnvironmentInstructionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmEnvironmentInstruction TranslateEntityToVm(AppEnvironmentData entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmEnvironmentInstructionsBase)
                .AddSimple(input => input.Id, output => output.Id)
                .AddNavigation(input => input.CreatedBy, output => output.CreatedBy)
                .AddSimple(input => input.Created, output => output.Created)
                .AddNavigation(input => input.ModifiedBy, output => output.ModifiedBy)
                .AddSimple(input => input.Modified, output => output.Modified)
                .GetFinal();
        }

        public override AppEnvironmentData TranslateVmToEntity(VmEnvironmentInstruction vModel)
        {
            throw new NotImplementedException();
        }
    }
}
