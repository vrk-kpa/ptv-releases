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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiElectronicChannelInBase, IVmOpenApiElectronicChannelInBase>), RegisterType.Transient)]
    internal class V2OpenApiEChannelInTranslator : V2OpenApiServiceChannelInTranslator<V2VmOpenApiElectronicChannelInBase, IVmOpenApiElectronicChannelInBase>
    {
        public V2OpenApiEChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IVmOpenApiElectronicChannelInBase TranslateEntityToVm(V2VmOpenApiElectronicChannelInBase entity)
        {
            throw new NotImplementedException("No translation implemented in V2OpenApiEChannelInTranslator");
        }

        public override V2VmOpenApiElectronicChannelInBase TranslateVmToEntity(IVmOpenApiElectronicChannelInBase vModel)
        {
            var vm = base.TranslateVmToEntity(vModel);
            vm.SignatureQuantity = vModel.SignatureQuantity;
            vm.RequiresSignature = vModel.RequiresSignature;
            vm.RequiresAuthentication = vModel.RequiresAuthentication;
            vm.Urls = vModel.Urls;
            vm.Attachments = vModel.Attachments;
            vm.DeleteAllAttachments = vModel.DeleteAllAttachments;
            vm.SupportEmails = TranslateToV2SupportEmails(vModel.SupportContacts);
            vm.SupportPhones = TranslateToV2SupportPhones(vModel.SupportContacts);
            vm.DeleteAllSupportEmails = vModel.DeleteAllSupportContacts;
            vm.DeleteAllSupportPhones = vModel.DeleteAllSupportContacts;
            vm.ServiceHours = TranslateToV2ServiceHours(vModel.ServiceHours);
            return vm;
        }
    }
}
