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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmJsonSoteOrganization>), RegisterType.Transient)]
    internal class OrganizationSoteTranslator : Translator<OrganizationVersioned, VmJsonSoteOrganization>
    {
        private readonly ITypesCache typesCache;
        
        public OrganizationSoteTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonSoteOrganization TranslateEntityToVm(OrganizationVersioned entity)
        {
            throw new NotImplementedException("Translator OrganizationVersioned -> VmJsonSoteOrganization is not implemented.");
        }

        public override OrganizationVersioned TranslateVmToEntity(VmJsonSoteOrganization vModel)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var organizationTypeId = typesCache.Get<OrganizationType>(vModel.Type);
            var areaInformationTypeId = typesCache.Get<AreaInformationType>(vModel.AreaInformationType);
            var extraTypeId = typesCache.Get<ExtraType>(ExtraTypeEnum.Sote.ToString());

            var translation = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(p => p.OrganizationPhones).ThenInclude(p => p.Phone).ThenInclude(et => et.ExtraTypes))
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), input => output => input.Id == output.Id)
                .UseVersioning<OrganizationVersioned, Organization>(o => o)
                .AddLanguageAvailability(i => i, o => o)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddSimple(i => organizationTypeId, o => o.TypeId)
                .AddSimple(i => areaInformationTypeId, o => o.AreaInformationTypeId)
                .AddSimple(i => i.MainOrganizationId, o => o.ParentId)
                .AddSimple(i => i.ValidFrom, o => o.ValidFrom)
                .AddSimple(i => i.ValidTo, o => o.ValidTo)
                .AddLocalizable(i => new VmName {Name = i.Name, TypeId = nameTypeId}, o => o.OrganizationNames)
                .AddCollectionWithRemove(i => i.ContactInfo == null || i.ContactInfo.PhoneNumber.IsNullOrEmpty()
                        ? new List<VmJsonSoteOrganization>()
                        : new List<VmJsonSoteOrganization> {i}
                    , o => o.OrganizationPhones,
                    x => x.Phone.ExtraTypes.Select(t => t.ExtraTypeId).Contains(extraTypeId));

            if (vModel.ContactInfo?.Address != null)
            {
                translation.AddCollectionWithRemove(i => i.ContactInfo?.Address == null
                        ? new List<VmJsonSoteOrganization>()
                        : new List<VmJsonSoteOrganization> {i}
                    , o => o.OrganizationAddresses,
                    x => x.Address.ExtraTypes.Select(t => t.ExtraTypeId).Contains(extraTypeId));
            }
            
            if (vModel.ServiceLocations != null)
            {
                translation.Propagation((i, o) =>
                {
                    i.ServiceLocations.ForEach(org => { org.OrganizationId = o.Id; });
                });
            }
            
            return translation.GetFinal();
        }
    }
}




