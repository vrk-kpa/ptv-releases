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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.DialCodes
{

    [RegisterService(typeof(ITranslator<DialCode, VmDialCode>), RegisterType.Scope)]
    internal class DialCodeTranslator : Translator<DialCode, VmDialCode>
    {
        public DialCodeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDialCode TranslateEntityToVm(DialCode entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddPartial<INameReferences, IVmTranslatedItem>(i => i.Country != null ? new Country { Id = i.Id, CountryNames = i.Country.CountryNames}  : null, o => o).GetFinal();
        }

        public override DialCode TranslateVmToEntity(VmDialCode vModel)
        {
            return CreateViewModelEntityDefinition<DialCode>(vModel)
                .UseDataContextUpdate(
                    i => true,
                    i => o => i.Id == o.Id,
                    def => def.UseDataContextCreate(i => true)
                 )
                .AddNavigation(i => i.Code, o => o.Code)
                .GetFinal();
        }
    }
}