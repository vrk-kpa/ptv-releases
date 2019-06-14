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

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelEmail, VmEmailData>), RegisterType.Transient)]
    internal class ServiceServiceChannelEmailTranslator : Translator<ServiceServiceChannelEmail, VmEmailData>
    {
        private readonly ILanguageCache languageCache;
        public ServiceServiceChannelEmailTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmEmailData TranslateEntityToVm(ServiceServiceChannelEmail entity)
        {
            throw new NotSupportedException();
        }

        public override ServiceServiceChannelEmail TranslateVmToEntity(VmEmailData vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();
           
            Guid languageId = vModel.LanguageId.IsAssigned()
                ? vModel.LanguageId.Value
                : RequestLanguageId;

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById)
                .UseDataContextUpdate(
                    input => existsById,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.EmailId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2) &&
                                       (languageId == output.Email.LocalizationId),
                    create => create.UseDataContextCreate(c => true)
                );

            var result = translation
                .AddNavigation(input => input, output => output.Email)
                .GetFinal();
            return result;
        }
    }
}
