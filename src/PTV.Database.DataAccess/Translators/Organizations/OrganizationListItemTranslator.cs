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
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationListItem>), RegisterType.Transient)]
    internal class OrganizationListItemTranslator : Translator<OrganizationVersioned, VmOrganizationListItem>
    {
        public OrganizationListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOrganizationListItem TranslateEntityToVm(OrganizationVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition<VmOrganizationListItem>(entity)
                .AddLocalizable(i => i.OrganizationNames.Where(name => name.TypeId == i.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId), o => o.Name)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatusId)
                .AddNavigation(i => i.Versioning, o => o.Version)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber), o => o.LanguagesAvailabilities);
            var model = definition.GetFinal();
            if (!model.Name.Any())
            {
                definition.AddDictionary(i => i.OrganizationNames, o => o.Name, k => languageCache.GetByValue(k.LocalizationId));
            }
            return model;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
