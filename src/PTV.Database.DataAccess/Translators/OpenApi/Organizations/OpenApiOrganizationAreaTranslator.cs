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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework.Interfaces;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationArea, VmOpenApiArea>), RegisterType.Transient)]
    internal class OpenApiOrganizationAreaTranslator : Translator<OrganizationArea, VmOpenApiArea>
    {
        private readonly ITypesCache typesCache;

        public OpenApiOrganizationAreaTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiArea TranslateEntityToVm(OrganizationArea entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationAreaTranslator!");
        }

        public override OrganizationArea TranslateVmToEntity(VmOpenApiArea vModel)
        {
            var exists = vModel.OwnerReferenceId.IsAssigned();
            var areaTypeId = typesCache.Get<AreaType>(vModel.Type);

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.OwnerReferenceId == output.OrganizationVersionedId &&
                    input.Code == output.Area.Code && areaTypeId == output.Area.AreaTypeId, def => def.UseDataContextCreate(x => true));
            
            definition.AddNavigation(input => input, output => output.Area);
            return definition.GetFinal();
        }
    }
}
