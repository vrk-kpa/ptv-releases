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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto.Base
{
    internal abstract class FintoJsonItemTranslator<TFintoItem, TName> : Translator<TFintoItem, VmFintoJsonItem> where TFintoItem : FintoItemBase<TFintoItem>, IFintoItemNames<TName>, IFintoItem where TName : NameBase
    {
        protected FintoJsonItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        protected ITranslationDefinitions<VmFintoJsonItem, TFintoItem> GetDefaultMappingViewModelToEntity(VmFintoJsonItem vModel)
        {
            return CreateViewModelEntityDefinition<TFintoItem>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, input => output => input.Id == output.Uri, def => def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid()))
//                .UseDataContext(i => i.Id, o => o.Uri)
                .AddNavigation(i => i.Label, o => o.Label)
                .AddNavigation(i => i.Id, o => o.Uri)
                .AddNavigation(i => i.Parents?.FirstOrDefault(), o => o.ParentUri)
                .AddNavigation(i => i.Notation, o => o.Code)
                .AddNavigation(i => i.OntologyType, o => o.OntologyType)
                .AddCollection(i => new List<string> { i.Finnish }, o => o.Names)
                .AddCollection(i => i.Narrower ?? new List<VmFintoJsonItem>(), o => o.Children)
                ;
        }

        public override TFintoItem TranslateVmToEntity(VmFintoJsonItem vModel)
        {
            var entity = GetDefaultMappingViewModelToEntity(vModel).GetFinal();
            entity.Children.ForEach(x => x.Parent = entity);
            return entity;
        }

        public override VmFintoJsonItem TranslateEntityToVm(TFintoItem entity)
        {
            return CreateEntityViewModelDefinition<VmFintoJsonItem>(entity)
                .AddNavigation(i => i.Label, o => o.Label)
                .AddNavigation(i => i.Uri, o => o.Id)
                .AddNavigation(i => i.Code, o => o.Notation)
                .AddNavigation(i => i.OntologyType, o => o.OntologyType)
                .GetFinal();
        }
    }
    internal abstract class FintoItemTranslator<TFintoItem, TName> : Translator<TFintoItem, VmServiceViewsJsonItem> where TFintoItem : FintoItemBase, IFintoItemNames<TName> where TName : NameBase
    {
        protected FintoItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TFintoItem TranslateVmToEntity(VmServiceViewsJsonItem vModel)
        {
            bool isNew = false;
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Id == o.Uri, def =>
                {
                    def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid());
                    isNew = true;
                })
                .AddNavigation(i => i.Label.TryGet(DomainConstants.DefaultLanguage), o => o.Label)
                .AddNavigation(i => i.Id, o => o.Uri)
                .AddNavigation(i => string.Join(";", i.BroaderURIs ?? new List<string>()), o => o.ParentUri)
                .AddNavigation(i => i.Notation, o => o.Code)
                .AddNavigation(i => i.ConceptType, o => o.OntologyType)
                .AddSimple(i => true, o => o.IsValid);
            var entity = definition.GetFinal();

            definition.AddCollectionWithRemove(i => i.Label.Select(x => GetLanguageText(x.Value, x.Key, isNew ? (Guid?)null : entity.Id)), o => o.Names, x => true);

            TranslateDescription(definition, isNew ? (Guid?)null : entity.Id);
            return entity;
        }

        protected virtual void TranslateDescription(ITranslationDefinitions<VmServiceViewsJsonItem, TFintoItem> definition, Guid? id)
        {

        }

        public override VmServiceViewsJsonItem TranslateEntityToVm(TFintoItem entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Label, o => o.Label)
                .AddNavigation(i => i.Uri, o => o.Id)
                .AddNavigation(i => i.Code, o => o.Notation)
                .AddNavigation(i => i.OntologyType, o => o.ConceptType)
                .GetFinal();
        }

        protected JsonLanguageLabel GetLanguageText(string text, string code, Guid? referenceId)
        {
            return new JsonLanguageLabel
            {
                Label = text,
                Lang = code,
                OwnerReferenceId = referenceId
            };
        }
    }
    internal abstract class FintoReplacedItemTranslator<TFintoItem> : Translator<TFintoItem, VmReplaceItemServiceViewsJsonItem> where TFintoItem : FintoItemBase
    {
        protected FintoReplacedItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TFintoItem TranslateVmToEntity(VmReplaceItemServiceViewsJsonItem vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => vModel.Item.Id == o.Uri || i.ReplacedBy.Id == o.Uri, def =>
                {
                    def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid());
                })
                .AddPartial(i => i.ReplacedBy);
            var entity = definition.GetFinal();

            return entity;
        }


        public override VmReplaceItemServiceViewsJsonItem TranslateEntityToVm(TFintoItem entity)
        {
            throw new NotImplementedException();
        }
    }

}
