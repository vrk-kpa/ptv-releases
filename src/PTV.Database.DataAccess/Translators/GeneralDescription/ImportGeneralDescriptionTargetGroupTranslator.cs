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

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceTargetGroup, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionTargetGroupTranslator : Translator<StatutoryServiceTargetGroup, ImportFintoItem>
    {
        private ITypesCache typesCache;
        public ImportGeneralDescriptionTargetGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportFintoItem TranslateEntityToVm(StatutoryServiceTargetGroup entity)
        {
            throw new NotSupportedException();

        }

        public override StatutoryServiceTargetGroup TranslateVmToEntity(ImportFintoItem vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i, o => o.TargetGroup).GetFinal();
            return entity.TargetGroup == null ? null : entity;

        }
    }

    [RegisterService(typeof(ITranslator<TargetGroup, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportFintoItemTargetGroupTranslator : Translator<TargetGroup, ImportFintoItem>
    {
        private ITypesCache typesCache;
        private ILogger logger;

        public ImportFintoItemTargetGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ILoggerFactory loggerFactory) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            logger = loggerFactory.CreateLogger<ImportFintoItemOntologyTermTranslator>();
        }

        public override ImportFintoItem TranslateEntityToVm(TargetGroup entity)
        {
            throw new NotSupportedException();

        }

        public override TargetGroup TranslateVmToEntity(ImportFintoItem vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => o.Uri.ToLower() == i.Url.ToLower(), def => def.AddSimple(i => Guid.Empty, o => o.Id)).GetFinal();
            if (!entity.Id.IsAssigned())
            {
                logger.LogWarning($"Target group not found for {vModel.Name} - {vModel.Url}.");
                Console.WriteLine($"Target group not found for {vModel.Name} - {vModel.Url}.");
                return null;
            }
            return entity;
        }
    }
}