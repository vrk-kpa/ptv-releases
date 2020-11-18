/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiItem>), RegisterType.Transient)]
    internal class OpenApiServiceChannelItemTranslator : Translator<ServiceChannelVersioned, VmOpenApiItem>
    {
        public OpenApiServiceChannelItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiItem TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            if (entity == null) return null;

            var vm = CreateEntityViewModelDefinition(entity)
                // We have to use unique root id for the service channel!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddNavigation(i => i.ServiceChannelNames?.Where(n => n.LocalizationId == languageCache.Get(DomainConstants.DefaultLanguage)).FirstOrDefault(), o => o.Name)
                .GetFinal();

            // If Finnish translation was not found let's get the first one available.
            if (string.IsNullOrEmpty(vm.Name))
            {
                vm.Name = entity.ServiceChannelNames?.FirstOrDefault()?.Name;
            }

            return vm;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiItem vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelItemTranslator.");
        }
    }
}
