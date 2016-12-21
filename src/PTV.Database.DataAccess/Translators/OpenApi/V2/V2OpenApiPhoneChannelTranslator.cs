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
    [RegisterService(typeof(ITranslator<V2VmOpenApiPhoneChannel, VmOpenApiPhoneChannel>), RegisterType.Transient)]
    internal class V2OpenApiPhoneChannelTranslator : V2OpenApiServiceChannelTranslator<V2VmOpenApiPhoneChannel, VmOpenApiPhoneChannel>
    {
        public V2OpenApiPhoneChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiPhoneChannel TranslateEntityToVm(V2VmOpenApiPhoneChannel entity)
        {
            var vm = base.TranslateEntityToVm(entity);
            var phone = entity.PhoneNumbers?.FirstOrDefault();
            if (phone != null)
            {
                vm.PhoneType = phone.Type;
                vm.ServiceChargeTypes = new List<string>() { phone.ServiceChargeType };
                vm.PhoneNumbers = new List<VmOpenApiLanguageItem>();
                vm.PhoneChargeDescriptions = new List<VmOpenApiLanguageItem>();
                entity.PhoneNumbers.ForEach(p => {
                    if (!string.IsNullOrEmpty(p.Number))
                    {
                        vm.PhoneNumbers.Add(new VmOpenApiLanguageItem()
                        {
                            Value = $"{p.PrefixNumber} {p.Number}",
                            Language = p.Language
                        });
                    }
                    if (!string.IsNullOrEmpty(p.ChargeDescription))
                        vm.PhoneChargeDescriptions.Add(new VmOpenApiLanguageItem()
                        {
                            Value = p.ChargeDescription,
                            Language = p.Language
                        });
                    });
            }
            vm.ServiceHours = TranslateToV1ServiceHours(entity.ServiceHours);
            vm.SupportContacts = TranslateToV1SupportContacts(entity.SupportEmails, null);
            return vm;
        }

        public override V2VmOpenApiPhoneChannel TranslateVmToEntity(VmOpenApiPhoneChannel vModel)
        {
            throw new NotImplementedException("No translation implementted in V2OpenApiPhoneChannelTranslator");
        }
    }
}
