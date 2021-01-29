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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, V7VmOpenApiAddressContact>), RegisterType.Transient)]
    internal class OpenApiAddressContactTranslator : OpenApiAddressBaseTranslator<V7VmOpenApiAddressContact>
    {
        public OpenApiAddressContactTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
        }

        public override V7VmOpenApiAddressContact TranslateEntityToVm(Address entity)
        {
            // subtype 'Foreign' -> 'Abroad' PTV-3914
            var definition = CreateBaseEntityVmDefinitions(entity, true);
            if (entity.AddressForeigns?.Count > 0)
            {
                definition.AddCollection(i => i.AddressForeigns.FirstOrDefault()?.ForeignTextNames, o => o.LocationAbroad);
            }
            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(V7VmOpenApiAddressContact vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressContactTranslator");
        }
    }
}
