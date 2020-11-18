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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceName, VmOpenApiLocalizedListItem>), RegisterType.Transient)]
    internal class OpenApiStatutoryServiceNameTranslator : OpenApiNameWithTypeBaseTranslator<StatutoryServiceName>
    {
        private readonly ITypesCache typesCache;

        public OpenApiStatutoryServiceNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(StatutoryServiceName entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override StatutoryServiceName TranslateVmToEntity(VmOpenApiLocalizedListItem vModel)
        {
            var langId = languageCache.Get(vModel.Language);
            var typeId = typesCache.Get<NameType>(vModel.Type.GetEnumValueByOpenApiEnumValue<NameTypeEnum>());
            return CreateViewModelEntityDefinition<StatutoryServiceName>(vModel)
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => o.TypeId == typeId && o.LocalizationId == langId &&
                    i.OwnerReferenceId == o.StatutoryServiceGeneralDescriptionVersionedId, name => name.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Name)
                .AddSimple(i => langId, o => o.LocalizationId)
                .AddSimple(i => typeId, o => o.TypeId)
                .GetFinal();
        }
    }
}
