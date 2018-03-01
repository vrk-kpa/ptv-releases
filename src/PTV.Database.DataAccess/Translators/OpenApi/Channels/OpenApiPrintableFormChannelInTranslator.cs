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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannel, VmOpenApiPrintableFormChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiPrintableFormChannelInTranslator : Translator<PrintableFormChannel, VmOpenApiPrintableFormChannelInVersionBase>
    {
        public OpenApiPrintableFormChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override VmOpenApiPrintableFormChannelInVersionBase TranslateEntityToVm(PrintableFormChannel entity)
        {
            throw new NotImplementedException();
        }
        public override PrintableFormChannel TranslateVmToEntity(VmOpenApiPrintableFormChannelInVersionBase vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition<PrintableFormChannel>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.ServiceChannelVersionedId, def => def.UseDataContextCreate(i => true));

            if (vModel.ChannelId.IsAssigned())
            {
                vModel.FormIdentifier.ForEach(i => i.OwnerReferenceId = vModel.ChannelId);
                vModel.FormReceiver.ForEach(i => i.OwnerReferenceId = vModel.ChannelId);
                vModel.ChannelUrls.ForEach(i => i.OwnerReferenceId = vModel.ChannelId);
            }

            if (vModel.DeleteAllFormIdentifiers || vModel.FormIdentifier?.Count > 0)
            {
                definition.AddCollection(i => i.FormIdentifier == null ? new List<VmOpenApiLanguageItem>() : i.FormIdentifier, o => o.FormIdentifiers, false);
            }

            if (vModel.DeleteAllFormReceivers || vModel.FormReceiver?.Count > 0)
            {
                definition.AddCollection(i => i.FormReceiver == null ? new List<VmOpenApiLanguageItem>() : i.FormReceiver, o => o.FormReceivers, false);
            }

            if (vModel.DeleteDeliveryAddress || vModel.DeliveryAddress != null)
            {
                var address = (vModel.DeleteDeliveryAddress && vModel.DeliveryAddress == null) ? null : new V7VmOpenApiAddressWithForeignIn()
                {
                    Id = vModel.DeliveryAddress.Id,
                    SubType = vModel.DeliveryAddress.SubType,
                    PostOfficeBoxAddress = vModel.DeliveryAddress.PostOfficeBoxAddress,
                    StreetAddress = vModel.DeliveryAddress.StreetAddress == null ? null : new VmOpenApiAddressStreetWithCoordinatesIn()
                    {
                        Street = vModel.DeliveryAddress.StreetAddress.Street,
                        StreetNumber = vModel.DeliveryAddress.StreetAddress.StreetNumber,
                        PostalCode = vModel.DeliveryAddress.StreetAddress.PostalCode,
                        Municipality = vModel.DeliveryAddress.StreetAddress.Municipality,
                        AdditionalInformation = vModel.DeliveryAddress.StreetAddress.AdditionalInformation
                    }
                };
                if (address != null && vModel.DeliveryAddress != null && vModel.DeliveryAddress.SubType == AddressTypeEnum.NoAddress.ToString())
                {
                    address.PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn()
                    {                        
                        AdditionalInformation = vModel.DeliveryAddress.DeliveryAddressInText
                    };
                }
                definition.AddNavigation(i => address, o => o.DeliveryAddress);
            }

            if (vModel.DeleteAllChannelUrls || vModel.ChannelUrls?.Count > 0)
            {
                definition.AddCollection(i => vModel.ChannelUrls == null ? new List<VmOpenApiLocalizedListItem>() : i.ChannelUrls, o => o.ChannelUrls, false);
            }

            return definition.GetFinal();
        }
    }
}
