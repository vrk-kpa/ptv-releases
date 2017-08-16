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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of channel relation
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmChannelRelation" />
    public class VmChannelRelation : IVmChannelRelation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public Guid Service { get; set; }
        /// <summary>
        /// Gets or sets the type id of the charge.
        /// </summary>
        /// <value>
        /// The type id of the charge.
        /// </value>
        public Guid? ChargeType { get; set; }
        /// <summary>
        /// Gets or sets the descriptions.
        /// </summary>
        /// <value>
        /// The descriptions.
        /// </value>
        public List<VmLocalizedDescription> Descriptions { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional informations.
        /// </summary>
        /// <value>
        /// The charge type additional informations.
        /// </value>
        public List<VmLocalizedDescription> ChargeTypeAdditionalInformations { get; set; }

        /// <summary>
        /// Gets or sets the texts.
        /// </summary>
        /// <value>
        /// The texts.
        /// </value>
        [JsonConverter(typeof(DictionaryToPropertyConverter<VmChannelRelationTexts>))]
        public IDictionary<string, VmChannelRelationTexts> Texts { get; set; }
        /// <summary>
        /// Gets or sets the connected channel.
        /// </summary>
        /// <value>
        /// The connected channel.
        /// </value>
        public VmConnectedChannel ConnectedChannel { get; set; }

        /// <summary>
        /// Gets or sets the digital authorizations.
        /// </summary>
        /// <value>
        /// The digital authorizations.
        /// </value>
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> DigitalAuthorizations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this relation is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this relation is new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this relation is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this relation is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool isLoaded { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmChannelRelation"/> class.
        /// </summary>
        public VmChannelRelation()
        {
            Texts = new Dictionary<string, VmChannelRelationTexts>();
            ConnectedChannel = new VmConnectedChannel();
            DigitalAuthorizations = new List<VmListItem>();
            Descriptions = new List<VmLocalizedDescription>();
            ChargeTypeAdditionalInformations = new List<VmLocalizedDescription>();

        }
    }

    /// <summary>
    /// View model of channel relation texts
    /// </summary>
    public class VmChannelRelationTexts
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional information.
        /// </summary>
        /// <value>
        /// The charge type additional information.
        /// </value>
        public string ChargeTypeAdditionalInformation { get; set; }
    }
}
