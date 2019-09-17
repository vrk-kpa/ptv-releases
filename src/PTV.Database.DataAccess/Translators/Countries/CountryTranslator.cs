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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Database.DataAccess.Caches;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.Municipalities
{

    [RegisterService(typeof(ITranslator<Country, VmJsonCountry>), RegisterType.Scope)]
    internal class JsonCountryTranslator : Translator<Country, VmJsonCountry>
    {
        public JsonCountryTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonCountry TranslateEntityToVm(Country entity)
        {
            throw new NotSupportedException();
        }

        public override Country TranslateVmToEntity(VmJsonCountry vModel)
        {
            bool isnew = false;
            var definitiion = CreateViewModelEntityDefinition<Country>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id,
                    i =>
                    {
                        isnew = true;
                        return Guid.NewGuid();
                    }));
            var entity = definitiion.GetFinal();

            if (!isnew)
            {
                vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            var dialCodes = new List<VmJsonDialCode>(){ };
            vModel.DialCodes.ForEach(x => dialCodes.Add(new VmJsonDialCode() { Code = x, OwnerReferenceId = !isnew ? entity.Id : (Guid?)null }));

            return definitiion
                .AddCollection(input => input.Names, output => output.CountryNames)
                .AddCollection(input => dialCodes, output => output.DialCodes)
                .AddNavigation(input => input.Code, output => output.Code)
                .GetFinal();
        }
    }
}
