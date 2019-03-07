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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of service search form
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    public interface IVmEntitySearch : IVmEntityBase, IVmSortBase, IVmMultiLocalized, IVmOrganizationInfo, IVmSearchEntities
    {
        /// <summary>
        /// The search content types 
        /// </summary>
        List<SearchEntityTypeEnum> ContentTypes { get; set; }
        
        
        /// <summary>
        /// Merged list of ContentTypes
        /// </summary>
        [JsonIgnore]
        SearchEntityTypeEnum ContentTypeEnum { get; set; }
            
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the ontology word identifiers.
        /// </summary>
        /// <value>
        /// The ontology words.
        /// </value>
        List<Guid> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the service classes identifiers.
        /// </summary>
        /// <value>
        /// The service class identifiers.
        /// </value>
        List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the service type identifiers.
        /// </summary>
        /// <value>
        /// The general description service type identifiers.
        /// </value>
        List<Guid> ServiceTypes { get; set; }
        /// <summary>
        /// Gets or sets the general description type identifiers.
        /// </summary>
        /// <value>
        /// The general description type identifiers.
        /// </value>
        List<Guid> GeneralDescriptionTypes { get; set; }        
        /// <summary>
        /// Gets or sets the target group identifiers.
        /// </summary>
        /// <value>
        /// The target group identifiers.
        /// </value>
        List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the industrial class identifiers.
        /// </summary>
        /// <value>
        /// The industrial class identifiers.
        /// </value>
        List<Guid> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the life event identifiers.
        /// </summary>
        /// <value>
        /// The life event identifiers.
        /// </value>
        List<Guid> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// define when entity expire in weeks
        /// </summary>
        int ExpirationInWeeks { get; set; }
        /// <summary>
        /// Gets or sets the language ids.
        /// </summary>
        /// <value>
        /// The language ids.
        /// </value>
        [JsonIgnore]
        List<Guid> LanguageIds { get; set; }
        /// <summary>
        /// The search channels content types 
        /// </summary>
        [JsonIgnore]
        List<Guid> ChannelTypeIds { get; set; }   
        /// <summary>
        /// All sub organization ids 
        /// </summary>
        [JsonIgnore]
        List<Guid> SubOrganizationIds { get; set; }
        /// <summary>
        /// User name 
        /// </summary>
        [JsonIgnore]
        string UserName { get; set; }
        /// <summary>
        /// Show only my content 
        /// </summary>
        bool MyContent { get; set; }
        /// <summary>
        /// Service service type  
        /// </summary>
        [JsonIgnore]
        List<Guid> ServiceServiceType { get; set; }
        /// <summary>
        /// Search type
        /// </summary>
        SearchTypeEnum? SearchType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Guid? AddressStreetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Guid? AddressStreetNumberId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string StreetNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Guid? PhoneDialCode { get; set; }

        /// <summary>
        /// ID of Postal code that user searches for
        /// </summary>
        Guid? PostalCodeId { get; set; }
        
        /// <summary>
        /// Checks whether the phone number is local (Finnish, without a dial code).
        /// </summary>
        bool? IsLocalNumber { get; set; }
        
        /// <summary>
        /// Searched email.
        /// </summary>
        string Email { get; set; }
    }
}
