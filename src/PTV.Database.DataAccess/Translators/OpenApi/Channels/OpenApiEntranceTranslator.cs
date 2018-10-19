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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq;
using System.Globalization;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<AccessibilityRegisterEntrance, V9VmOpenApiEntrance>), RegisterType.Transient)]
    internal class OpenApiEntranceTranslator : Translator<AccessibilityRegisterEntrance, V9VmOpenApiEntrance>
    {
        public OpenApiEntranceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives) { }

        public override V9VmOpenApiEntrance TranslateEntityToVm(AccessibilityRegisterEntrance entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.IsMain, o => o.IsMainEntrance)
                .AddCollection(i => i.Names, o => o.Name)
                .AddNavigation(i => i.Address?.Coordinates.FirstOrDefault(), o => o.Coordinates)
                .AddCollection(i => i.SentenceGroups.OrderBy(j => j.OrderNumber), o => o.AccessibilitySentences)
                .AddNavigation(i => i.IsMain ? i.AccessibilityRegister : null, o => o.ContactInfo)
                .GetFinal();
        }

        public override AccessibilityRegisterEntrance TranslateVmToEntity(V9VmOpenApiEntrance vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiEntranceTranslator");
        }
    }

    [RegisterService(typeof(ITranslator<Coordinate, VmOpenApiCoordinates>), RegisterType.Transient)]
    internal class OpenApiEntranceCoordinatesTranslator : Translator<Coordinate, VmOpenApiCoordinates>
    {
        public OpenApiEntranceCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives) { }

        public override VmOpenApiCoordinates TranslateEntityToVm(Coordinate entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Latitude.ToString(CultureInfo.InvariantCulture), o => o.Latitude)
                .AddNavigation(i => i.Longitude.ToString(CultureInfo.InvariantCulture), o => o.Longitude)
                .GetFinal();
        }

        public override Coordinate TranslateVmToEntity(VmOpenApiCoordinates vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiEntranceTranslator");
        }
    }
}
