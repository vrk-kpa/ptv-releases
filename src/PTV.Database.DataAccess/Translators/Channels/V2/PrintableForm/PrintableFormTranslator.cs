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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;

namespace PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm
{
    [RegisterService(typeof(ITranslator<PrintableFormChannel, VmPrintableForm>), RegisterType.Transient)]
    internal class PrintableFormTranslator : Translator<PrintableFormChannel, VmPrintableForm>
    {
        private readonly IInternalLanguageCache internalLanguageCache;
        private readonly EntityDefinitionHelper definitionHelper;

        public PrintableFormTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, EntityDefinitionHelper entityHelper)
            : base(resolveManager, translationPrimitives)
        {
            internalLanguageCache = languageCache as IInternalLanguageCache;
            definitionHelper = entityHelper;
        }

        public override VmPrintableForm TranslateEntityToVm(PrintableFormChannel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddDictionary(
                    i => i.FormIdentifiers,
                    o => o.FormIdentifier,
                    k => languageCache.GetByValue(k.LocalizationId)
                );

            definitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    input => internalLanguageCache.FilterCollection(input.ChannelUrls, RequestLanguageId),
                    output => output.FormFiles,
                    k => languageCache.GetByValue(k.LocalizationId)
                );

            return definition.GetFinal();
        }

        public override PrintableFormChannel TranslateVmToEntity(VmPrintableForm vModel)
        {
            var translationDefinition = CreateViewModelEntityDefinition<PrintableFormChannel>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => o.ServiceChannelVersionedId == i.Id)
                .Propagation((i, o) => i.PrintableFormChannelId = o.Id)
                .AddCollection(i => i.FormIdentifier?.Select(
                        pair => new VmPrintableFormChannelIdentifier
                        {
                            PrintableFormChannelId = i.PrintableFormChannelId,
                            FormIdentifier = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key)
                        }),
                    o => o.FormIdentifiers, true);

            foreach (var key in vModel.FormFiles?.Keys.ToList() ?? new List<string>())
            {
                vModel.FormFiles[key] = vModel.FormFiles[key].Where(x => !x.UrlAddress.IsNullOrWhitespace()).ToList();
            }

            translationDefinition.AddCollectionWithRemove(i => i.FormFiles?.SelectMany(pair =>
                {
                    var localizationId = languageCache.Get(pair.Key);
                    return pair.Value.Select(ff =>
                    {
                        ff.OwnerReferenceId2 = i.PrintableFormChannelId;
                        ff.LocalizationId = localizationId;
                        return ff;
                    });
                }), o => o.ChannelUrls, x => true);
            definitionHelper.AddOrderedCollectionWithRemove(
                translationDefinition,
                i => i.FormFiles,
                o => o.ChannelUrls,
                x => true
            );
            var entity = translationDefinition.GetFinal();
            return entity;
        }
    }
}

