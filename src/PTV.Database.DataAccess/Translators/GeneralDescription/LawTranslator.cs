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
using System.Linq;
using Microsoft.AspNetCore.Mvc.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<Law, VmLaw>), RegisterType.Transient)]
    internal class LawTranslator_V2 : Translator<Law, VmLaw>
    {
        private ILanguageCache languageCache;
        public LawTranslator_V2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmLaw TranslateEntityToVm(Law entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddDictionary(i => i.WebPages.Select(x=>x.WebPage).GroupBy(x => x.LocalizationId).Select(x => x.OrderByDescending(y => y.Modified).First()), o => o.UrlAddress, web => languageCache.GetByValue(web.LocalizationId))
                .AddDictionary(i => i.Names, o => o.Name, name => languageCache.GetByValue(name.LocalizationId))
                .GetFinal();

        }

        public override Law TranslateVmToEntity(VmLaw vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var names = new List<VmName>()
            {
                new VmName {Name = vModel.Name == null ? string.Empty : vModel.Name.TryGetOrDefault(RequestLanguageCode.ToString(), string.Empty), OwnerReferenceId = vModel.Id },
            };

            var webPages = new List<VmWebPage>()
            {
                new VmWebPage { UrlAddress = vModel.UrlAddress == null ? string.Empty : vModel.UrlAddress.TryGetOrDefault(RequestLanguageCode.ToString(), string.Empty), OwnerReferenceId = vModel.Id},
            };

            var transaltionDefinition = CreateViewModelEntityDefinition<Law>(vModel)
               .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
               .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
               .AddCollection(i => webPages, o => o.WebPages)
               .AddCollection(i => names, o => o.Names);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }
}