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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.DataAccess.Translators.Channels
{
//    [RegisterService(typeof(ITranslator<ServiceChannelSupport, VmPhoneData>), RegisterType.Transient)]
//    internal class ServiceChannelSupportTranslator : Translator<ServiceChannelSupport, VmPhoneData>
//    {
//        private ITypesCache typesCahce;
//
//        public ServiceChannelSupportTranslator(IResolveManager resolveManager, ITypesCache typesCahce, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
//        {
//            this.typesCahce = typesCahce;
//        }
//
//        public override VmPhoneData TranslateEntityToVm(ServiceChannelSupport entity)
//        {
//            var result = CreateEntityViewModelDefinition<VmPhoneData>(entity)
//                .AddNavigation(input => input.Phone, output => output.PhoneNumber)
//                .AddNavigation(input => input.Email, output => output.Email)
//                .AddSimpleList(input => input.ServiceChargeTypes.Select(x => x.ServiceChargeTypeId), output => output.ChargeTypes)
//                .AddNavigation(input => input.PhoneChargeDescription, output => output.ChargeDescription)
//                .GetFinal();
//            return result;
//        }
//
//        public override ServiceChannelSupport TranslateVmToEntity(VmPhoneData vModel)
//        {
//            var translator = CreateViewModelEntityDefinition<ServiceChannelSupport>(vModel)
//                .DisableAutoTranslation()
//                .UseDataContextUpdate(i => ((VmEntityBase) i).Id.IsAssigned(), i => o=> ((VmEntityBase) i).Id == o.ServiceChannelId, def => def.UseDataContextCreate(i => !((VmEntityBase) i).Id.IsAssigned(), o => o.Id, i => Guid.NewGuid()))
//                .UseDataContextCreate(i => !((VmEntityBase) i).Id.IsAssigned(),o => o.Id, i => Guid.NewGuid())
//                .AddNavigation(input => input.PhoneNumber, output => output.Phone)
//                .AddRequestLanguage(output => output)
//                .AddNavigation(input => input.Email, output => output.Email)
//                .AddCollection(input => input.ChargeTypes.Select(x => new VmSelectableItem() { Id = x}), output => output.ServiceChargeTypes);
//
//            if (!string.IsNullOrEmpty(vModel.ChargeDescription))
//            {
//                translator
//                    .AddNavigation(input => input.ChargeDescription, output => output.PhoneChargeDescription);
//            }
//            else
//            {
//                translator
//                    .AddNavigation(input => string.Empty, output => output.PhoneChargeDescription);
//            }
//
//            return translator.GetFinal();
//        }
//    }
}
