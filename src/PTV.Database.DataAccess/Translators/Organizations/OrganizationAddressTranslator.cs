﻿/**
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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationAddress, VmAddressSimple>), RegisterType.Transient)]
    internal class OrganizationAddressTranslator : Translator<OrganizationAddress, VmAddressSimple>
    {
        private ITypesCache typesCache;

        public OrganizationAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimple TranslateEntityToVm(OrganizationAddress entity)
        {
            throw new NotSupportedException();
        }

        public override OrganizationAddress TranslateVmToEntity(VmAddressSimple vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition<OrganizationAddress>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output => input.Id == output.AddressId && input.OwnerReferenceId == output.OrganizationVersionedId, def => def.UseDataContextCreate(input => true))
                .AddSimple(input => typesCache.Get<AddressCharacter>(input.AddressCharacter.ToString()), output => output.CharacterId)
                .AddNavigation(input => input, output => output.Address)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }

    }
}
