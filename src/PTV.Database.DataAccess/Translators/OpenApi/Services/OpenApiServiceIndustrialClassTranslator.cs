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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceIndustrialClass, VmOpenApiStringItem>), RegisterType.Transient)]
    internal class OpenApiServiceIndustrialClassTranslator : Translator<ServiceIndustrialClass, VmOpenApiStringItem>
    {
        public OpenApiServiceIndustrialClassTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override VmOpenApiStringItem TranslateEntityToVm(ServiceIndustrialClass entity)
        {
            throw new NotImplementedException();
        }
        public override ServiceIndustrialClass TranslateVmToEntity(VmOpenApiStringItem vModel)
        {
            return CreateViewModelEntityDefinition<ServiceIndustrialClass>(vModel)
                .UseDataContextUpdate(
                  i => i.OwnerReferenceId.IsAssigned(),
                  i => o => i.OwnerReferenceId.Value == o.ServiceVersionedId && i.Value == o.IndustrialClass.Uri,
                  def => def.UseDataContextCreate(i => true)
                )
                .AddNavigation(i => i.Value, o => o.IndustrialClass)
                .GetFinal();
        }
    }
}