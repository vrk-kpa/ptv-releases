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
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, VmOrganizationListItem>), RegisterType.Transient)]
    internal class OrganizationListItemTranslator : Translator<Organization, VmOrganizationListItem>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public OrganizationListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.languageCache = languageCache;
        }

        private string GetParentName(Organization organization)
        {
            if (organization == null) return string.Empty;
            return languageCache.Filter(organization.OrganizationNames.Where(name => name.TypeId == organization.DisplayNameTypeId), RequestLanguageCode)?.Name;
        }

        public override VmOrganizationListItem TranslateEntityToVm(Organization entity)
        {
            return CreateEntityViewModelDefinition<VmOrganizationListItem>(entity)
                .AddNavigation(i => languageCache.Filter(i.OrganizationNames.Where(name => name.TypeId == i.DisplayNameTypeId), RequestLanguageCode)?.Name, o => o.Name)
                .AddNavigation(i =>
                {
                    var childrenNamesAll = i.Children.Select(child => child.OrganizationNames.First(k => k.Type.Code == NameTypeEnum.Name.ToString()).Name);
                    var result = childrenNamesAll.Any() ? childrenNamesAll.Aggregate((o1, o2) => o1 + ", " + o2) : string.Empty;
                    return result;
                }
                , o => o.SubOrganizations)
                .AddNavigation(i => GetParentName(i.Parent), o => o.MainOrganization)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddSimple(i => i.PublishingStatusId ?? Guid.Empty, o => o.PublishingStatusId)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(VmOrganizationListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
