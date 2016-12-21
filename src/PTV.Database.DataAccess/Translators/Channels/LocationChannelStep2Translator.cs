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
//    [RegisterService(typeof(ITranslator<ServiceLocationChannel, VmLocationChannelStep2>), RegisterType.Transient)]
//    internal class LocationChannelStep2Translator : Translator<ServiceLocationChannel, VmLocationChannelStep2>
//    {
//        private ITypesCache typesCahce;
//
//        public LocationChannelStep2Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCahce) : base(resolveManager, translationPrimitives)
//        {
//            this.typesCahce = typesCahce;
//        }
//
//        public override VmLocationChannelStep2 TranslateEntityToVm(ServiceLocationChannel entity)
//        {
//            return CreateEntityViewModelDefinition<VmLocationChannelStep2>(entity)
//                // TODO DAnger
//                //.AddCollection(input => input.Email, output => output.Email)
//                //.AddNavigation(input => input.Phone, output => output.PhoneNumber)
//                //.AddNavigation(input => input.Fax, output => output.Fax)
//                //.AddSimpleList(input => input.ServiceChargeTypes.Select(x => x.ServiceChargeTypeId), output => output.ChargeTypes)
//                //.AddNavigation(input => input.PhoneChargeDescriptions.Select(x => x.Description).FirstOrDefault(), output => output.ChargeDescription)
//                .GetFinal();
//        }
//
//        public override ServiceLocationChannel TranslateVmToEntity(VmLocationChannelStep2 vModel)
//        {
//            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceLocationChannel>(vModel)
//                .UseDataContextCreate(i => !((VmEntityBase)i).Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
//                .UseDataContextUpdate(i => ((VmEntityBase)i).Id.IsAssigned(), i => o => ((VmEntityBase)i).Id == o.ServiceChannelId);
//                 // TODO DAnger
//                 //.AddNavigation(input => input.Email, output => output.Email)
//                 //.AddNavigation(input => input.PhoneNumber, output => output.Phone)
//                 //.AddNavigation(input => input.Fax, output => output.Fax)
//                 //.AddCollection(input => input.ChargeTypes.Select(x => new VmSelectableItem() { Id = x}), output => output.ServiceChargeTypes)
//                 //;
//            //if (vModel.ChargeTypes.Any(x => typesCahce.Compare<ServiceChargeType>(x, ServiceChargeTypeEnum.Other.ToString()))
 //               && !string.IsNullOrEmpty(vModel.ChargeDescription))
//            //{
//            //    transaltionDefinition
//            //        .AddCollection(
//            //            input => new List<VmStringText> { new VmStringText {Text = input.ChargeDescription, Id = ((VmEntityBase) vModel).Id ?? Guid.Empty} },
//            //            output => output.PhoneChargeDescriptions);
//            //}
//
//            var entity = transaltionDefinition.GetFinal();
//            return entity;
//        }
//    }
}
