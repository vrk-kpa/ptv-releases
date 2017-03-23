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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelPhone, V4VmOpenApiPhoneWithType>), RegisterType.Transient)]
    internal class OpenApiServiceChannelPhoneWithTypeTranslator : OpenApiServiceChannelPhoneBaseTranslator<V4VmOpenApiPhoneWithType>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelPhoneWithTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiPhoneWithType TranslateEntityToVm(ServiceChannelPhone entity)
        {
            return base.CreateBaseDefinitions(entity)
                .AddNavigation(i => typesCache.GetByValue<PhoneNumberType>(i.Phone.TypeId), o => o.Type)
                .GetFinal();
        }

        public override ServiceChannelPhone TranslateVmToEntity(V4VmOpenApiPhoneWithType vModel)
        {
            var exists = vModel.OwnerReferenceId.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel);

            if (vModel.ExistsOnePerLanguage)
            {
                definition.UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => i.OwnerReferenceId == o.ServiceChannelVersionedId &&
                    languageCache.Get(i.Language) == o.Phone.LocalizationId, e => e.UseDataContextCreate(x => true));
            }
            else
            {
                Guid typeId = typesCache.Get<PhoneNumberType>(vModel.Type);
                definition.UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => i.OwnerReferenceId == o.ServiceChannelVersionedId && typeId == o.Phone.TypeId &&
                    languageCache.Get(i.Language) == o.Phone.LocalizationId && i.Number == o.Phone.Number && i.PrefixNumber == o.Phone.PrefixNumber.Code,
                    e => e.UseDataContextCreate(x => true));
            }

            if (exists)
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue)
                {
                    vModel.Id = entity.PhoneId;
                }
            }

            definition.AddNavigation(i => i, o => o.Phone);

            return definition.GetFinal();
        }
    }
}
