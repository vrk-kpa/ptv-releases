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
using System;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelEmail, VmEmailData>), RegisterType.Transient)]
    internal class ServiceChannelEmailTranslator : Translator<ServiceChannelEmail, VmEmailData>
    {
        public ServiceChannelEmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmEmailData TranslateEntityToVm(ServiceChannelEmail entity)
        {
            throw new NotSupportedException();
        }

        public override ServiceChannelEmail TranslateVmToEntity(VmEmailData vModel)
        {
            if (vModel == null)
            {
                return null;
            }
            bool existsById = vModel.Id.IsAssigned();
            var existsByOwnerReference = !vModel.Id.IsAssigned() && vModel.OwnerReferenceId.IsAssigned();
            Guid languageId = vModel.LanguageId.IsAssigned()
                ? vModel.LanguageId.Value
                : RequestLanguageId;

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById && !existsByOwnerReference)
                .UseDataContextUpdate(input => existsById || existsByOwnerReference, input => output =>
                    (!input.Id.IsAssigned() || input.Id == output.EmailId) &&
                    (!input.OwnerReferenceId.IsAssigned() || output.ServiceChannelVersionedId == input.OwnerReferenceId) &&
                    (languageId == output.Email.LocalizationId),
                    create => create.UseDataContextCreate(c => true)
            );

            if (existsByOwnerReference)
            {
                var serviceChannelEmail = translation.GetFinal();
                if (serviceChannelEmail.Created > DateTime.MinValue)
                {
                    vModel.Id = serviceChannelEmail.EmailId;
                }
            }

            return translation
                .AddNavigation(input => input, output => output.Email)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();
        }

    }
}
