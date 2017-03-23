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

using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using Microsoft.Extensions.Logging;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Includes the methods for validating Open Api request parameters.
    /// </summary>
    public class ValidationBaseController : BaseController
    {
        private IOrganizationService organizationService;
        private ICodeService codeService;
        private IFintoService fintoService;

        /// <summary>
        /// Gets the OrganizationService instance.
        /// </summary>
        protected IOrganizationService OrganizationService { get { return organizationService; } private set { } }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="organizationService">The organization service. Used for getting organization values from db.</param>
        /// <param name="codeService">The code service. Used for getting codes values from db.</param>
        /// <param name="settings">Application settings</param>
        /// <param name="fintoService">IFintoService instance</param>
        /// <param name="logger">ILogger instance</param>
        public ValidationBaseController(IOrganizationService organizationService, ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService, ILogger logger) : base(settings, logger)
        {
            this.organizationService = organizationService;
            this.codeService = codeService;
            this.fintoService = fintoService;
        }

        /// <summary>
        /// Checks that the organization exists in db
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateOrganization(string id, string propertyName = "OrganizationId")
        {
            if (string.IsNullOrEmpty(id))
                return;

            Guid guid;
            Guid.TryParse(id, out guid);
            if (guid == null)
            {
                this.ModelState.AddModelError(propertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }

            if (!organizationService.OrganizationExists(guid))
            {
                this.ModelState.AddModelError(propertyName, CoreMessages.OpenApi.RecordNotFound);
            }
        }

        /// <summary>
        /// Validates the oid.
        /// </summary>
        /// <param name="oid">The oid.</param>
        protected void ValidateOid(string oid)
        {
            if (string.IsNullOrEmpty(oid))
                return;

            if (organizationService.GetOrganizationIdByOid(oid) != Guid.Empty)
            {
                this.ModelState.AddModelError("Oid", string.Format(CoreMessages.OpenApi.OidExists, oid));
            }
        }

        /// <summary>
        /// Validates the oid.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="organizationId">The organization identifier.</param>
        protected void ValidateOid(string oid, Guid organizationId)
        {
            if (string.IsNullOrEmpty(oid))
                return;

            Guid orgId = organizationService.GetOrganizationIdByOid(oid);

            // OID exists in the database and it is used by some other organization
            if (orgId != Guid.Empty && orgId != organizationId)
            {
                this.ModelState.AddModelError("Oid", string.Format(CoreMessages.OpenApi.OidExists, oid));
            }
        }

        /// <summary>
        /// Validates the oid.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <param name="sourceId">The source identifier.</param>
        protected void ValidateOid(string oid, string sourceId)
        {
            if (string.IsNullOrEmpty(oid))
                return;
            var idByOid = organizationService.GetOrganizationIdByOid(oid);
            var idBySource = organizationService.GetOrganizationIdBySource(sourceId);
            // If the organization id was found from external sources and it was not the same than the id related to Oid, validation was not successfull.
            if (idBySource != Guid.Empty && idByOid != Guid.Empty && idByOid != idBySource)
            {
                this.ModelState.AddModelError("Oid", string.Format(CoreMessages.OpenApi.OidExists, oid));
            }
        }

        /// <summary>
        /// Checks that municipality code is valid.
        /// </summary>
        /// <param name="code">Municipality code</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateMunicipalityCode(string code, string propertyName)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }
            var municipality = codeService.GetMunicipalityByCode(code);
            if (municipality == null || municipality.Id == null)
            {
                this.ModelState.AddModelError(propertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, code));
            }
        }

        /// <summary>
        /// Checks that country code is valid.
        /// </summary>
        /// <param name="code">Country code</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateCountryCode(string code, string propertyName)
        {
            var country = codeService.GetCountryByCode(code);
            if (string.IsNullOrEmpty(country))
            {
                this.ModelState.AddModelError(propertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, code));
            }
        }

        /// <summary>
        /// Checks that postal code is valid.
        /// </summary>
        /// <param name="code">Postal code</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidatePostalCode(string code, string propertyName)
        {
            var postalInfo = codeService.GetPostalCodeByCode(code);
            if (string.IsNullOrEmpty(postalInfo))
            {
                this.ModelState.AddModelError(propertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, code));
            }
        }

        /// <summary>
        /// Checks address validity.
        /// </summary>
        /// <param name="address">Address model</param>
        /// <param name="propertyName">The property name used in request</param>
        /// <param name="updateElement">default is false</param>
        protected void ValidateAddress(IV4VmOpenApiAddressIn address, string propertyName, bool updateElement = false)
        {
            if (address == null) return;

            // Check that municipality code is valid.
            ValidateMunicipalityCode(address.Municipality, string.Format("{0}.Municipality", propertyName));
            // Check that country code is valid.
            ValidateCountryCode(address.Country, string.Format("{0}.Country", propertyName));
            // Check that postal code is valid.
            ValidatePostalCode(address.PostalCode, string.Format("{0}.PostalCode", propertyName));
        }

        /// <summary>
        /// Checks validity of the addresses in address list.
        /// </summary>
        /// <param name="list">List of addresses</param>
        /// <param name="updateElement">default value is false</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateAddressList<TModel>(IList<TModel> list, bool updateElement = false, string propertyName = "Addresses") where TModel : IV4VmOpenApiAddressWithTypeIn
        {
            if (list == null)
                return;

            var i = 0;
            list.ForEach(address =>
            {
                ValidateAddress(address, string.Format("{0}[{1}]", propertyName, i++), updateElement);
            });
        }

        /// <summary>
        /// Checks validity of language code list.
        /// </summary>
        /// <param name="list">List of language codes</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateLanguageList(IReadOnlyList<string> list, string propertyName = "Languages")
        {
            if (list == null) return;
            var i = 0;
            list.ForEach(l => ValidateLanguage(l, string.Format("{0}[{1}]", propertyName, i++)));
        }

        /// <summary>
        /// Checks validity of service classes (uri list).
        /// </summary>
        /// <param name="list">List of uris</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateServiceClasses(IList<string> list, string propertyName = "ServiceClasses")
        {
            var i = 0;
            list.ForEach(l =>
            {
                var item = fintoService.GetServiceClassByUri(l);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Format("{0}[{1}]", propertyName, i++), string.Format(CoreMessages.OpenApi.CodeNotFound, l));
                }
            });
        }

        /// <summary>
        /// Checks validity of ontology terms (uri list).
        /// </summary>
        /// <param name="list">List of uris</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateOntologyTerms(IList<string> list, string propertyName = "OntologyTerms")
        {
            var i = 0;
            list.ForEach(l =>
            {
                var item = fintoService.GetOntologyTermByUri(l);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Format("{0}[{1}]", propertyName, i++), string.Format(CoreMessages.OpenApi.CodeNotFound, l));
                }
            });
        }

        /// <summary>
        /// Checks validity of target groups (uri list).
        /// </summary>
        /// <param name="list">List of uris</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateTargetGroups(IList<string> list, string propertyName = "TargetGroups")
        {
            var i = 0;
            list.ForEach(l =>
            {
                var item = fintoService.GetTargetGroupByUri(l);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Format("{0}[{1}]", propertyName, i++), string.Format(CoreMessages.OpenApi.CodeNotFound, l));
                }
            });
        }

        /// <summary>
        /// Checks validity of life events (uri list)
        /// </summary>
        /// <param name="list">List of uris</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateLifeEvents(IList<string> list, string propertyName = "LifeEvents")
        {
            var i = 0;
            list.ForEach(l =>
            {
                var item = fintoService.GetLifeEventByUri(l);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Format("{0}[{1}]", propertyName, i++), string.Format(CoreMessages.OpenApi.CodeNotFound, l));
                }
            });
        }

        /// <summary>
        /// Checks validity of industrial classes (uri list)
        /// </summary>
        /// <param name="list">List of uris</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidateIndustrialClasses(IList<string> list, string propertyName = "IndustrialClasses")
        {
            var i = 0;
            list.ForEach(l =>
            {
                var item = fintoService.GetIndustrialClassByUri(l);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Format("{0}[{1}]", propertyName, i++), string.Format(CoreMessages.OpenApi.CodeNotFound, l));
                }
            });
        }

        /// <summary>
        /// Checks validity of the phone numbers in phone list.
        /// </summary>
        /// <param name="list">List of phone numbers</param>
        /// <param name="propertyName">The property name used in request</param>
        protected void ValidatePhoneNumberList<TModel>(IList<TModel> list, string propertyName = "PhoneNumbers") where TModel : IVmOpenApiPhoneSimpleVersionBase
        {
            if (list == null)
                return;

            var i = 0;
            list.ForEach(phone =>
            {
                ValidatePrefixNumber(phone.PrefixNumber, string.Format("{0}[{1}]", propertyName, i++));
            });
        }

        /// <summary>
        /// Validate publishing status.
        /// Allowed updates for publishing status:
        /// Draft     -> Draft or Published
        /// Published -> Modified, Published, Archived
        /// Archived  -> Modified, Published, Archived
        /// </summary>
        /// <param name="currentPublishingStatus">Current publishing status</param>
        /// <param name="strPublishingStatus">New publishing status (from request)</param>
        protected void ValidatePublishingStatus(PublishingStatus currentPublishingStatus, string strPublishingStatus)
        {
            PublishingStatus? newPublishingStatus = null;
            if (!string.IsNullOrEmpty(strPublishingStatus))
            {
                PublishingStatus tmpPublishingStatus;
                if (Enum.TryParse(strPublishingStatus, out tmpPublishingStatus))
                {
                    newPublishingStatus = tmpPublishingStatus;
                }
            }
            switch (currentPublishingStatus)
            {
                case PublishingStatus.Draft:
                    if (newPublishingStatus != null && newPublishingStatus != PublishingStatus.Draft && newPublishingStatus != PublishingStatus.Published)
                    {
                        this.ModelState.AddModelError("PublishingStatus", string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentPublishingStatus));
                    }
                    break;
                case PublishingStatus.Published:
                    if (newPublishingStatus != null && newPublishingStatus != PublishingStatus.Modified && newPublishingStatus != PublishingStatus.Published && newPublishingStatus != PublishingStatus.Deleted)
                    {
                        this.ModelState.AddModelError("PublishingStatus", string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentPublishingStatus));
                    }
                    break;
                case PublishingStatus.Deleted:
                    if (newPublishingStatus != null && newPublishingStatus != PublishingStatus.Modified && newPublishingStatus != PublishingStatus.Published && newPublishingStatus != PublishingStatus.Deleted)
                    {
                        this.ModelState.AddModelError("PublishingStatus", string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentPublishingStatus));
                    }
                    break;
                case PublishingStatus.Modified:
                    this.ModelState.AddModelError("PublishingStatus", "You cannot update entity with status Modified.");
                    break;
                case PublishingStatus.OldPublished:
                    this.ModelState.AddModelError("PublishingStatus", "You cannot update entity with status OldPublished.");
                    break;
                default:
                    break;
            }
        }

        private void ValidateLanguage(string code, string propertyName)
        {
            var languageInfo = codeService.GetLanguageByCode(code);
            if (languageInfo == null)
            {
                this.ModelState.AddModelError(propertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, code));
            }
        }

        private void ValidatePrefixNumber(string prefixNumber, string propertyName)
        {
            if (string.IsNullOrEmpty(prefixNumber)) return;

            var dialCode = codeService.GetDialCode(prefixNumber);
            if (dialCode == null)
            {
                this.ModelState.AddModelError(propertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, prefixNumber));
            }
        }

    }
}
