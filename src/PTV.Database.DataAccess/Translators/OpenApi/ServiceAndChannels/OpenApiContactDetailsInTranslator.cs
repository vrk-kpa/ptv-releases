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

using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, V9VmOpenApiContactDetailsInBase>), RegisterType.Transient)]
    internal class OpenApiContactDetailsInTranslator : Translator<ServiceServiceChannel, V9VmOpenApiContactDetailsInBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiContactDetailsInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V9VmOpenApiContactDetailsInBase TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(V9VmOpenApiContactDetailsInBase vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(j => j.ServiceServiceChannelPhones).ThenInclude(j => j.Phone));

            if (vModel.DeleteAllAddresses || vModel.Addresses?.Count > 0)
            {
                var list = new List<V9VmOpenApiAddressIn>();
                var order = 1;
                vModel.Addresses.ForEach(a =>
                {
                    var address = a.ConvertToForeignAddress();
                    address.OrderNumber = order++;
                    list.Add(address);
                });
                definition.AddCollection(i => list, o => o.ServiceServiceChannelAddresses, false);
            }

            if (vModel.DeleteAllEmails || vModel.Emails?.Count > 0)
            {
                var emails = new List<V4VmOpenApiEmail>();
                var i = 1;
                vModel.Emails.ForEach(e => emails.Add(new V4VmOpenApiEmail()
                { Value = e.Value, Language = e.Language, OwnerReferenceId = e.OwnerReferenceId, OwnerReferenceId2 = e.OwnerReferenceId2, ExistsOnePerLanguage = false, OrderNumber = i++ }));
                definition.AddCollection(input => emails, output => output.ServiceServiceChannelEmails, false);
            }

            if (vModel.DeleteAllPhones || vModel.PhoneNumbers?.Count > 0)
            {
                var phoneNumbers = new List<V4VmOpenApiPhoneWithType>();
                var i = 1;
                vModel.PhoneNumbers.ForEach(p => phoneNumbers.Add(new V4VmOpenApiPhoneWithType()
                {
                    Number = p.Number,
                    PrefixNumber = p.PrefixNumber,
                    Language = p.Language,
                    OwnerReferenceId = p.OwnerReferenceId,
                    OwnerReferenceId2 = p.OwnerReferenceId2,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    ServiceChargeType = p.ServiceChargeType,
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    OrderNumber = i++,
                }));
                definition.AddCollectionWithRemove(input => phoneNumbers, output => output.ServiceServiceChannelPhones, r => r.Phone != null && r.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()));
            }
            if (vModel.FaxNumbers?.Count > 0 || vModel.DeleteAllFaxNumbers)
            {
                var faxNumbers = new List<V4VmOpenApiPhoneWithType>();
                var i = 1;
                vModel.FaxNumbers?.ForEach(f => faxNumbers.Add(new V4VmOpenApiPhoneWithType()
                {
                    Type = PhoneNumberTypeEnum.Fax.ToString(),
                    PrefixNumber = f.PrefixNumber,
                    Number = f.Number,
                    Language = f.Language,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(), // Service charge type cannot be null!
                    OwnerReferenceId = f.OwnerReferenceId,
                    OwnerReferenceId2 = f.OwnerReferenceId2,
                    OrderNumber = i++,
                }));
                definition.AddCollectionWithRemove(input => faxNumbers, output => output.ServiceServiceChannelPhones, r => r.Phone != null && r.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()));
            }

            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                // Check if the order of items is already set (PTV-3705)
                if (!vModel.WebPages.Any(w => w.OrderNumber != 0))
                {
                    var order = 1;
                    vModel.WebPages.ForEach(w => w.OrderNumber = order++);
                }
                definition.AddCollection(i => i.WebPages, o => o.ServiceServiceChannelWebPages, false);
            }

            return definition.GetFinal();
        }
    }
}