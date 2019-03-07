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
using PTV.Database.DataAccess.Interfaces.Caches;
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
    [RegisterService(typeof(ITranslator<ServiceChannelPhone, VmPhone>), RegisterType.Transient)]
    internal class ServiceChannelPhoneTranslator : Translator<ServiceChannelPhone, VmPhone>
    {
        private ILanguageCache languageCache;

        public ServiceChannelPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmPhone TranslateEntityToVm(ServiceChannelPhone entity)
        {
            throw new NotSupportedException();
//            return CreateEntityViewModelDefinition<VmPhone>(entity)
//                .AddPartial(input => input.Phone)
//                .AddNavigation(i=> i.PhoneId.ToString(), o => o.Id)
//                .AddSimple(i => i.ServiceChannelId, o=>o.OwnerReferenceId)
//                .GetFinal();
        }

        public override ServiceChannelPhone TranslateVmToEntity(VmPhone vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            bool exists = vModel.Id.IsAssigned();
            bool updateWithLanguage = vModel.OwnerReferenceId.IsAssigned() && vModel.LanguageId.IsAssigned();
            Guid? languageId = vModel.LanguageId ?? RequestLanguageId;
            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists && !updateWithLanguage)
                .UseDataContextUpdate(input => exists || updateWithLanguage,
                    input => output =>
                        (!input.Id.IsAssigned() || input.Id == output.PhoneId) &&
                        (languageId == output.Phone.LocalizationId) &&
                        (!input.OwnerReferenceId.IsAssigned() || output.ServiceChannelVersionedId == vModel.OwnerReferenceId),
                    e => e.UseDataContextCreate(x => true));

            if (!exists && updateWithLanguage)
            {
                var serviceChannelPhone = translation.GetFinal();
                if (serviceChannelPhone.Created > DateTime.MinValue)
                {
                    vModel.Id = serviceChannelPhone.PhoneId;
                }
            }

            return translation
                .AddNavigation(input => input, output => output.Phone)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }

    }
}
