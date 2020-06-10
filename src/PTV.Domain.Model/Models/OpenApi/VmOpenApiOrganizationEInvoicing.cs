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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.ComponentModel.DataAnnotations;
using PTV.Framework.Attributes;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization email
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationEInvoicing" />
    public class VmOpenApiOrganizationEInvoicing : VmOpenApiBase, IVmOpenApiOrganizationEInvoicing
    {
        /// <summary>
        /// Operator code.
        /// </summary>
        [JsonProperty(Order = 1)]
        [MaxLength(110)]
        public string OperatorCode { get; set; }

        /// <summary>
        /// Electronic invoicing address
        /// </summary>
        [MaxLength(110)]
        [JsonProperty(Order = 2)]
        public string ElectronicInvoicingAddress { get; set; }

        /// <summary>
        /// Localized list of additional information for electronic invoicing address.
        /// </summary>
        [ListPropertyMaxLength(150, "Value")]
        [ListValueNotEmpty("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [JsonProperty(Order = 3)]
        public IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
