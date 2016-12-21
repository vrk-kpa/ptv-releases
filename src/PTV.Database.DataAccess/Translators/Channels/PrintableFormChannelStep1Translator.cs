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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannel, VmPrintableFormChannelStep1>), RegisterType.Transient)]
    internal class PrintableFormChannelStep1Translator : Translator<PrintableFormChannel, VmPrintableFormChannelStep1>
    {
        private ILanguageCache languageCache;
        public PrintableFormChannelStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmPrintableFormChannelStep1 TranslateEntityToVm(PrintableFormChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FormIdentifier, o => o.FormIdentifier)
                .AddNavigation(i => i.FormReceiver, o => o.FormReceiver)
                .AddNavigation(i => i.DeliveryAddress ?? new Address(), o => o.DeliveryAddress)
                .AddCollection(input => languageCache.FilterCollection(input.ChannelUrls, RequestLanguageCode), output => output.WebPages).GetFinal();
        }

        public override PrintableFormChannel TranslateVmToEntity(VmPrintableFormChannelStep1 vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<PrintableFormChannel>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => o.ServiceChannelId == i.Id);

            SetStep1Translation(transaltionDefinition, vModel);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

        private void SetStep1Translation(ITranslationDefinitions<VmPrintableFormChannelStep1, PrintableFormChannel> definition, VmPrintableFormChannelStep1 model)
        {
            model.UrlAttachments?.ForEach(i => i.OwnerReferenceId = model.Id);
            model.DeliveryAddress?.SafeCall(i => {
                i.OwnerReferenceId = model.Id;
                i.PostalCode = !string.IsNullOrEmpty(i.AdditionalInformation) && i.PostalCode == null ? new VmPostalCode() { Code = "Undefined" } : i.PostalCode;
            });

            definition
                .AddNavigation(i => i.FormIdentifier, o => o.FormIdentifier)
                .AddNavigation(i => i.FormReceiver, o => o.FormReceiver)
                .AddNavigation(i => (i.DeliveryAddress?.PostalCode != null) ? i.DeliveryAddress : null, o => o.DeliveryAddress)
                .AddSimple(i => (i.DeliveryAddress?.PostalCode != null) ? i.DeliveryAddress.Id : null, o => o.DeliveryAddressId)
                .AddCollection(i => i.WebPages, o => o.ChannelUrls);
        }
    }
}

