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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<Email, VmEmailData>), RegisterType.Transient)]
    internal class EmailTranslator : Translator<Email, VmEmailData>
    {
        public EmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override VmEmailData TranslateEntityToVm(Email entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Value, o => o.Email)
                .AddNavigation(i => i.Description, o => o.AdditionalInformation)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .GetFinal();
        }

        public override Email TranslateVmToEntity(VmEmailData vModel)
        {
            Guid id = vModel.Id ?? Guid.Empty;
            bool exists = vModel.Id.IsAssigned();
            if (vModel.LanguageId.IsAssigned())
            {
                SetLanguage(vModel.LanguageId.Value);
            }

            var translationDefinitions = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => id == o.Id)
                .AddNavigation(i => i.Email, o => o.Value)
                .AddNavigation(i => i.AdditionalInformation, o => o.Description)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .AddSimple(i => i.LanguageId.Value, o => o.LocalizationId);

//            if (vModel.LanguageId.IsAssigned())
//            {
//                translationDefinitions.AddSimple(i => i.LanguageId.Value, o => o.LocalizationId);
//            }

            var entity = translationDefinitions.GetFinal();
            return entity;
        }
    }
}
