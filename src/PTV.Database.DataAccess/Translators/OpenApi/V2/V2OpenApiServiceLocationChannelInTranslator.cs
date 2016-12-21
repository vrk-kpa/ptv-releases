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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiServiceLocationChannelInBase, IVmOpenApiServiceLocationChannelInBase>), RegisterType.Transient)]
    internal class V2OpenApiServiceLocationChannelInTranslator : V2OpenApiServiceChannelInTranslator<V2VmOpenApiServiceLocationChannelInBase, IVmOpenApiServiceLocationChannelInBase>
    {
        public V2OpenApiServiceLocationChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IVmOpenApiServiceLocationChannelInBase TranslateEntityToVm(V2VmOpenApiServiceLocationChannelInBase entity)
        {
            throw new NotImplementedException("No translation implemented in V2OpenApiServiceLocationChannelInTranslator");
        }

        public override V2VmOpenApiServiceLocationChannelInBase TranslateVmToEntity(IVmOpenApiServiceLocationChannelInBase vModel)
        {
            var vm = base.TranslateVmToEntity(vModel);
            vm.ServiceAreaRestricted = vModel.ServiceAreaRestricted;
            vm.ServiceAreas = vModel.ServiceAreas;
            if (!string.IsNullOrEmpty(vModel.Email))
            {
                vm.SupportEmails.Add(new VmOpenApiEmail() { Value = vModel.Email, Language = LanguageCode.fi.ToString()});
            }
            // Set the phone numebers and the data related to it (service charge type, phone charge description etc...)
            if (!string.IsNullOrEmpty(vModel.Phone))
            {
                if (vModel.ServiceChargeTypes?.Count == 0)
                {
                    vModel.ServiceChargeTypes = new List<string>() { ServiceChargeTypeEnum.Charged.ToString() };
                }
                if (vModel.PhoneChargeDescriptions?.Count > 0)
                {
                    vModel.PhoneChargeDescriptions.ForEach(d =>
                    {
                        vm.PhoneNumbers.Add(new VmOpenApiPhone()
                        {
                            Number = vModel.Phone,
                            Language = d.Language,
                            ServiceChargeType = vModel.ServiceChargeTypes.FirstOrDefault(),
                            ChargeDescription = d.Value
                        });
                    });
                }
                else
                {
                    vm.PhoneNumbers.Add(new VmOpenApiPhone()
                    {
                        Number = vModel.Phone,
                        Language = LanguageCode.fi.ToString(),
                        ServiceChargeType = vModel.ServiceChargeTypes.FirstOrDefault()
                    });
                }
            }

            // Set fax
            if (!string.IsNullOrEmpty(vModel.Fax))
            {
                vm.FaxNumbers.Add(new VmOpenApiPhoneSimple() { Number = vModel.Fax, Language = LanguageCode.fi.ToString() });
            }

            vm.PhoneServiceCharge = vModel.PhoneServiceCharge;
            vm.Addresses = new List<V2VmOpenApiAddressWithType>();
            vModel.Addresses.ForEach(a => vm.Addresses.Add(a));
            vm.DeleteAllSupportEmails = vModel.DeleteEmail;
            vm.DeleteAllPhoneNumbers = vModel.DeletePhone;
            vm.DeleteAllFaxNumbers = vModel.DeleteFax;
            vm.ServiceHours = TranslateToV2ServiceHours(vModel.ServiceHours);
            return vm;
        }
    }
}
