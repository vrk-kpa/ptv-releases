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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmOpenApiContactDetailsInBase>), RegisterType.Transient)]
    internal class OpenApiContactDetailsInTranslator : Translator<ServiceServiceChannel, VmOpenApiContactDetailsInBase>
    {
        public OpenApiContactDetailsInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiContactDetailsInBase TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmOpenApiContactDetailsInBase vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel);

            if (vModel.DeleteAllAddresses || vModel.Addresses?.Count > 0)
            {
                var list = new List<V7VmOpenApiAddressWithForeignIn>();
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

            if (vModel.DeleteAllPhones || vModel.Phones?.Count > 0)
            {
                var phones = new List<V4VmOpenApiPhoneWithType>();
                var i = 1;
                vModel.Phones.ForEach(p => phones.Add(new V4VmOpenApiPhoneWithType()
                {
                    Number = p.Number,
                    PrefixNumber = p.PrefixNumber,
                    Language = p.Language,
                    OwnerReferenceId = p.OwnerReferenceId,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    ServiceChargeType = p.ServiceChargeType,
                    ExistsOnePerLanguage = false,
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    OrderNumber = i++,
                }));

                definition.AddCollection(input => phones, output => output.ServiceServiceChannelPhones, false);
            }

            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.ServiceServiceChannelWebPages, false);
            }

            return definition.GetFinal();
        }
    }
}
