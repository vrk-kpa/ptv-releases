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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, IPhoneNumber>), RegisterType.Transient)]
    internal class ServiceChannelPhoneNumberStepTranslator : Translator<ServiceChannelVersioned, IPhoneNumber>
    {
        private ITypesCache typesCache;
        public ServiceChannelPhoneNumberStepTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override IPhoneNumber TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddLocalizable(input => input.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Phone.ToString())), output => output.PhoneNumber)
                .GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(IPhoneNumber vModel)
        {
            if (vModel?.PhoneNumber == null)
            {
                return null;
            }
            vModel.PhoneNumber.TypeId = vModel.PhoneNumber.TypeId.IsAssigned() ? vModel.PhoneNumber.TypeId : typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            vModel.PhoneNumber.ChargeType = vModel.PhoneNumber.ChargeType.IsAssigned() ? vModel.PhoneNumber.ChargeType : typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString());
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddCollection(input => new List<VmPhone> { vModel.PhoneNumber }, output => output.Phones)
                .GetFinal();
        }
    }
}
