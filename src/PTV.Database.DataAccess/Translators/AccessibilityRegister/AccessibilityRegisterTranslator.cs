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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.AccessibilityRegister
{
    [RegisterService(typeof(ITranslator<Model.Models.AccessibilityRegister, VmAccessibilityRegister>), RegisterType.Transient)]
    internal class AccessibilityRegisterTranslator : Translator<Model.Models.AccessibilityRegister, VmAccessibilityRegister>
    {
        private readonly ILanguageCache languageCache;

        public AccessibilityRegisterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmAccessibilityRegister TranslateEntityToVm(Model.Models.AccessibilityRegister entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.ServiceChannelId, o => o.ServiceChannelId)
                .AddSimple(i => i.AddressId, o => o.AddressId)
                .AddNavigation(i => i.Address, o => o.Address)
                .AddNavigation(i => languageCache.GetByValue(i.AddressLanguageId), o => o.AddressLanguage)
                .AddSimple(i => i.ServiceChannelId, o => o.ServiceChannelId)
                .AddSimple(i => i.Modified, o => o.SetAt)
                .AddNavigation(i => i.ContactEmail, o => o.ContactEmail)
                .AddNavigation(i => i.ContactPhone, o => o.ContactPhone)
                .AddNavigation(i => i.ContactUrl, o => o.ContactUrl)
                .AddCollection(i => i.Entrances.OrderBy(g => g.OrderNumber), o => o.Entrances);
            return definition.GetFinal();
        }

        public override Model.Models.AccessibilityRegister TranslateVmToEntity(VmAccessibilityRegister vModel)
        {
            if (vModel == null) return null;
            var exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id)
                .AddSimple(i => i.AddressId, o => o.AddressId)
                .AddSimple(i => i.AddressLanguageId, o => o.AddressLanguageId)
                .AddSimple(i => true, o => o.IsValid)
                .AddNavigation(i => i.ContactEmail, o => o.ContactEmail)
                .AddNavigation(i => i.ContactPhone, o => o.ContactPhone)
                .AddNavigation(i => i.ContactUrl, o => o.ContactUrl);

            if (vModel.ServiceChannelId.HasValue)
            {
                definition.AddSimple(i => i.ServiceChannelId.Value, o => o.ServiceChannelId);
            }

            if (!vModel.AddressLanguage.IsNullOrEmpty())
            {
                definition.AddSimple(i => languageCache.Get(i.AddressLanguage), o => o.AddressLanguageId);
            }

            if (!vModel.Entrances.IsNullOrEmpty())
            {
                definition.AddCollectionWithRemove(i => i.Entrances, o => o.Entrances, x => true);
            }

            if (vModel.SetAt.HasValue)
            {
                definition.AddSimple(i => i.SetAt.Value, o => o.Modified);
            }

            return definition.GetFinal();
        }
    }
}
