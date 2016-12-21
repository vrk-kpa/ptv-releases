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
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, IV2VmOpenApiServiceLocationChannelInBase>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelMainInTranslator : OpenApiServiceChannelInTranslator<IV2VmOpenApiServiceLocationChannelInBase>
    {

        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;


        public OpenApiServiceLocationChannelMainInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }
        public override IV2VmOpenApiServiceLocationChannelInBase TranslateEntityToVm(ServiceChannel entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ServiceChannel TranslateVmToEntity(IV2VmOpenApiServiceLocationChannelInBase vModel)
        {
            var definitions = CreateVmToChannelDefinitions(vModel)
                .AddNavigation(i => ServiceChannelTypeEnum.ServiceLocation.ToString(), o => o.Type)
                .AddNavigationOneMany(i => i, o => o.ServiceLocationChannels);

            // Set fax and phone numbers
            var phoneNumbers = new List<VmOpenApiPhoneWithType>();
            if (vModel.PhoneNumbers?.Count > 0)
            {
                vModel.PhoneNumbers.ForEach(p => phoneNumbers.Add(new VmOpenApiPhoneWithType()
                {
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    PrefixNumber = p.PrefixNumber,
                    Number = p.Number,
                    Language = p.Language,
                    ServiceChargeType = p.ServiceChargeType,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    OwnerReferenceId = vModel.Id
                }));
            }
            if (vModel.FaxNumbers?.Count > 0)
            {
                vModel.FaxNumbers.ForEach(f => phoneNumbers.Add(new VmOpenApiPhoneWithType()
                {
                    Type = PhoneNumberTypeEnum.Fax.ToString(),
                    PrefixNumber = f.PrefixNumber,
                    Number = f.Number,
                    Language = f.Language,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(), // Service charge type cannot be null!
                    OwnerReferenceId = vModel.Id
                }));
            }
            if (phoneNumbers.Count > 0)
            {
                definitions.AddCollection(i => phoneNumbers, o => o.Phones);
            }
            return definitions.GetFinal();
        }
    }
}
