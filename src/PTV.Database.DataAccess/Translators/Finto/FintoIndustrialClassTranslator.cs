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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Finto.Base;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<IndustrialClass, VmIndustrialClassJsonItem>), RegisterType.Transient)]
    internal class FintoIndustrialClassTranslatorOld : Translator<IndustrialClass, VmIndustrialClassJsonItem>
    {
        public FintoIndustrialClassTranslatorOld(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IndustrialClass TranslateVmToEntity(VmIndustrialClassJsonItem vModel)
        {
            bool isNew = false;
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Uri || i.UriCode == o.Uri, def =>
                {
                    def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid());
                    isNew = true;
                })
                .AddNavigation(i => i.Names.Where(x => x.Lang == DomainConstants.DefaultLanguage).Select(x => x.Label).FirstOrDefault(), o => o.Label)
                .AddNavigation(i => i.UriCode, o => o.Uri)
                .AddNavigation(i => i.ParentUri, o => o.ParentUri)
                .AddNavigation(i => i.Level.ToString(), o => o.Code)
                .AddSimple(i => i.Level, o => o.OrderNumber)
//                .AddNavigation(i => "IndustrialClass", o => o.OntologyType)

                .AddCollection(i => i.Children ?? new List<VmIndustrialClassJsonItem>(), o => o.Children);

                var entity = definition.GetFinal();
            definition.AddCollection(
                i =>
                    i.Names.Select(x =>
                        new JsonLanguageLabel
                        {
                            Label = x.Label,
                            OwnerReferenceId = isNew ? (Guid?) null : entity.Id,
                            Lang = x.Lang
                        })
                    , o => o.Names);
            return entity;
        }

        public override VmIndustrialClassJsonItem TranslateEntityToVm(IndustrialClass entity)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(ITranslator<IndustrialClass, VmServiceViewsJsonItem>), RegisterType.Transient)]
    [RegisterService(typeof(ITranslator<IndustrialClass, IBaseFintoItem>), RegisterType.Transient)]
    internal class FintoIndustrialClassTranslator : FintoItemTranslator<IndustrialClass, IndustrialClassName>
    {
        public FintoIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
    }

    [RegisterService(typeof(ITranslator<IndustrialClass, VmReplaceItemServiceViewsJsonItem>), RegisterType.Transient)]
    internal class ReplaceFintoIndustrialClassTranslator : FintoReplacedItemTranslator<IndustrialClass>
    {
        public ReplaceFintoIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
    }


}
