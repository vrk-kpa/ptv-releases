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
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ElectronicChannelUrl, VmWebPage>), RegisterType.Transient)]
    internal class ElectronicChannelUrlTranslator : Translator<ElectronicChannelUrl, VmWebPage>
    {
        public ElectronicChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmWebPage TranslateEntityToVm(ElectronicChannelUrl entity)
        {
            return CreateEntityViewModelDefinition<VmWebPage>(entity)
                .AddNavigation(i => i.WebPage, o => o.UrlAddress)
                .AddSimple(i => i.ElectronicChannelId, o => o.Id)
                .GetFinal();
        }

        public override ElectronicChannelUrl TranslateVmToEntity(VmWebPage vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            var transaltionDefinition = CreateViewModelEntityDefinition<ElectronicChannelUrl>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => (i.Id == o.ElectronicChannelId), def => def.UseDataContextCreate(i => true))
               .AddNavigation(i => vModel.UrlAddress, o => o.WebPage)
               .AddRequestLanguage(output => output);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

    }
}
