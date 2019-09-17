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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelWebPage, VmWebPage>), RegisterType.Transient)]
    internal class ServiceChannelWebPageTranslator : Translator<ServiceChannelWebPage, VmWebPage>
    {
        private ITypesCache typesCache;

        public ServiceChannelWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmWebPage TranslateEntityToVm(ServiceChannelWebPage entity)
        {
            return CreateEntityViewModelDefinition<VmWebPage>(entity)
                .AddSimple(input => input.TypeId, output => output.TypeId)
                .AddPartial(input => input.WebPage)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .AddSimple(i=> i.WebPageId, o => o.Id)
                .AddSimple(i => i.ServiceChannelVersionedId, o=>o.OwnerReferenceId)
                .GetFinal();
        }

        public override ServiceChannelWebPage TranslateVmToEntity(VmWebPage vModel)
        {
            Guid? id = vModel.Id;
            bool exists = id.IsAssigned();

            var translation = CreateViewModelEntityDefinition<ServiceChannelWebPage>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output => (id == output.WebPageId) &&
                (!input.OwnerReferenceId.IsAssigned() || output.ServiceChannelVersionedId == vModel.OwnerReferenceId));

            if (vModel.TypeId.IsAssigned())
            {
                translation.AddSimple(input => input.TypeId.Value, output => output.TypeId);
            }
            else
            {
                translation.AddSimple(input => typesCache.Get<WebPageType>(WebPageTypeEnum.HomePage.ToString()), output => output.TypeId);
            }

            return translation
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .AddNavigation(input => input, output => output.WebPage)
                .GetFinal();
        }

    }
}
