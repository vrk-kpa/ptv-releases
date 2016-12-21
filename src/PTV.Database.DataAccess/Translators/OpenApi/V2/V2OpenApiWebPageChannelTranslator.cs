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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiWebPageChannel, VmOpenApiWebPageChannel>), RegisterType.Transient)]
    internal class V2OpenApiWebPageChannelTranslator : V2OpenApiServiceChannelTranslator<V2VmOpenApiWebPageChannel, VmOpenApiWebPageChannel>
    {
        public V2OpenApiWebPageChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiWebPageChannel TranslateEntityToVm(V2VmOpenApiWebPageChannel entity)
        {
            var vm = base.TranslateEntityToVm(entity);
            vm.Urls = entity.Urls;
            vm.SupportContacts = TranslateToV1SupportContacts(entity.SupportEmails, entity.SupportPhones);
            vm.ServiceHours = TranslateToV1ServiceHours(entity.ServiceHours);
            return vm;
        }

        public override V2VmOpenApiWebPageChannel TranslateVmToEntity(VmOpenApiWebPageChannel vModel)
        {
            var vm = base.TranslateVmToEntity(vModel);
            vm.Urls = vModel.Urls;
            vm.ServiceHours = TranslateToV2ServiceHours(vModel.ServiceHours);
            return vm;
        }
    }
}
