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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<Phone, VmPhone>), RegisterType.Transient)]
    internal class PhoneTranslator : Translator<Phone, VmPhone>
    {
        private readonly ITypesCache typesCache;

        public PhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmPhone TranslateEntityToVm(Phone entity)
        {
            return CreateEntityViewModelDefinition<VmPhone>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Number, o => o.Number)
                .AddNavigation(i => i.PrefixNumber, o => o.DialCode)
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddSimple(i => i.ChargeTypeId, o => o.ChargeType)
                .AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription)
                .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
                .AddSimple(i => i.LocalizationId, o => o.LanguageId)
                .AddSimple(i => i.PrefixNumber == null, o => o.IsLocalNumber)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .AddSimpleList(i => i.ExtraTypes.Select(et => et.ExtraTypeId), o => o.ExtraTypes)
                .GetFinal();
        }

        public override Phone TranslateVmToEntity(VmPhone vModel)
        {
            bool exists = vModel.Id.IsAssigned();
            bool isFax = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()) == vModel.TypeId;
            if (vModel.LanguageId.IsAssigned())
            {
                SetLanguage(vModel.LanguageId.Value);
            }

            var dialCode = vModel.IsLocalNumber ? null : vModel.DialCode?.Id;
            if (isFax && vModel.DialCode != null)
            {
                dialCode = vModel.DialCode?.Id;
            }

            var translationDefinitions = CreateViewModelEntityDefinition<Phone>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, def => def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
                .AddNavigation(i => i.Number, o => o.Number)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                //.AddNavigation(i => i.DialCode, o => o.PrefixNumber)
                .AddSimple(i => dialCode, o => o.PrefixNumberId)
                // .AddNavigation(i => i.IsLocalNumber ? null : i.DialCode, o => o.PrefixNumber)
                .AddSimple(i => i.ChargeType.IsAssigned() ? i.ChargeType : typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()), o => o.ChargeTypeId)
                .AddRequestLanguage(i => i)
                .AddSimple(i => (vModel.TypeId.IsAssigned()? i.TypeId : typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())), o => o.TypeId);

            if (!string.IsNullOrEmpty(vModel.AdditionalInformation) || vModel.Id.IsAssigned())
            {
                translationDefinitions.AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation);
            }

            if (!string.IsNullOrEmpty(vModel.ChargeDescription) || vModel.Id.IsAssigned())
            {
                translationDefinitions.AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription);
            }

//            if (vModel.LanguageId.IsAssigned())
//            {
//                translationDefinitions.AddSimple(i => i.LanguageId.Value, o => o.LocalizationId);
//            }

            var entity = translationDefinitions.GetFinal();

            if (vModel.ExtraTypes != null)
            {
                translationDefinitions.AddCollectionWithRemove(i => i.ExtraTypes.Select(et =>
                    new VmExtraType { Id = et, OwnerReferenceId = entity.Id}), o => o.ExtraTypes, x => true );
            }

            return entity;
        }
    }
}
