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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<Law, VmLaw>), RegisterType.Transient)]
    internal class LawTranslator_V2 : Translator<Law, VmLaw>
    {
        public LawTranslator_V2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLaw TranslateEntityToVm(Law entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddDictionary(
                    i => i.WebPages.GroupBy(x => x.LocalizationId)
                        .Select(x => x.OrderByDescending(y => y.Modified).First()),
                    o => o.WebPage,
                    web => languageCache.GetByValue(web.LocalizationId))
                .AddDictionary(i => i.Names, o => o.Name, name => languageCache.GetByValue(name.LocalizationId))
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .GetFinal();

        }

        public override Law TranslateVmToEntity(VmLaw vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var transaltionDefinition = CreateViewModelEntityDefinition<Law>(vModel)
                .DefineEntitySubTree(i => i.Include(j => j.Names).Include(j => j.WebPages).ThenInclude(j => j.WebPage))
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .AddCollection(i => i.WebPage?.Where(x => !x.Value.UrlAddress.IsNullOrWhitespace())
                    .Select(
                    pair => new VmWebPage
                    {
                        Id = pair.Value.Id,
                        UrlAddress = pair.Value.UrlAddress,
                        LocalizationId = languageCache.Get(pair.Key),
                        Name = i.Name?.Where(x => x.Key == pair.Key).Select(x => x.Value).FirstOrDefault()
                    }), o => o.WebPages, true)
                .AddCollection(i => i.Name?.Select(
                        pair => new VmName
                        {
                            OwnerReferenceId = i.Id,
                            Name = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key)
                        }),
                    o => o.Names, true)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }
}
