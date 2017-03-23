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
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    abstract class OpenApiServiceChannelTranslator<TVmOpenApiServiceChannel> : OpenApiServiceChannelBaseTranslator<TVmOpenApiServiceChannel> where TVmOpenApiServiceChannel : class, IVmOpenApiServiceChannel
    {
        private readonly ITypesCache typesCache;

        protected OpenApiServiceChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiServiceChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return CreateChannelDefinitions(entity)
                .GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannel vModel)
        {
            return CreateVmToChannelDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<ServiceChannelVersioned, TVmOpenApiServiceChannel> CreateChannelDefinitions(ServiceChannelVersioned entity)
        {

            return CreateBaseDefinitions<TVmOpenApiServiceChannel>(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.ServiceChannelNames, o => o.ServiceChannelNames)
                .AddNavigation(i => typesCache.GetByValue<ServiceChannelType>(i.TypeId), o => o.ServiceChannelType);
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannel, ServiceChannelVersioned> CreateVmToChannelDefinitions(TVmOpenApiServiceChannel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel)
                .AddCollection(i => i.ServiceChannelNames, o => o.ServiceChannelNames)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(i.ServiceChannelType), o => o.TypeId);
        }

    }
}
