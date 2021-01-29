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
    [RegisterService(typeof(ITranslator<ClsStreetNumberCoordinate, VmCoordinate>), RegisterType.Transient)]
    internal class ClsStreetNumberCoordinateTranslator : Translator<ClsStreetNumberCoordinate, VmCoordinate>
    {
        private readonly ITypesCache typesCache;

        public ClsStreetNumberCoordinateTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmCoordinate TranslateEntityToVm(ClsStreetNumberCoordinate entity)
        {
            var mainCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var model = CreateEntityViewModelDefinition<VmCoordinate>(entity);
            var result = model.AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.TypeId == mainCoordinateTypeId, output => output.IsMain)
                .AddSimple(input => input.TypeId, output => output.TypeId)
                .GetFinal();
            return result;
        }

        public override ClsStreetNumberCoordinate TranslateVmToEntity(VmCoordinate vModel)
        {
            if (vModel?.OwnerReferenceId == null) return null;

            Enum.TryParse(vModel.CoordinateState, true, out CoordinateStates cState);
            var coordinateState = !string.IsNullOrEmpty(vModel.CoordinateState) ? cState.ToString() : CoordinateStates.EnteredByUser.ToString();

            var mainCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextLocalizedUpdate(i => true,
                    i => o => i.Id.IsAssigned() ? (i.Id == o.Id) : (i.OwnerReferenceId == o.RelatedToId),
                    def => def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid()))
                .AddNavigation(input => coordinateState, output => output.CoordinateState)
                .AddSimple(input => input.Latitude, output => output.Latitude)
                .AddSimple(input => input.Longitude, output => output.Longitude)
                .AddSimple(input => mainCoordinateTypeId, output => output.TypeId)
                .AddSimple(input => input.OwnerReferenceId ?? Guid.Empty, output => output.RelatedToId)
                .GetFinal();

            return definition;

        }
    }
}
