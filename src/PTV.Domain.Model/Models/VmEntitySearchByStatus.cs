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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service search form
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmOrganizationEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntitySearch" />
    public class VmEntitySearchByStatus : IVmEntitySearchByStatus
    {
        private readonly IVmEntitySearch search;
        private readonly List<Guid> publishedStatuses;
        private readonly List<Guid> archivedStatuses;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="publishedStatuses"></param>
        /// <param name="archivedStatuses"></param>
        public VmEntitySearchByStatus(IVmEntitySearch search, List<Guid> publishedStatuses, List<Guid> archivedStatuses)
        {
            this.search = search;
            this.publishedStatuses = publishedStatuses;
            this.archivedStatuses = archivedStatuses;
        }
        
        /// <inheritdoc />
        public Guid? Id { get => search.Id; set => search.Id = value; }
        /// <inheritdoc />
        public int PageNumber { get => search.PageNumber; set => search.PageNumber = value; }
        /// <inheritdoc />
        public int MaxPageCount { get => search.MaxPageCount; set => search.MaxPageCount = value; }
        /// <inheritdoc />
        public int Skip { get => search.Skip; set => search.Skip = value; }
        /// <inheritdoc />
        public List<VmSortParam> SortData { get => search.SortData; set => search.SortData = value; }
        /// <inheritdoc />
        public List<string> Languages { get => search.Languages; set => search.Languages = value; }
        /// <inheritdoc />
        public Guid? OrganizationId { get => search.OrganizationId; set => search.OrganizationId = value; }
        /// <inheritdoc />
        public List<Guid> OrganizationIds { get => search.OrganizationIds; set => search.OrganizationIds = value; }
        /// <inheritdoc />
        public IList<Guid> EntityIds { get => search.EntityIds; set => search.EntityIds = value; }
        /// <inheritdoc />
        public IList<Guid> EntityVersionIds { get => search.EntityVersionIds; set => search.EntityVersionIds = value; }
        /// <inheritdoc />
        public List<SearchEntityTypeEnum> ContentTypes { get => search.ContentTypes; set => search.ContentTypes = value; }
        /// <inheritdoc />
        public SearchEntityTypeEnum ContentTypeEnum { get => search.ContentTypeEnum; set => search.ContentTypeEnum = value; }
        /// <inheritdoc />
        public string Name { get => search.Name; set => search.Name = value; }
        /// <inheritdoc />
        public List<Guid> OntologyTerms { get => search.OntologyTerms; set => search.OntologyTerms = value; }
        /// <inheritdoc />
        public List<Guid> ServiceClasses { get => search.ServiceClasses; set => search.ServiceClasses = value; }
        /// <inheritdoc />
        public List<Guid> ServiceTypes { get => search.ServiceTypes; set => search.ServiceTypes = value; }
        /// <inheritdoc />
        public List<Guid> GeneralDescriptionTypes { get => search.GeneralDescriptionTypes; set => search.GeneralDescriptionTypes = value; }
        /// <inheritdoc />
        public List<Guid> TargetGroups { get => search.TargetGroups; set => search.TargetGroups = value; }
        /// <inheritdoc />
        public List<Guid> IndustrialClasses { get => search.IndustrialClasses; set => search.IndustrialClasses = value; }
        /// <inheritdoc />
        public List<Guid> LifeEvents { get => search.LifeEvents; set => search.LifeEvents = value; }

        /// <inheritdoc />
        public List<Guid> SelectedPublishingStatuses
        {
            get => search.SelectedPublishingStatuses.Intersect(publishedStatuses).ToList();
            set => search.SelectedPublishingStatuses = value;
        }
        /// <inheritdoc />
        public bool UseOnlySelectedStatuses { get => search.UseOnlySelectedStatuses; set => search.UseOnlySelectedStatuses = value; }
        /// <inheritdoc />
        public int ExpirationInWeeks { get => search.ExpirationInWeeks; set => search.ExpirationInWeeks = value; }
        /// <inheritdoc />
        public List<Guid> LanguageIds { get => search.LanguageIds; set => search.LanguageIds = value; }
        /// <inheritdoc />
        public List<Guid> ChannelTypeIds { get => search.ChannelTypeIds; set => search.ChannelTypeIds = value; }
        /// <inheritdoc />
        public List<Guid> SubOrganizationIds { get => search.SubOrganizationIds; set => search.SubOrganizationIds = value; }
        /// <inheritdoc />
        public string UserName { get => search.UserName; set => search.UserName = value; }
        /// <inheritdoc />
        public bool MyContent { get => search.MyContent; set => search.MyContent = value; }
        /// <inheritdoc />
        public List<Guid> ServiceServiceType { get => search.ServiceServiceType; set => search.ServiceServiceType = value; }
        /// <inheritdoc />
        public SearchTypeEnum? SearchType { get => search.SearchType; set => search.SearchType = value; }
        /// <inheritdoc />
        public Guid? AddressStreetId { get => search.AddressStreetId; set => search.AddressStreetId = value; }
        /// <inheritdoc />
        public Guid? AddressStreetNumberId { get => search.AddressStreetNumberId; set => search.AddressStreetNumberId = value; }
        /// <inheritdoc />
        public string StreetNumber { get => search.StreetNumber; set => search.StreetNumber = value; }
        /// <inheritdoc />
        public string PhoneNumber { get => search.PhoneNumber; set => search.PhoneNumber = value; }
        /// <inheritdoc />
        public Guid? PhoneDialCode { get => search.PhoneDialCode; set => search.PhoneDialCode = value; }
        /// <inheritdoc />
        public Guid? PostalCodeId { get => search.PostalCodeId; set => search.PostalCodeId = value; }
        /// <inheritdoc />
        public bool? IsLocalNumber { get => search.IsLocalNumber; set => search.IsLocalNumber = value; }
        /// <inheritdoc />
        public string Email { get => search.Email; set => search.Email = value; }
        /// <inheritdoc />
        public DateTime Expiration { get => search.Expiration; set => search.Expiration = value; }
        /// <inheritdoc />
        public bool UseLocalizedSubType { get => search.UseLocalizedSubType; set => search.UseLocalizedSubType = value; }
        /// <inheritdoc />
        public string Language { get => search.Language; set => search.Language = value; }
        /// <inheritdoc />
        public Guid? LanguageId { get => search.LanguageId; set => search.LanguageId = value; }
        /// <inheritdoc />
        public bool UsePreferredPublishingStatus { get => search.UsePreferredPublishingStatus; set => search.UsePreferredPublishingStatus = value; }
        /// <inheritdoc />
        public bool UseOrganizationSorting { get => search.UseOrganizationSorting; set => search.UseOrganizationSorting = value; }
        /// <inheritdoc />
        public List<Guid> AreaInformationTypes { get => search.AreaInformationTypes; set => search.AreaInformationTypes = value; }
        /// <inheritdoc />
        public List<Guid> RestrictedTypes { get => search.RestrictedTypes; set => search.RestrictedTypes = value; }

        /// <inheritdoc />
        public List<Guid> SelectedArchivedStatuses
        {
            get => search.SelectedPublishingStatuses.Intersect(archivedStatuses).ToList();
            set => search.SelectedPublishingStatuses = value;
        }
    }
}
