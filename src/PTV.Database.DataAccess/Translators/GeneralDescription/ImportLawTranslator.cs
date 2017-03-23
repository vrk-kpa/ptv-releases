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
            throw new NotSupportedException();

        }

        public override Law TranslateVmToEntity(ImportLaw vModel)
        {
            bool newEntity = false;
            string name = vModel.Names.FirstOrDefault(x => x.Lang == LanguageCode.fi.ToString())?.Label;

            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id.Value)
                .UseDataContextUpdate(i => !i.Id.IsAssigned(), i => o => o.Names.Any(x => x.Name == name) && o.StatutoryServiceLaws.Any(), def =>
                {
                    newEntity = true;
                    def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid());
                });
            var entity = definition.GetFinal();
            if (!newEntity)
            {
                vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
                vModel.Links.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            definition.AddCollection(i => i.Names, o => o.Names)
                .AddCollection(i => i.Links, o => o.WebPages)
                .GetFinal();

            vModel.Id = entity.Id;
            return entity;
        }
    }
}