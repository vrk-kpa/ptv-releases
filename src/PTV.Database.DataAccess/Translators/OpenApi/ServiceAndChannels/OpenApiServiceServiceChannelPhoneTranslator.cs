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
**/

using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelPhone, V4VmOpenApiPhoneWithType>), RegisterType.Transient)]
    internal class OpenApiServiceServiceChannelPhoneTranslator : Translator<ServiceServiceChannelPhone, V4VmOpenApiPhoneWithType>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceServiceChannelPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiPhoneWithType TranslateEntityToVm(ServiceServiceChannelPhone entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceServiceChannelPhoneTranslator!");
        }

        public override ServiceServiceChannelPhone TranslateVmToEntity(V4VmOpenApiPhoneWithType vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.OwnerReferenceId.IsAssigned() && vModel.OwnerReferenceId2.IsAssigned();
            Guid typeId = typesCache.Get<PhoneNumberType>(vModel.Type);

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => i.OwnerReferenceId == o.ServiceId && i.OwnerReferenceId2 == o.ServiceChannelId && typeId == o.Phone.TypeId &&
                    languageCache.Get(i.Language) == o.Phone.LocalizationId && i.Number == o.Phone.Number && i.PrefixNumber == o.Phone.PrefixNumber.Code,
                    e => e.UseDataContextCreate(x => true));

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
