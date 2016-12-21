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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
// it's allowed to use obsolete attributes for transaltions V1 <-> V2
#pragma warning disable 612, 618

    [RegisterService(typeof(ITranslator<V2VmOpenApiServiceLocationChannel, VmOpenApiServiceLocationChannel>), RegisterType.Transient)]
    internal class V2OpenApiServiceLocationChannelTranslator : V2OpenApiServiceChannelTranslator<V2VmOpenApiServiceLocationChannel, VmOpenApiServiceLocationChannel>
    {
        public V2OpenApiServiceLocationChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) :
            base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiServiceLocationChannel TranslateEntityToVm(V2VmOpenApiServiceLocationChannel entity)
        {
            var vm = base.TranslateEntityToVm(entity);
            vm.ServiceAreaRestricted = entity.ServiceAreaRestricted;
            vm.Email = entity.SupportEmails.FirstOrDefault()?.Value;

            var phone = entity.PhoneNumbers.FirstOrDefault(p => p.Type == PhoneNumberTypeEnum.Phone.ToString());
            if (phone != null)
            {
                vm.Phone = $"{ phone.PrefixNumber }{ phone.Number }";
                if (!string.IsNullOrEmpty(phone.ServiceChargeType))
                {
                    vm.ServiceChargeTypes = new List<string>() { phone.ServiceChargeType };
                }
                if (!string.IsNullOrEmpty(phone.ChargeDescription))
                {
                    vm.PhoneChargeDescriptions = new List<VmOpenApiLanguageItem>()
                    {
                        new VmOpenApiLanguageItem() { Value = phone.ChargeDescription, Language = phone.Language }
                    };

                }
            }
            var fax = entity.PhoneNumbers.FirstOrDefault(p => p.Type == PhoneNumberTypeEnum.Fax.ToString());
            if (fax != null)
            {
                vm.Fax = $"{ fax.PrefixNumber }{ fax.Number }";
            }

            // Coordinates
            var visitingAddress = entity.Addresses.Where(a => a.Type == AddressTypeEnum.Visiting.ToString()).FirstOrDefault();
            if (visitingAddress != null)
            {
                vm.Latitude = visitingAddress.Latitude;
                vm.Longitude = visitingAddress.Longitude;
            }

            vm.PhoneServiceCharge = entity.PhoneServiceCharge;
            vm.ServiceAreas = entity.ServiceAreas;
            vm.Addresses = new List<VmOpenApiAddressWithType>();
            entity.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion1()));
            vm.ServiceHours = TranslateToV1ServiceHours(entity.ServiceHours);
            return vm;
        }

        public override V2VmOpenApiServiceLocationChannel TranslateVmToEntity(VmOpenApiServiceLocationChannel vModel)
        {
            throw new NotImplementedException("Translation not implemented in V2OpenApiServiceLocationChannelTranslator!");
        }
    }

#pragma warning restore 612, 618
}
