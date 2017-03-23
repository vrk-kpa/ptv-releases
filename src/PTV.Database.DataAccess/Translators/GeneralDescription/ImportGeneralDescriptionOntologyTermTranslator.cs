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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceOntologyTerm, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionOntologyTermTranslator : Translator<StatutoryServiceOntologyTerm, ImportFintoItem>
    {
        private ITypesCache typesCache;
        public ImportGeneralDescriptionOntologyTermTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportFintoItem TranslateEntityToVm(StatutoryServiceOntologyTerm entity)
        {
            throw new NotSupportedException();

        }

        public override StatutoryServiceOntologyTerm TranslateVmToEntity(ImportFintoItem vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i, o => o.OntologyTerm).GetFinal();
            return entity.OntologyTerm == null ? null : entity;
        }
    }

    [RegisterService(typeof(ITranslator<OntologyTerm, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportFintoItemOntologyTermTranslator : Translator<OntologyTerm, ImportFintoItem>
    {
        private ITypesCache typesCache;
        private ILogger logger;
        public ImportFintoItemOntologyTermTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ILoggerFactory loggerFactory) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            logger = loggerFactory.CreateLogger<ImportFintoItemOntologyTermTranslator>();
        }

        public override ImportFintoItem TranslateEntityToVm(OntologyTerm entity)
        {
            throw new NotSupportedException();

        }

        public override OntologyTerm TranslateVmToEntity(ImportFintoItem vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o =>
                    o.Uri.ToLower() == i.Url.ToLower() || o.Label.ToLower() == i.Name.ToLower(), def => def.AddSimple(i => Guid.Empty, o => o.Id))
                .GetFinal();
            if (!entity.Id.IsAssigned())
            {
                logger.LogWarning($"Ontology term not found for {vModel.Name} - {vModel.Url}.");
                Console.WriteLine($"Ontology term not found for {vModel.Name} - {vModel.Url}.");
                return null;
            }
            return entity;
        }
    }
}