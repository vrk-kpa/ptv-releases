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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiServiceLocationChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelMainInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiServiceLocationChannelInVersionBase>
    {

        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;


        public OpenApiServiceLocationChannelMainInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }
        public override VmOpenApiServiceLocationChannelInVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiServiceLocationChannelInVersionBase vModel)
        {
            var definitions = CreateVmToChannelDefinitions(vModel)
                .AddNavigation(i => ServiceChannelTypeEnum.ServiceLocation.ToString(), o => o.Type);

            // Update oid info if it is set (PTV-3841, PTV-4439). The updating of Oid is handeled in SaveServiceChannel method (ChannelService)
            if (!string.IsNullOrEmpty(vModel.Oid) && !vModel.Id.HasValue)
            {
                definitions.AddNavigation(i => new VmOpenApiStringItem { Value = i.Oid }, o => o.UnificRoot);
            }

            // Set display name type
            if (vModel.DisplayNameType?.Count > 0)
            {
                if (vModel.VersionId.IsAssigned())
                {
                    vModel.DisplayNameType.ForEach(d => d.OwnerReferenceId = vModel.VersionId);
                }
                definitions.AddCollectionWithKeep(i => i.DisplayNameType, o => o.DisplayNameTypes, x => true);
            }

            // Set fax and phone numbers
            var order = 1;
            if (vModel.PhoneNumbers?.Count > 0 || vModel.DeleteAllPhoneNumbers)
            {
                var phoneNumbers = new List<V4VmOpenApiPhoneWithType>();
                vModel.PhoneNumbers?.ForEach(p => phoneNumbers.Add(new V4VmOpenApiPhoneWithType()
                {
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    PrefixNumber = p.PrefixNumber,
                    Number = p.Number,
                    Language = p.Language,
                    ServiceChargeType = p.ServiceChargeType,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    OwnerReferenceId = vModel.VersionId,
                    OrderNumber = order++,
                }));
                definitions.AddCollectionWithRemove(i => phoneNumbers, o => o.Phones, r => r.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()));
            }
            if (vModel.FaxNumbers?.Count > 0 || vModel.DeleteAllFaxNumbers)
            {
                var faxNumbers = new List<V4VmOpenApiPhoneWithType>();
                vModel.FaxNumbers?.ForEach(f => faxNumbers.Add(new V4VmOpenApiPhoneWithType()
                {
                    Type = PhoneNumberTypeEnum.Fax.ToString(),
                    PrefixNumber = f.PrefixNumber,
                    Number = f.Number,
                    Language = f.Language,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(), // Service charge type cannot be null!
                    OwnerReferenceId = vModel.VersionId,
                    OrderNumber = order++,
                }));
                definitions.AddCollectionWithRemove(i => faxNumbers, o => o.Phones, r => r.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()));
            }
            
            // Set addresses
            if (vModel.Addresses != null && vModel.Addresses.Count > 0)
            {
                var list = new List<V9VmOpenApiAddressIn>();
                order = 1;
                vModel.Addresses.ForEach(item => list.Add(new V9VmOpenApiAddressIn
                {
                    Type = item.Type,
                    SubType = item.SubType,
                    StreetAddress = item.StreetAddress,
                    PostOfficeBoxAddress = item.PostOfficeBoxAddress,
                    OtherAddress = item.OtherAddress,
                    ForeignAddress = item.LocationAbroad,
                    Country = item.Country,
                    OrderNumber = order++,
                }));
                definitions.AddCollectionWithRemove(i => list, o => o.Addresses, x => true);
            }

            return definitions.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannel, VmOpenApiStringItem>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelSocialHeathcareTranslator : Translator<ServiceChannel, VmOpenApiStringItem>
    {

        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;


        public OpenApiServiceLocationChannelSocialHeathcareTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }
        public override VmOpenApiStringItem TranslateEntityToVm(ServiceChannel entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceLocationChannelSocialHeathcareTranslator");
        }

        public override ServiceChannel TranslateVmToEntity(VmOpenApiStringItem vModel)
        {
            return base.CreateViewModelEntityDefinition(vModel)
                .AddNavigationOneMany(i => i, o => o.SocialHealthCenters)
                .GetFinal();
        }
    }
}