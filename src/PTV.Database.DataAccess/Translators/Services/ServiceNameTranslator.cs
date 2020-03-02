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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceName, VmName>), RegisterType.Transient)]
    internal class ServiceNameTranslator : Translator<ServiceName, VmName>
    {
        public ServiceNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmName TranslateEntityToVm(ServiceName entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceName TranslateVmToEntity(VmName vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }

            var definition = CreateViewModelEntityDefinition<ServiceName>(vModel)
                .UseDataContextLocalizedUpdate(input => input.OwnerReferenceId.HasValue, input => output => output.TypeId == input.TypeId && input.OwnerReferenceId == output.ServiceVersionedId, name => name.UseDataContextCreate(x => true))
                .AddNavigation(input => input.Name, output => output.Name)
                .AddRequestLanguage(output => output)
                .AddSimple(input => input.TypeId, output => output.TypeId);

            if (vModel.Inherited.HasValue)
            {
                definition.AddSimple(input => input.Inherited.Value, output => output.Inherited);
            }

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceName, string>), RegisterType.Transient)]
    internal class ServiceNameStringTranslator : Translator<ServiceName, string>
    {
        public ServiceNameStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(ServiceName entity)
        {
            return CreateEntityViewModelDefinition(entity)
               .AddNavigation(i => i.Name, o => o)
               .GetFinal();
        }

        public override ServiceName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
