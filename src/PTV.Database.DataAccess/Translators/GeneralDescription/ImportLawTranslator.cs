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
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<Law, ImportLaw>), RegisterType.Transient)]
    internal class ImportLawTranslator : Translator<Law, ImportLaw>
    {
        private ITypesCache typesCache;
        public ImportLawTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportLaw TranslateEntityToVm(Law entity)
        {
            var localizationFi = typesCache.Get<Language>(DomainConstants.DefaultLanguage);
            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection(i => i.Names, o => o.Names)
                .AddCollection(i => i.WebPages, o => o.Links)
                .AddNavigation(i => (i.WebPages.FirstOrDefault(j => j.WebPage.LocalizationId == localizationFi) ?? i.WebPages.FirstOrDefault())?.WebPage?.Url ?? string.Empty, o => o.LawReference)
                .GetFinal();
        }

        public override Law TranslateVmToEntity(ImportLaw vModel)
        {
            bool newEntity = false;
            string webPage = !string.IsNullOrEmpty(vModel.LawReference)? vModel.LawReference : (vModel.Links.FirstOrDefault(x => x.Lang.ToLower() == DomainConstants.DefaultLanguage.ToLower()) ?? vModel.Links.FirstOrDefault()) ?.Label;
            if (!vModel.Id.IsAssigned())
            {
                vModel.Id = null;
            }
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Id != null && (o.Id == i.Id.Value), d => 
                d.UseDataContextUpdate(i => true, i => o => o.WebPages.Any(x => x.WebPage.Url.ToLower() == webPage) && o.StatutoryServiceLaws.Any(), def =>
                {
                    newEntity = true;
                    def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid());
                }));
            var entity = definition.GetFinal();
            if (newEntity)
            {
                definition
                    .AddCollection(i => i.Names, o => o.Names)
                    .AddCollection(i => i.Links, o => o.WebPages);
            }
            vModel.Id = entity.Id;
            return entity;
        }
    }
}