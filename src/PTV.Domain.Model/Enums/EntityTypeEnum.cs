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

namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Entity Types
    /// </summary>
    public enum EntityTypeEnum
    {
        /// <summary>
        /// The service
        /// </summary>
        Service,
        /// <summary>
        /// The channel
        /// </summary>
        Channel,
        /// <summary>
        /// The organization
        /// </summary>
        Organization,
        /// <summary>
        /// The general description
        /// </summary>
        GeneralDescription,
        /// <summary>
        /// The service collection
        /// </summary>
        ServiceCollection
    }
    
    /// <summary>
    /// Search Entity Types
    /// </summary>
    public enum SearchEntityTypeEnum
    {
        /// <summary>
        /// The service
        /// </summary>
        [JsonProperty("service")]
        Service,
        /// <summary>
        /// The Service type Service
        /// </summary>
        [JsonProperty("serviceService")]
        ServiceService,
        /// <summary>
        /// The Service type ProfessionalQualification
        /// </summary>
        [JsonProperty("serviceProfessional")]
        ServiceProfessional,
        /// <summary>
        /// The Service type PermitOrOtherObligation
        /// </summary>
        [JsonProperty("servicePermit")]
        ServicePermit,
        /// <summary>
        /// The channel
        /// </summary>        
        [JsonProperty("channel")]
        Channel,
        /// <summary>
        /// The organization
        /// </summary>
        [JsonProperty("organization")]
        Organization,
        /// <summary>
        /// The general description
        /// </summary>
        [JsonProperty("generalDescription")]
        GeneralDescription,
        /// <summary>
        /// The service collection
        /// </summary>
        [JsonProperty("serviceCollection")]
        ServiceCollection,
        /// <summary>
        /// The ServiceLocation
        /// </summary>
        [JsonProperty("serviceLocation")]
        ServiceLocation,
        /// <summary>
        /// The Phone
        /// </summary>
        [JsonProperty("phone")]
        Phone,
        /// <summary>
        /// The PrintableForm
        /// </summary>
        [JsonProperty("printableForm")]
        PrintableForm,
        /// <summary>
        /// WebPage
        /// </summary>
        [JsonProperty("webPage")]
        WebPage,
        /// <summary>
        /// The EChannel
        /// </summary>
        [JsonProperty("eChannel")]
        EChannel
    }
}