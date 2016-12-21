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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Phone, VmOpenApiPhoneWithType>), RegisterType.Transient)]
    internal class OpenApiPhoneWithTypeTranslator : Translator<Phone, VmOpenApiPhoneWithType>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiPhoneWithTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiPhoneWithType TranslateEntityToVm(Phone entity)
        {
            throw new NotImplementedException("Phone -> VmOpenApiPhone translator is not implemented.");
        }

        public override Phone TranslateVmToEntity(VmOpenApiPhoneWithType vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var serviceChargeType = (string.IsNullOrEmpty(vModel.ServiceChargeType))
                ? typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString())
                : typesCache.Get<ServiceChargeType>(vModel.ServiceChargeType);

            var phoneNumberType = (string.IsNullOrEmpty(vModel.Type))
                ? typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())
                : typesCache.Get<PhoneNumberType>(vModel.Type);

            var translationDefinitions = CreateViewModelEntityDefinition<Phone>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id)
                .AddNavigation(i => i.Number, o => o.Number)
                .AddSimple(i => serviceChargeType, o => o.ServiceChargeTypeId)
                .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
                .AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .AddSimple(i => phoneNumberType, o => o.TypeId);

            return translationDefinitions.GetFinal();
        }
    }
}
