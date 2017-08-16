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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

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
                .AddNavigation(i => i.PrefixNumber, o => o.PrefixNumber)
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddSimple(i => i.ServiceChargeTypeId, o => o.ChargeTypeId)
                .AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription)
                .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
                .AddSimple(i => i.LocalizationId, o => o.LanguageId)
                .AddSimple(i => (i.PrefixNumber == null && !string.IsNullOrEmpty(i.Number)), o => o.IsFinnishServiceNumber)
                .GetFinal();
        }

        public override Phone TranslateVmToEntity(VmPhone vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var translationDefinitions = CreateViewModelEntityDefinition<Phone>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, def => def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
                .AddNavigation(i => i.Number, o => o.Number)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => i.PrefixNumber?.Id, o => o.PrefixNumberId)
                .AddSimple(i => i.ChargeTypeId.IsAssigned() ? i.ChargeTypeId : typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()), o => o.ServiceChargeTypeId)
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

            if (vModel.LanguageId.IsAssigned())
            {
                translationDefinitions.AddSimple(i => i.LanguageId.Value, o => o.LocalizationId);
            }

            var entity = translationDefinitions.GetFinal();
            return entity;
        }
    }
}
