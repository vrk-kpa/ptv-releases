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
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, IPhoneNumberAndFax>), RegisterType.Transient)]
    internal class ServiceChannelPhoneFaxStepTranslator : Translator<ServiceChannelVersioned, IPhoneNumberAndFax>
    {
        private ITypesCache typesCache;
        public ServiceChannelPhoneFaxStepTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override IPhoneNumberAndFax TranslateEntityToVm(ServiceChannelVersioned entity)
        {
//            throw new NotSupportedException();
            return CreateEntityViewModelDefinition(entity)
                .AddLocalizable(input => input.Phones.Select(x => x.Phone).Where(x => x.Type.Code == PhoneNumberTypeEnum.Phone.ToString()), output => output.PhoneNumber)
                .AddLocalizable(input => input.Phones.Select(x => x.Phone).Where(x => x.Type.Code == PhoneNumberTypeEnum.Fax.ToString()), output => output.Fax)
                .GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(IPhoneNumberAndFax vModel)
        {
            var phones = new List<VmPhone>();
            if (vModel.Fax != null)
            {
                vModel.Fax.TypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());
                phones.Add(vModel.Fax);
            }
            if (vModel.PhoneNumber != null)
            {
                vModel.PhoneNumber.TypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
                phones.Add(vModel.PhoneNumber);
            }
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddCollection(input => phones , output => output.Phones)
                .GetFinal();
        }
    }
}
