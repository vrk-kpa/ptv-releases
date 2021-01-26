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
using Microsoft.EntityFrameworkCore.Internal;
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
    [RegisterService(typeof(ITranslator<DigitalAuthorization, VmRovaJsonItem>), RegisterType.Transient)]
    internal class FintoDigitalAuthorizationTranslator : Translator<DigitalAuthorization, VmRovaJsonItem>
    {
        public FintoDigitalAuthorizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override DigitalAuthorization TranslateVmToEntity(VmRovaJsonItem vModel)
        {
            bool isNew = false;
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Uri.ToLower() == o.Uri.ToLower(), def =>
                {
                    def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid());
                    isNew = true;
                })
                .AddNavigation(i => i.Labels.Where(x => x.Lang == DomainConstants.DefaultLanguage).Select(x => x.Label).FirstOrDefault(), o => o.Label)
                .AddNavigation(i => i.Uri.ToLower(), o => o.Uri)
                .AddNavigation(i => i.BroaderConcepts?.Join(";").ToLower(), o => o.ParentUri)
                .AddNavigation(i => i.Id.ToString(), o => o.Code)
                .AddSimple(i => true, o => o.IsValid);
            var entity = definition.GetFinal();
            if (!isNew)
            {
                vModel.Labels.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            definition.AddCollection(i => i.Labels, o => o.Names);

            return entity;
        }

        public override VmRovaJsonItem TranslateEntityToVm(DigitalAuthorization entity)
        {
            throw new NotSupportedException();
        }

    }
}
