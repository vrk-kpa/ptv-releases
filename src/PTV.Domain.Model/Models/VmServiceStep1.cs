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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceStep1" />
    /// /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmAreaInformation" />
    public class VmServiceStep1 : VmEnumEntityStatusBase, IVmServiceStep1, IVmEntitySecurity, IVmRootBasedEntity, IVmAreaInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceStep1"/> class.
        /// </summary>
        public VmServiceStep1()
        {
            ServiceName = string.Empty;
            AlternateServiceName = string.Empty;
            ShortDescriptions = string.Empty;
            Description = string.Empty;
            UserInstruction = string.Empty;
            ServiceUsage = string.Empty;
            AdditionalInformation = string.Empty;
            AdditionalInformationDeadLine = string.Empty;
            AdditionalInformationProcessingTime = string.Empty;
            AdditionalInformationValidityTime = string.Empty;
            Languages = new List<Guid>();            
            OverrideTargetGroups = new List<Guid>();
            Laws = new List<VmLaw>();
            AreaBusinessRegions = new List<Guid>();
            AreaHospitalRegions = new List<Guid>();
            AreaProvince = new List<Guid>();
            AreaMunicipality = new List<Guid>();
            ServiceProducers = new List<VmServiceProducer>();
            Organizers = new List<Guid>();
            OrganizersItems = new List<VmTreeItem>();
            ServiceVouchers = new List<VmServiceVoucher>();
            LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>();
        }

        /// <summary>
        /// Language availability information for service
        /// </summary>
        public List<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Gets or sets the general description.
        /// </summary>
        /// <value>
        /// The general description.
        /// </value>
        public VmGeneralDescription GeneralDescription { get; set; }
        /// <summary>
        /// Gets or sets the type identifier of the charge.
        /// </summary>
        /// <value>
        /// The type of the charge.
        /// </value>
        public Guid? ChargeType { get; set; }

        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public Guid? ServiceTypeId { get; set; }
        /// <summary>
        /// Gets or sets the are infomration type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        public Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }
        /// <summary>
        /// Gets or sets the alternate name of the service.
        /// </summary>
        /// <value>
        /// The alternate name of the service.
        /// </value>
        public string AlternateServiceName { get; set; }
        /// <summary>
        /// Gets or sets the short descriptions.
        /// </summary>
        /// <value>
        /// The short descriptions.
        /// </value>
        public string ShortDescriptions { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the user instruction.
        /// </summary>
        /// <value>
        /// The user instruction.
        /// </value>
        public string UserInstruction { get; set; }
        /// <summary>
        /// Gets or sets the service usage.
        /// </summary>
        /// <value>
        /// The service usage.
        /// </value>
        public string ServiceUsage { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public string AdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the additional information dead line.
        /// </summary>
        /// <value>
        /// The additional information dead line.
        /// </value>
        public string AdditionalInformationDeadLine { get; set; }
        /// <summary>
        /// Gets or sets the additional information processing time.
        /// </summary>
        /// <value>
        /// The additional information processing time.
        /// </value>
        public string AdditionalInformationProcessingTime { get; set; }
        /// <summary>
        /// Gets or sets the additional information validity time.
        /// </summary>
        /// <value>
        /// The additional information validity time.
        /// </value>
        public string AdditionalInformationValidityTime { get; set; }
        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public List<Guid> Languages { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
        /// <summary>
        /// Gets or sets the service coverage type identifier.
        /// </summary>
        /// <value>
        /// The service coverage type identifier.
        /// </value>
        public List<VmLaw> Laws { get; set; }
        /// <summary>
        /// Gets or sets target groups, that will override the general description target groups.
        /// </summary>
        /// <value>
        /// The overriding target groups.
        /// </value>
        public List<Guid> OverrideTargetGroups { get; set; }        

        /// <summary>
        /// Entity security information. <see cref="IVmEntitySecurity.Security"/>
        /// </summary>
        public ISecurityOwnOrganization Security { get; set; }

        /// <summary>
        /// Root node identifier
        /// </summary>
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the list of HospitalRegions.
        /// </summary>
        /// <value>
        /// The list of HospitalRegions.
        /// </value>
        public List<Guid> AreaHospitalRegions { get; set; }
        /// <summary>
        /// Gets or sets the list of Municipality.
        /// </summary>
        /// <value>
        /// The list of Municipality.
        /// </value>
        public List<Guid> AreaMunicipality { get; set; }
        /// <summary>
        /// Gets or sets the list of Province.
        /// </summary>
        /// <value>
        /// The list of Province.
        /// </value>
        public List<Guid> AreaProvince { get; set; }
        /// <summary>
        /// Gets or sets the list of BusinessRegions.
        /// </summary>
        /// <value>
        /// The list of BusinessRegions.
        /// </value>
        public List<Guid> AreaBusinessRegions { get; set; }
        /// <summary>
        /// Gets or sets the organizers.
        /// </summary>
        /// <value>
        /// The organizers.
        /// </value>
        public List<Guid> Organizers { get; set; }
        /// <summary>
        /// Gets or sets the organizers items.
        /// </summary>
        /// <value>
        /// The organizers items.
        /// </value>
        [JsonIgnore]
        public List<VmTreeItem> OrganizersItems { get; set; }
        /// <summary>
        /// Gets or sets the service producers.
        /// </summary>
        /// <value>
        /// The service producers.
        /// </value>
        public List<VmServiceProducer> ServiceProducers { get; set; }
        /// <summary>
        /// Gets or sets the funding type identifier.
        /// </summary>
        /// <value>
        /// The funding type identifier.
        /// </value>
        public Guid FundingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the service vouchers.
        /// </summary>
        /// <value>
        /// The service vouchers.
        /// </value>
        public List<VmServiceVoucher> ServiceVouchers { get; set; }
        /// <summary>
        /// Get true if Service vouchers contains some item
        /// </summary>
        public bool IsActiveVoucher
        {
            get
            {
                return ServiceVouchers.Any();
            }
        }
        /// <summary>
        /// Get true if Service has attached general desctiption
        /// </summary>
        public bool IsGeneralDescriptionAttached
        {
            get
            {
                return GeneralDescription?.Id.HasValue ?? false;
            }
        }
    }
}
