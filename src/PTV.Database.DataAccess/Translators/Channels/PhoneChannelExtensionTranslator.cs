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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
//    [RegisterService(typeof(ITranslator<PhoneChannel, VmPhoneData>), RegisterType.Transient)]
//    internal class PhoneChannelExtensionTranslator : Translator<PhoneChannel, VmPhoneData>
//    {
//        private ILanguageCache languageCache;
//        private ITypesCache typesCahce;
//        public PhoneChannelExtensionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache, ITypesCache typesCahce) : base(resolveManager, translationPrimitives)
//        {
//            this.languageCache = languageCache;
//            this.typesCahce = typesCahce;
//        }
//
//        public override VmPhoneData TranslateEntityToVm(PhoneChannel entity)
//        {
//            return CreateEntityViewModelDefinition(entity)
//                .AddLocalizable(i => i.LocalizedPhones, o => o.PhoneNumber)
//                .AddSimple(i => i.PhoneTypeId, o => o.TypeId)
//                .AddNavigation(i => i.PhoneChargeDescription?.Description ?? String.Empty, o => o.ChargeDescription)
//                .AddSimpleList(i => i.ServiceChargeTypes.Select(x => x.ServiceChargeTypeId), o => o.ChargeTypes).GetFinal();
//        }
//
//        public override PhoneChannel TranslateVmToEntity(VmPhoneData vModel)
//        {
//            var charTypes= vModel.ChargeTypes.Select(x => new VmSelectableItem() {Id = x, OwnerReferenceId = vModel.OwnerReferenceId});
//            var translator = CreateViewModelEntityDefinition<PhoneChannel>(vModel)
//                .DisableAutoTranslation()
//                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
//                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.Id, def => def.UseDataContextCreate(i => true))
//                .AddCollection(i => charTypes , o => o.ServiceChargeTypes)
//                .AddLocalizable(i => i, o => o.LocalizedPhones);
//
//            translator = vModel.TypeId.HasValue
//                ? translator.AddSimple(i => i.TypeId.Value, o => o.PhoneTypeId)
//                : translator.AddNavigation(i => PhoneNumberTypeEnum.Phone.ToString(), o => o.PhoneType);
//
//            if ( !string.IsNullOrEmpty(vModel.ChargeDescription) || vModel.OwnerReferenceId.IsAssigned())
//            {
//                translator
//                    .AddNavigation(input => new VmStringText { Text = input.ChargeDescription, Id = input.OwnerReferenceId ?? Guid.Empty}, output => output.PhoneChargeDescription);
//            }
//            var entity = translator.GetFinal();
//            return entity;
//        }
//    }
}
