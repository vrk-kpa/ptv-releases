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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescription, VmGeneralDescriptionResults>), RegisterType.Transient)]
    internal class GeneralDescriptionResultsTranslator : Translator<StatutoryServiceGeneralDescription, VmGeneralDescriptionResults>
    {
        private ITypesCache typesCache;

        public GeneralDescriptionResultsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmGeneralDescriptionResults TranslateEntityToVm(StatutoryServiceGeneralDescription entity)
        {
            SetLanguage(LanguageCode.fi);
            return CreateEntityViewModelDefinition(entity)
                .AddLocalizable(i => i.Names.Where(j => typesCache.Compare<NameType>(j.TypeId, NameTypeEnum.Name.ToString())), o => o.Name)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.Description.ToString())), o => o.Description)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.ShortDescription.ToString())), o => o.ShortDescription)
                .AddSimpleList(i => i.TargetGroups.Select(m => m.TargetGroupId), o => o.TargetGroups)
                .AddSimpleList(i => i.ServiceClasses.Select(j => j.ServiceClassId), o => o.ServiceClasses)
                .GetFinal();

        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(VmGeneralDescriptionResults vModel)
        {
            throw new NotImplementedException();
        }
    }
}
