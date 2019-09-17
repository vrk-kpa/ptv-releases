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
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
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

    [Flags]
    public enum SearchEntityTypeEnum : ulong
    {
        /// <summary>
        /// The service
        /// </summary>
        [JsonProperty("service")]
        Service = 0b1,
        /// <summary>
        /// The Service type Service
        /// </summary>
        [JsonProperty("serviceService")]
        ServiceService = 0b11,
        /// <summary>
        /// The Service type ProfessionalQualification
        /// </summary>
        [JsonProperty("serviceProfessional")]
        ServiceProfessional = 0b101,
        /// <summary>
        /// The Service type PermitOrOtherObligation
        /// </summary>
        [JsonProperty("servicePermit")]
        ServicePermit = 0b1001,
        
        /// <summary>
        /// The Service type PermitOrOtherObligation
        /// this enum should not be used for search criteria, it is fallback solution due to duplicity of names
        /// </summary>
        [JsonProperty("permissionAndObligation")]
        PermissionAndObligation = 0b111111111111,
        /// <summary>
        /// The Service type ProfessionalQualifications
        /// this enum should not be used for search criteria, it is fallback solution due to duplicity of names
        /// </summary>
        [JsonProperty("professionalQualifications")]
        ProfessionalQualifications = 0b111111111110,
        /// <summary>
        /// The channel
        /// </summary>        
        [JsonProperty("channel")]
        Channel = 0b10000,
      
        /// <summary>
        /// The ServiceLocation
        /// </summary>
        [JsonProperty("serviceLocation")]
        ServiceLocation = 0b110000,
        /// <summary>
        /// The Phone
        /// </summary>
        [JsonProperty("phone")]
        Phone = 0b1010000,
        /// <summary>
        /// The PrintableForm
        /// </summary>
        [JsonProperty("printableForm")]
        PrintableForm = 0b10010000,
        /// <summary>
        /// WebPage
        /// </summary>
        [JsonProperty("webPage")]
        WebPage = 0b100010000,
        /// <summary>
        /// The EChannel
        /// </summary>
        [JsonProperty("eChannel")]
        EChannel = 0b1000010000,
                  
        /// <summary>
        /// The organization
        /// </summary>
        [JsonProperty("organization")]
        Organization = 0b10000000000,
        /// <summary>
        /// The general description
        /// </summary>
        [JsonProperty("generalDescription")]
        GeneralDescription = 0b100000000000,
        /// <summary>
        /// The service collection
        /// </summary>
        [JsonProperty("serviceCollection")]
        ServiceCollection = 0b1000000000000,
        
        /// <summary>
        /// All values are set
        /// </summary>
        [JsonProperty("all")]
        All = ulong.MaxValue
    }
}