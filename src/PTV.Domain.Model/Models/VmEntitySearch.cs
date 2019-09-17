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
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service search form
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmOrganizationEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntitySearch" />
    public class VmEntitySearch : VmOrganizationEntityBase, IVmEntitySearch
    {
        /// <summary>
        /// Construcotr of searvice searfch view model.
        /// </summary>
        public VmEntitySearch()
        {
            SortData = new List<VmSortParam>();
            OntologyTerms = new List<Guid>();
            EntityIds = new List<Guid>();
            EntityVersionIds = new List<Guid>();
            ServiceClasses = new List<Guid>();
            ServiceTypes = new List<Guid>();
            GeneralDescriptionTypes = new List<Guid>();
            SelectedPublishingStatuses = new List<Guid>();
            Languages = new List<string>();
            LanguageIds = new List<Guid>();
            IndustrialClasses = new List<Guid>();
            LifeEvents = new List<Guid>();
            ContentTypes = new List<SearchEntityTypeEnum>();
            ChannelTypeIds = new List<Guid>();
            SubOrganizationIds = new List<Guid>();
            ServiceServiceType = new List<Guid>();
        }
        
        /// <summary>
        /// unific root ids to be find
        /// </summary>
        public IList<Guid> EntityIds { get; set; }
        
        /// <summary>
        /// version ids to be find
        /// </summary>
        public IList<Guid> EntityVersionIds { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Size of one page
        /// </summary>
        public int MaxPageCount { get; set; }

        /// <summary>
        /// Search for content merged to one field
        /// </summary>
        public SearchEntityTypeEnum ContentTypeEnum { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the ontology word identifiers.
        /// </summary>
        /// <value>
        /// The ontology words.
        /// </value>
        public List<Guid> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the service classes identifier.
        /// </summary>
        /// <value>
        /// The service class identifiers.
        /// </value>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifiers.
        /// </value>
        public List<Guid> ServiceTypes { get; set; }
        /// <summary>
        /// Gets or sets the general description type identifiers.
        /// </summary>
        /// <value>
        /// The general description type identifiers.
        /// </value>
        public List<Guid> GeneralDescriptionTypes { get; set; }  
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Defines if ExtendPublishingStatusesByEquivalents should be used
        /// </summary>
        public bool UseOnlySelectedStatuses { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public List<string> Languages { get; set; }
        /// <summary>
        /// Gets or sets the language ids.
        /// </summary>
        /// <value>
        /// The language ids.
        /// </value>
        [JsonIgnore]
        public List<Guid> LanguageIds { get; set; }
        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        /// <value>
        /// The skip amount.
        /// </value>
        public int Skip { get; set; }
        /// <summary>
        /// Sort params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
        /// <summary>
        /// define when entity expire in weeks
        /// </summary>
        public int ExpirationInWeeks { get; set; }
        /// <summary>
        /// Gets or sets the target group identifiers.
        /// </summary>
        /// <value>
        /// The target group identifiers.
        /// </value>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the industrial class identifiers.
        /// </summary>
        /// <value>
        /// The industrial class identifiers.
        /// </value>
        public List<Guid> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the life event identifiers.
        /// </summary>
        /// <value>
        /// The life event identifiers.
        /// </value>
        public List<Guid> LifeEvents { get; set; }
        /// <summary>
        /// The search content types 
        /// </summary>
        public List<SearchEntityTypeEnum> ContentTypes { get; set; }
        /// <summary>
        /// The search channels content types 
        /// </summary>
        [JsonIgnore]
        public List<Guid> ChannelTypeIds { get; set; }
        /// <summary>
        /// All sub organization ids 
        /// </summary>
        [JsonIgnore]
        public List<Guid> SubOrganizationIds { get; set; }
        /// <summary>
        /// User name 
        /// </summary>
        [JsonIgnore]
        public string UserName { get; set; }
        /// <summary>
        /// Show only my content 
        /// </summary>
        public bool MyContent { get; set; }
        /// <summary>
        /// Service service type  
        /// </summary>
        [JsonIgnore]
        public List<Guid> ServiceServiceType { get; set; }

        /// <summary>
        /// Type of search request
        /// </summary>
        public SearchTypeEnum? SearchType { get; set; }

        /// <summary>
        /// ID of street that user searches for
        /// </summary>
        [JsonProperty("Street")]
        public Guid? AddressStreetId { get; set; }
        
        /// <summary>
        /// ID of street number that user searches for
        /// </summary>
        [JsonProperty("StreetNumberRange")]
        public Guid? AddressStreetNumberId { get; set; }
        
        /// <summary>
        /// String format of street number that user searches for
        /// </summary>
        public string StreetNumber { get; set; }
        
        /// <summary>
        /// ID of Postal code that user searches for
        /// </summary>
        [JsonProperty("PostalCode")]
        public Guid? PostalCodeId { get; set; }
        
        /// <summary>
        /// String format of phone number that user searches for
        /// </summary>
        [JsonProperty("Phone")]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// ID of Phone dial code (prefix) that user searches for
        /// </summary>
        [JsonProperty("DialCode")]
        public Guid? PhoneDialCode { get; set; }
        
        /// <summary>
        /// Checks whether the phone number is local (Finnish, without a dial code).
        /// </summary>
        public bool? IsLocalNumber { get; set; }
        
        /// <summary>
        /// Searched email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// define when entity expire
        /// </summary>
        public DateTime Expiration { get; set; }
        
        /// <summary>
        /// define if entity should be ordered by localized subtype(implemented only for services)
        /// </summary>
        public bool UseLocalizedSubType { get; set; }
        
        /// <summary>
        /// ui language for sorting
        /// </summary>
        public string Language { get; set; }
    }
}
