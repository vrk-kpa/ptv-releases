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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<IndustrialClass, VmIndustrialClassJsonItem>), RegisterType.Transient)]
    internal class FintoIndustrialClassTranslator : Translator<IndustrialClass, VmIndustrialClassJsonItem>
    {
        public FintoIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IndustrialClass TranslateVmToEntity(VmIndustrialClassJsonItem vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Uri, def => def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
                //                .UseDataContext(i => i.Id, o => o.Uri)
                .AddNavigation(i => i.Name, o => o.Label)
                .AddNavigation(i => i.Code, o => o.Uri)
                .AddNavigation(i => i.Parent, o => o.ParentUri)
                .AddNavigation(i => i.Level.ToString(), o => o.Code)
//                .AddNavigation(i => "IndustrialClass", o => o.OntologyType)
                .AddCollection(i => new List<string> { i.Name }, o => o.Names)
                .AddCollection(i => i.Children ?? new List<VmIndustrialClassJsonItem>(), o => o.Children)
                .GetFinal();
        }

        public override VmIndustrialClassJsonItem TranslateEntityToVm(IndustrialClass entity)
        {
            throw new NotSupportedException();
        }
    }

}
