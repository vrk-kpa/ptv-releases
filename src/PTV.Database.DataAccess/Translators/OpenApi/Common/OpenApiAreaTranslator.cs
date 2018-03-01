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
**/

using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Area, VmOpenApiArea>), RegisterType.Transient)]
    internal class OpenApiAreaTranslator : Translator<Area, VmOpenApiArea>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiAreaTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiArea TranslateEntityToVm(Area entity)
        {
            // Area types changed into Municipality, Region, BusinessSubRegion and HospitalDistrict(PTV-2184)
            var areaType = typesCache.GetByValue<AreaType>(entity.AreaTypeId).GetOpenApiEnumValue<AreaTypeEnum>();
            return CreateEntityViewModelDefinition(entity)
                    .AddNavigation(i => areaType, o => o.Type)
                    .AddNavigation(i => i.Code, o => o.Code)
                    .AddCollection(i => i.AreaNames, o => o.Name)
                    .AddCollection(i => i.AreaMunicipalities.Select(m => m.Municipality), o => o.Municipalities)
                    .GetFinal();
        }

        public override Area TranslateVmToEntity(VmOpenApiArea vModel)
        {
            var areaTypeId = typesCache.Get<AreaType>(vModel.Type);

            return CreateViewModelEntityDefinition<Area>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code.ToLower() == o.Code.ToLower() && areaTypeId == o.AreaTypeId)
                .GetFinal();
        }
    }
}
