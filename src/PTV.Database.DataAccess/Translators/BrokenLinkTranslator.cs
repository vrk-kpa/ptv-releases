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

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslator<WebPage, VmBrokenLink>), RegisterType.Transient)]
    internal class BrokenLinkTranslator : Translator<WebPage, VmBrokenLink>
    {
        public BrokenLinkTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmBrokenLink TranslateEntityToVm(WebPage entity)
        {
            return CreateEntityViewModelDefinition<VmBrokenLink>(entity)
                .AddNavigation(i => i.Url, o => o.Url)
                .AddNavigation(i => i.ExceptionComment, o => o.ExceptionComment)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.IsBroken, o => o.IsBroken)
                .AddSimple(i => i.IsException, o => o.IsException)
                .AddSimple(i => i.ValidationDate, o => o.ValidationDate)
                .GetFinal();
        }

        public override WebPage TranslateVmToEntity(VmBrokenLink vModel)
        {
            return CreateViewModelEntityDefinition<WebPage>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Id == o.Id || i.Url == o.Url)
                .AddNavigation(i => i.ExceptionComment, o => o.ExceptionComment)
                .AddSimple(i => i.IsBroken, o => o.IsBroken)
                .AddSimple(i => i.IsException, o => o.IsException)
                .AddSimple(i => i.ValidationDate, o => o.ValidationDate)
                .GetFinal();
        }
    }
}
