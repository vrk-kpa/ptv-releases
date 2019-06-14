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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationPhone, VmJsonSoteOrganization>), RegisterType.Transient)]
    internal class OrganizationSotePhoneTranslator : Translator<OrganizationPhone, VmJsonSoteOrganization>
    {
        private readonly ITypesCache typesCache;
        public OrganizationSotePhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonSoteOrganization TranslateEntityToVm(OrganizationPhone entity)
        {
            throw new NotSupportedException("Translator OrganizationPhone -> VmJsonSoteOrganization is not implemented.");
        }

        public override OrganizationPhone TranslateVmToEntity(VmJsonSoteOrganization vModel)
        {
            if (vModel == null) return null;
            
            var exists = vModel.Id.IsAssigned();
            var extraTypeId = typesCache.Get<ExtraType>(ExtraTypeEnum.Sote.ToString());

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output =>
                    output.Phone.ExtraTypes.Any(et => et.ExtraTypeId == extraTypeId) &&
                    output.OrganizationVersionedId == vModel.Id, def => def.UseDataContextCreate());

            var phoneId = Guid.Empty;
            return translation
                .Propagation((i, o) => { phoneId = o.PhoneId; })
                .AddNavigation(i => new VmPhone {Number = i.ContactInfo?.PhoneNumber, ExtraTypes = new List<Guid> {extraTypeId}, Id = phoneId}
                    , o => o.Phone)
                .GetFinal();
        }
    }
}