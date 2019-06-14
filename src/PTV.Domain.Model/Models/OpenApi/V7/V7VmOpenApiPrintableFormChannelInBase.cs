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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of printable form channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPrintableFormChannelInVersionBase" />
    public class V7VmOpenApiPrintableFormChannelInBase : VmOpenApiPrintableFormChannelInVersionBase
    {
        /// <summary>
        /// List of localized form receivers. One per language.
        /// </summary>
        [JsonProperty(Order = 11)]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public IList<VmOpenApiLanguageItem> FormReceiver { get; set; }

        /// <summary>
        /// Form delivery address.
        /// </summary>
        [JsonProperty(Order = 12)]
        public V7VmOpenApiAddressDeliveryIn DeliveryAddress { get; set; }

        /// <summary>
        /// List of localized service channel descriptions. Possible type values are: Description, ShortDescription.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "ShortDescription" })]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// Area type. Possible values are: WholeCountry, WholeCountryExceptAlandIslands or AreaType.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        [ListRequiredIf("AreaType", "AreaType")]
        [ListWithEnum(typeof(AreaTypeEnum), "Type")]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [ListWithEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> SupportPhones { get => base.SupportPhones; set => base.SupportPhones = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }
        
        /// <summary>
        /// Gets or sets the delivery addresses.
        /// </summary>
        [JsonIgnore]
        public override IList<V8VmOpenApiAddressDeliveryIn> DeliveryAddresses { get; set; }
        
        /// <summary>
        /// Set to true to delete all existing delivery addresses for the service channel. The DeliveryAddresses should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllDeliveryAddresses { get; set; }
        
        /// <summary>
        /// Set to true to delete all existing delivery address for the service channel. The DeliveryAddress should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 16)]
        public virtual bool DeleteDeliveryAddress { get; set; }

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonIgnore]
        public override IList<string> Languages { get; set; }

        /// <summary>
        /// Set to true to delete all existing form receivers for the service channel. The form receivers collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 20)]
        public virtual bool DeleteAllFormReceivers { get; set; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidFrom { get => base.ValidFrom; set => base.ValidFrom = value; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidTo { get => base.ValidTo; set => base.ValidTo = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 7;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = GetVersionBaseModel(VersionNumber());
            if (DeliveryAddress == null && FormReceiver == null) return vm;
            
            var v8address = DeliveryAddress == null
                ? new V8VmOpenApiAddressDeliveryIn
                    {
                        SubType = AddressTypeEnum.NoAddress.ToString(),
                        FormReceiver = FormReceiver
                    }
                : new V8VmOpenApiAddressDeliveryIn
                    {
                        Id = DeliveryAddress.Id,
                        SubType = DeliveryAddress.SubType,
                        StreetAddress = DeliveryAddress.StreetAddress,
                        PostOfficeBoxAddress = DeliveryAddress.PostOfficeBoxAddress,
                        DeliveryAddressInText = DeliveryAddress.DeliveryAddressInText,
                        FormReceiver = FormReceiver
                    };
            
            vm.DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn> {v8address};
            return vm;
        }
        #endregion
    }
}
