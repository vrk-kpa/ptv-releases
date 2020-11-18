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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of entity item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiEntityItem" />
    public class VmOpenApiEntityItem : IVmOpenApiEntityItem
    {
        /// <summary>
        /// Id of the item.
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Type of the item. For version 10 (and up) the type for service can be Service, PermitOrObligation or ProfessionalQualification
        /// and for service channel EChannel, WebPage, PrintableForm, Phone or ServiceLocation.
        /// In older versions type can only be either Service or ServiceChannel.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonIgnore]
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// General description id for a service. Used only internally.
        /// </summary>
        [JsonIgnore]
        public virtual Guid? GeneralDescriptionId { get; set; }
    }
}
