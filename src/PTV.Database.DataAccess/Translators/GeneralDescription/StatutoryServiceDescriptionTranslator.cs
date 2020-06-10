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
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceDescription, VmDescription>), RegisterType.Transient)]
    internal class StatutoryServiceDescriptionTranslator : Translator<StatutoryServiceDescription, VmDescription>
    {
        public StatutoryServiceDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDescription TranslateEntityToVm(StatutoryServiceDescription entity)
        {
            throw new NotImplementedException();
        }

        public override StatutoryServiceDescription TranslateVmToEntity(VmDescription vModel)
        {
            var definition = vModel.LocalizationId.IsAssigned() ? GetDefinitionForLanguage(vModel) : GetLocalizedDefinition(vModel);
            definition
                .AddNavigation(input => input.Description, output => output.Description)
                .AddSimple(input => input.TypeId, output => output.TypeId);

            return definition.GetFinal();
        }

        private ITranslationDefinitions<VmDescription, StatutoryServiceDescription> GetLocalizedDefinition(VmDescription vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextLocalizedUpdate(
                    input => input.OwnerReferenceId.HasValue, input => output =>
                        output.TypeId == input.TypeId &&
                        input.OwnerReferenceId == output.StatutoryServiceGeneralDescriptionVersionedId,
                    name => name.UseDataContextCreate(x => true))
                .AddRequestLanguage(output => output);

        }
        private ITranslationDefinitions<VmDescription, StatutoryServiceDescription> GetDefinitionForLanguage(VmDescription vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(
                    input => input.OwnerReferenceId.HasValue, input => output =>
                        output.TypeId == input.TypeId &&
                        input.OwnerReferenceId == output.StatutoryServiceGeneralDescriptionVersionedId &&
                        input.LocalizationId == output.LocalizationId,
                    name => name.UseDataContextCreate(x => true))
                .AddSimple(i => i.LocalizationId.Value, o => o.LocalizationId);

        }
    }
}
