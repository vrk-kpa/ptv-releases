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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceLifeEvent, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionLifeEventTranslator : Translator<StatutoryServiceLifeEvent, ImportFintoItem>
    {
        public ImportGeneralDescriptionLifeEventTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override ImportFintoItem TranslateEntityToVm(StatutoryServiceLifeEvent entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input.LifeEvent)
                .GetFinal();
        }

        public override StatutoryServiceLifeEvent TranslateVmToEntity(ImportFintoItem vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i, o => o.LifeEvent).GetFinal();
            return entity.LifeEvent == null ? null : entity;
        }
    }

    [RegisterService(typeof(ITranslator<LifeEvent, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportFintoItemLifeEventTranslator : Translator<LifeEvent, ImportFintoItem>
    {
        private ILogger<ImportFintoItemOntologyTermTranslator> logger;
        private IFintoJsonCache fintoCache;

        public ImportFintoItemLifeEventTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILoggerFactory loggerFactory, IFintoJsonCache fintoCache) : base(resolveManager, translationPrimitives)
        {
            this.fintoCache = fintoCache;
            logger = loggerFactory.CreateLogger<ImportFintoItemOntologyTermTranslator>();
        }

        public override ImportFintoItem TranslateEntityToVm(LifeEvent entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Uri, output => output.Url)
                .AddNavigation(input => input.Label, output => output.Name)
                .GetFinal();
        }

        public override LifeEvent TranslateVmToEntity(ImportFintoItem vModel)
        {
            string replacedBy = fintoCache.Get<LifeEvent>(vModel.Url).ToLower();
            var entity = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true,
                    i => o => o.Uri.ToLower() == i.Url.ToLower() || o.Uri.ToLower() == replacedBy,
                    def => def.AddSimple(i => Guid.Empty, o => o.Id)).GetFinal();
            if (!entity.Id.IsAssigned())
            {
                logger.LogWarning($"Life event not found for {vModel.Name} - {vModel.Url} ({replacedBy}).");
                Console.WriteLine($"Life event not found for {vModel.Name} - {vModel.Url} ({replacedBy}).");
                return null;
            }
            return entity;
        }
    }
}
