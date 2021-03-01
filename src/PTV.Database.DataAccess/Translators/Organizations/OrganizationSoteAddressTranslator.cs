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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
/* SOTE has been disabled (SFIPTV-1177)
    [RegisterService(typeof(ITranslator<OrganizationAddress, VmJsonSoteOrganization>), RegisterType.Transient)]
    internal class OrganizationSoteAddressTranslator : Translator<OrganizationAddress, VmJsonSoteOrganization>
    {
        private readonly ITypesCache typesCache;

        public OrganizationSoteAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonSoteOrganization TranslateEntityToVm(OrganizationAddress entity)
        {
            throw new NotSupportedException("Translator OrganizationAddress -> VmJsonSoteOrganization is not implemented.");
        }

        public override OrganizationAddress TranslateVmToEntity(VmJsonSoteOrganization vModel)
        {
            if (vModel?.ContactInfo?.Address == null) return null;
//            var extraTypeId = typesCache.Get<ExtraType>(ExtraTypeEnum.Sote.ToString());

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => true,
                    input => output => output.Address.ExtraTypes.Any(et => et.ExtraTypeId == extraTypeId) && output.OrganizationVersionedId == input.Id,
                    def => def.UseDataContextCreate(x => true))
                .AddSimple(i => typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString()), output => output.CharacterId)
                .AddPartial(i => i.ContactInfo?.Address as IVmOrderable, o => o as IOrderable);

            translation.Propagation((i, o) => { if (o.Created != DateTime.MinValue) i.ContactInfo.Address.Id = o.AddressId; });
            translation.AddNavigation(i => i.ContactInfo.Address, o => o.Address);

            return translation.GetFinal();
        }
    }
*/
}
