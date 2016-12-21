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
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    internal abstract class V2OpenApiServiceChannelTranslator<TFromModel, TToModel> : V2OpenApiServiceChannelBaseTranslator<TFromModel, TToModel>
        where TFromModel : class, IVmOpenApiServiceChannel, new() where TToModel : class, IVmOpenApiServiceChannel, new()
    {
        public V2OpenApiServiceChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TToModel TranslateEntityToVm(TFromModel entity)
        {
            var vm = base.TranslateEntityToVm(entity);
            vm.ServiceChannelType = entity.ServiceChannelType;
            vm.OrganizationId = entity.OrganizationId;
            vm.ServiceChannelNames = entity.ServiceChannelNames;
            return vm;
        }

        public override TFromModel TranslateVmToEntity(TToModel vModel)
        {
            var vm = base.TranslateVmToEntity(vModel);
            vm.ServiceChannelType = vModel.ServiceChannelType;
            vm.OrganizationId = vModel.OrganizationId;
            vm.ServiceChannelNames = vModel.ServiceChannelNames;
            return vm;
        }
    }
}
