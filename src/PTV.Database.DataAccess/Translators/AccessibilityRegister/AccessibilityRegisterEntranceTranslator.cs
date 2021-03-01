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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.AccessibilityRegister
{
    [RegisterService(typeof(ITranslator<AccessibilityRegisterEntrance, VmAccessibilityRegisterEntrance>), RegisterType.Transient)]
    internal class AccessibilityRegisterEntranceTranslator : Translator<AccessibilityRegisterEntrance, VmAccessibilityRegisterEntrance>
    {
        public AccessibilityRegisterEntranceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAccessibilityRegisterEntrance TranslateEntityToVm(AccessibilityRegisterEntrance entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.AccessibilityRegisterId, o => o.AccessibilityRegisterId)
                .AddSimple(i => i.OrderNumber.GetValueOrDefault(), o => o.OrderNumber)
                .AddSimple(i => i.IsMain, o => o.IsMain)
                .AddSimple(i => i.EntranceId, o => o.EntranceId)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Address, o => o.Address)
                .AddCollection(i => i.SentenceGroups.OrderBy(s => s.OrderNumber), o => o.Groups)
                .AddDictionary(i => i.Names, o => o.Names, k => languageCache.GetByValue(k.LocalizationId));

            if (entity.AddressId.IsAssigned())
            {
                definition.AddSimple(i => i.AddressId.Value, o => o.AddressId);
            }

            return definition.GetFinal();
        }

        public override AccessibilityRegisterEntrance TranslateVmToEntity(VmAccessibilityRegisterEntrance vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid())
                .AddSimple(i => i.AccessibilityRegisterId, o => o.AccessibilityRegisterId)
                .AddSimple(i => i.AddressId, o => o.AddressId)
                .AddNavigation(i => i.Address, o => o.Address)
                .AddSimple(i => i.EntranceId, o => o.EntranceId)
                .AddSimple(i => i.IsMain, o => o.IsMain)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddCollectionWithRemove(i => i.Names?.Select(
                        pair => new VmAccessibilityRegisterLanguageItem
                        {
                            OwnerReferenceId = Guid.Empty,
                            Value = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key)
                        }),
                    o => o.Names, x => true)
                .AddCollectionWithRemove(i => i.Groups, o => o.SentenceGroups, x => true);

            return definition.GetFinal();
        }
    }
}
