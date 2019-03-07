﻿/**
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
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Finto.Base;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<OntologyTerm, VmServiceViewsJsonItem>), RegisterType.Transient)]
    [RegisterService(typeof(ITranslator<OntologyTerm, IBaseFintoItem>), RegisterType.Transient)]
    internal class FintoOntologyTermTranslator : FintoItemTranslator<OntologyTerm, OntologyTermName>
    {
        public FintoOntologyTermTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        protected override void TranslateDescription(ITranslationDefinitions<VmServiceViewsJsonItem, OntologyTerm> definition, Guid? id)
        {
            definition.AddCollectionWithRemove(i => i.Notes?.Select(x => GetLanguageText(string.Join(" ", x.Value), x.Key, id)) ?? new List<JsonLanguageLabel>(), o => o.Descriptions, x => true);
        }
    }

//    [RegisterService(typeof(ITranslator<OntologyTerm, VmServiceViewsJsonItem>), RegisterType.Transient)]
//    internal class FintoOntologyTermServiceViewsJsonTranslator : Translator<OntologyTerm, VmServiceViewsJsonItem>
//    {
//        private ILanguageCache languageCache;
//
//        public FintoOntologyTermServiceViewsJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
//        {
//            this.languageCache = languageCache;
//        }
//
//        public override OntologyTerm TranslateVmToEntity(VmServiceViewsJsonItem vModel)
//        {
//            return CreateViewModelEntityDefinition(vModel)
//                .DisableAutoTranslation()
//                //                .UseDataContextUpdate(i => true, i => o => i.Id == o.Uri, def => def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
//                .UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid())
//                .AddNavigation(i => i.Finnish, o => o.Label)
//                .AddNavigation(i => i.Id, o => o.Uri)
//                .AddNavigation(i => string.Join(";", i.BroaderURIs ?? new List<string>()), o => o.ParentUri)
//                .AddNavigation(i => i.Notation, o => o.Code)
//                .AddNavigation(i => i.ConceptType, o => o.OntologyType)
//                .AddCollection(i => i.Label.Select(x => GetLanguageText(x.Value, x.Key)), o => o.Names)
//                .GetFinal();
//        }
//
//        public override VmServiceViewsJsonItem TranslateEntityToVm(OntologyTerm entity)
//        {
//            return CreateEntityViewModelDefinition(entity)
//                .AddNavigation(i => i.Label, o => o.Label)
//                .AddNavigation(i => i.Uri, o => o.Id)
//                .AddNavigation(i => i.Code, o => o.Notation)
//                .AddNavigation(i => i.OntologyType, o => o.ConceptType)
//                .GetFinal();
//        }
//
//        private JsonLanguageLabel GetLanguageText(string text, string code)
//        {
//            return new JsonLanguageLabel
//            {
//                Label = text,
//                Lang = code
//            };
//        }
//    }

//    [RegisterService(typeof(ITranslator<OntologyTermName, VmLanguageText>), RegisterType.Transient)]
//    internal class OntologyTermNameTextTranslator : Translator<OntologyTermName, VmLanguageText>
//    {
//        public OntologyTermNameTextTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
//        {
//        }
//
//        public override OntologyTermName TranslateVmToEntity(VmLanguageText vModel)
//        {
//            return CreateViewModelEntityDefinition(vModel)
//                .DisableAutoTranslation()
//                .UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid())
//                .AddNavigation(i => i.Text, o => o.Name)
//                .AddSimple(i => i.LocalizationId, o => o.LocalizationId)
//                .GetFinal();
//        }
//
//        public override VmLanguageText TranslateEntityToVm(OntologyTermName entity)
//        {
//            throw new NotSupportedException();
//        }
//
//    }

}

