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
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, VmAddressSimpleBase>), RegisterType.Transient)]
    internal class BaseAddressTranslator : Translator<Address, VmAddressSimpleBase>
    {
        private readonly ITypesCache typesCache;

        public BaseAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimpleBase TranslateEntityToVm(Address entity)
        {
            var definition = CreateEntityViewModelDefinition<VmAddressSimpleBase>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.UniqueId, output => output.UniqueId)
                .AddCollection(input => GetCoordinates(input.Coordinates), output => output.Coordinates);

            return definition.GetFinal();
        }

        private IList<AddressCoordinate> GetCoordinates(ICollection<AddressCoordinate> coordinates)
        {
            var mainCoordinateType = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var mainCoordinates = coordinates.Where(x => x.TypeId == mainCoordinateType).ToList();
            var mainCoordinate = mainCoordinates.Count > 1
                ? mainCoordinates.FirstOrDefault(x => x.CoordinateState == CoordinateStates.Ok.ToString()) ??
                  mainCoordinates.FirstOrDefault()
                : mainCoordinates.FirstOrDefault();
            var filteredCoordinates = coordinates.Except(mainCoordinates).ToList();
            if (mainCoordinate != null)
            {
                filteredCoordinates.Add(mainCoordinate);
            }

            return filteredCoordinates;
        }
        public override Address TranslateVmToEntity(VmAddressSimpleBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}