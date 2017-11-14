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
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Phone, V4VmOpenApiPhoneWithType>), RegisterType.Transient)]
    internal class OpenApiPhoneChannelPhoneTranslator : VmOpenApiPhoneChannelPhoneBaseTranslator<V4VmOpenApiPhoneWithType>
    {
        private readonly ITypesCache typesCache;

        public OpenApiPhoneChannelPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiPhoneWithType TranslateEntityToVm(Phone entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override Phone TranslateVmToEntity(V4VmOpenApiPhoneWithType vModel)
        {
            var phoneNumberType = (string.IsNullOrEmpty(vModel.Type))
                ? typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())
                : typesCache.Get<PhoneNumberType>(vModel.Type);

            var translationDefinitions = base.CreateVmEntityBaseDefinitions(vModel)
                .AddSimple(i => phoneNumberType, o => o.TypeId)
                .AddNavigation(i => i.PrefixNumber, o => o.PrefixNumber);

            return translationDefinitions.GetFinal();
        }
    }
}
