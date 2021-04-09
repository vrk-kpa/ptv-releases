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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannelUrl, VmWebPage>), RegisterType.Transient)]
    internal class PrintableFormChannelUrlTranslator : Translator<PrintableFormChannelUrl, VmWebPage>
    {
        private readonly ITypesCache typesCache;

        public PrintableFormChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmWebPage TranslateEntityToVm(PrintableFormChannelUrl entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Type.Id, o => o.TypeId)
                .AddSimple(i => i.Id, o => o.Id)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable);
                
            model.AddNavigation(i => i.WebPage, o => o.UrlAddress);   
            
            return model.GetFinal();
        }

        public override PrintableFormChannelUrl TranslateVmToEntity(VmWebPage vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }

            if (vModel?.UrlAddress == null)
            {
                vModel.UrlAddress = string.Empty;
            }

            var transaltionDefinition = CreateViewModelEntityDefinition<PrintableFormChannelUrl>(vModel)
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .AddSimple(i => i.TypeId ?? typesCache.Get<PrintableFormChannelUrlType>(PrintableFormChannelUrlTypeEnum.PDF.ToString()), o => o.TypeId)
                .AddNavigation(i => vModel.UrlAddress, o => o.WebPage)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .AddRequestLanguage(output => output);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }
}
