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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannel, VmLocationChannelStep1>), RegisterType.Transient)]
    internal class LocationChannelStep1Translator : Translator<ServiceLocationChannel, VmLocationChannelStep1>
    {
        public LocationChannelStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLocationChannelStep1 TranslateEntityToVm(ServiceLocationChannel entity)
        {
            return CreateEntityViewModelDefinition<VmLocationChannelStep1>(entity)                                
                .AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Visiting.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses)
                .AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Postal.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses)
                .GetFinal();
        }

        public override ServiceLocationChannel TranslateVmToEntity(VmLocationChannelStep1 vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceLocationChannel>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelVersionedId)                
                .Propagation((i,o) => i.LocationChannelId = o.Id);

            var order = 1;
            vModel.VisitingAddresses.ForEach(item => item.OrderNumber = order++);
            order = 1;
            vModel.PostalAddresses.ForEach(item => item.OrderNumber = order++);
            transaltionDefinition.AddCollection(i => i.VisitingAddresses.Concat(i.PostalAddresses), o => o.Addresses);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }
}
