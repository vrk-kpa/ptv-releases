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
    [RegisterService(typeof(ITranslator<StatutoryServiceIndustrialClass, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionIndustrialClassTranslator : Translator<StatutoryServiceIndustrialClass, ImportFintoItem>
    {
        private ITypesCache typesCache;
        public ImportGeneralDescriptionIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportFintoItem TranslateEntityToVm(StatutoryServiceIndustrialClass entity)
        {
            throw new NotSupportedException();

        }

        public override StatutoryServiceIndustrialClass TranslateVmToEntity(ImportFintoItem vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i, o => o.IndustrialClass).GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<IndustrialClass, ImportFintoItem>), RegisterType.Transient)]
    internal class ImportFintoItemIndustrialClassTranslator : Translator<IndustrialClass, ImportFintoItem>
    {
        private ITypesCache typesCache;
        public ImportFintoItemIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportFintoItem TranslateEntityToVm(IndustrialClass entity)
        {
            throw new NotSupportedException();

        }

        public override IndustrialClass TranslateVmToEntity(ImportFintoItem vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => o.Uri.ToLower() == i.Url.ToLower()).GetFinal();
        }
    }
}