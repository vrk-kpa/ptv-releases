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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceWebPage, VmServiceVoucher>), RegisterType.Transient)]
    internal class ServiceWebPageServiceVoucherTranslator : Translator<ServiceWebPage, VmServiceVoucher>
    {
        public ServiceWebPageServiceVoucherTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override VmServiceVoucher TranslateEntityToVm(ServiceWebPage entity)
        {
            throw new NotImplementedException("ServiceWebPage -> VmServiceVoucher is not implemented.");
        }

        public override ServiceWebPage TranslateVmToEntity(VmServiceVoucher vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.Id.IsAssigned();
            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, 
                    input => output => (input.Id == output.WebPageId) && (!input.OwnerReferenceId.IsAssigned() || output.ServiceVersionedId == vModel.OwnerReferenceId));

            return translation
                .AddNavigation(input => input, output => output.WebPage)
                .GetFinal();
        }
    }
}
