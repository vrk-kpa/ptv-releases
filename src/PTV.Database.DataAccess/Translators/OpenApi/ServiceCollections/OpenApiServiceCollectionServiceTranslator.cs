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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceCollections
{
    [RegisterService(typeof(ITranslator<ServiceCollectionService, VmOpenApiServiceCollectionService>), RegisterType.Transient)]
    internal class OpenApiServiceCollectionServiceTranslator : Translator<ServiceCollectionService, VmOpenApiServiceCollectionService>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceCollectionServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceCollectionService TranslateEntityToVm(ServiceCollectionService entity)
        {
            var definition = CreateEntityViewModelDefinition(entity);

            if (entity.Service != null)
            {
                var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                definition.AddPartial(i => i.Service.Versions.Where(x => x.PublishingStatusId == publishedId).FirstOrDefault(), o => o);
            }
            else
            {
                var service = new ServiceVersioned() { UnificRootId = entity.ServiceId };
                definition.AddCollection(i => service.ServiceNames.ToList(), o => o.Name);
            }

            return definition.GetFinal();
        }

        public override ServiceCollectionService TranslateVmToEntity(VmOpenApiServiceCollectionService vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
            .UseDataContextUpdate(
                i => i.OwnerReferenceId.IsAssigned(),
                i => o => i.Id.Value == o.ServiceId && i.OwnerReferenceId.Value == o.ServiceCollectionVersionedId,
                x => x.UseDataContextCreate(i => true)
            )
            .AddSimple(i => i.Id.Value, o => o.ServiceId);

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceVersioned, VmOpenApiServiceCollectionService>), RegisterType.Transient)]
    internal class OpenApiServiceCollectionServiceItemTranslator : Translator<ServiceVersioned, VmOpenApiServiceCollectionService>
    {
        private ILanguageCache languageCache;
        private ITypesCache typeCache;

        public OpenApiServiceCollectionServiceItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typeCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceCollectionService TranslateEntityToVm(ServiceVersioned entity)
        {
            if (entity == null) return null;

            var nameTypeId = typeCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var descriptionTypeId = typeCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
            var vm = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.ServiceNames?.Where(n => n.TypeId == nameTypeId), o => o.Name)
                .AddCollection(i => i.ServiceDescriptions?.Where(n => n.TypeId == descriptionTypeId), o => o.Description)
                .GetFinal();

            return vm;
        }

        public override ServiceVersioned TranslateVmToEntity(VmOpenApiServiceCollectionService vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceCollectionServiceItemTranslator.");
        }
    }
}
