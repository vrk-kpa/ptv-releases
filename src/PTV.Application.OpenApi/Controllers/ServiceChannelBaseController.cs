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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.DataValidators;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service channel base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.ValidationBaseController" />
    public class ServiceChannelBaseController : ValidationBaseController
    {
        private IChannelService channelService;
        private IOrganizationService organizationService;
        private ICodeService codeService;
        private int pageSize;
        private int versionNumber;

        /// <summary>
        /// Gets the ChannelService instance.
        /// </summary>
        protected IChannelService ChannelService { get { return channelService; } private set { } }

        /// <summary>
        /// Get the page size defined in application settings.
        /// </summary>
        protected int PageSize { get { return pageSize; } private set { } }

        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public ServiceChannelBaseController(IChannelService channelService, IOrganizationService organizationService,
            ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService, ILogger logger, int versionNumber) : base(organizationService, codeService, settings, fintoService, logger)
        {
            this.channelService = channelService;
            this.organizationService = organizationService;
            this.codeService = codeService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the identifier list.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="page">The page.</param>
        /// <returns>identifier list</returns>
        protected IActionResult GetIdList(DateTime? date, int page)
        {
            return Ok(ChannelService.GetServiceChannels(date, page, PageSize));
        }

        /// <summary>
        /// Get service channel base.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IActionResult Get(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var sc = ChannelService.GetServiceChannelById(guid, versionNumber);

            if (sc == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{id}' not found." });
            }

            return Ok(sc);
        }

        /// <summary>
        /// Get service channel by type base.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual IActionResult GetByType(string type, DateTime? date)
        {
            var channelType = type.Parse<ServiceChannelTypeEnum>();
            return Ok(ChannelService.GetServiceChannelsByType(channelType, date, versionNumber));
        }

        /// <summary>
        /// Get service channel by organization base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual IActionResult GetByOrganizationId(string organizationId, DateTime? date)
        {
            return GetByOrganizationIdAndType(organizationId, date);
        }

        /// <summary>
        /// Get service channel by organization and type base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual IActionResult GetByOrganizationIdAndType(string organizationId, string type, [FromQuery]DateTime? date)
        {
            return GetByOrganizationIdAndType(organizationId, date, type);
        }

        /// <summary>
        /// Post electronic channel base.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IActionResult PostEChannel(IVmOpenApiElectronicChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, organizationService, codeService);

            return PostServiceChannel(vmBase, validator);
        }

        /// <summary>
        /// Puts the e channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutEChannel(string id, IVmOpenApiElectronicChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Electronic", id);
        }

        /// <summary>
        /// Puts the e channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutEChannelBySource(string sourceId, IVmOpenApiElectronicChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Electronic", sourceId: sourceId);
        }

        /// <summary>
        /// Posts the phone channel base.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostPhoneChannel(IVmOpenApiPhoneChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, organizationService, codeService);
            return PostServiceChannel(vmBase, validator);
        }

        /// <summary>
        /// Puts the phone channel base.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPhoneChannel(string id, IVmOpenApiPhoneChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Phone", id);
        }

        /// <summary>
        /// Puts the phone channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPhoneChannelBySource(string sourceId, IVmOpenApiPhoneChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Phone", sourceId: sourceId);
        }

        /// <summary>
        /// Posts the web page channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostWebPageChannel(IVmOpenApiWebPageChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, organizationService, codeService);
            return PostServiceChannel(vmBase, validator);
        }

        /// <summary>
        /// Puts the web page channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutWebPageChannel(string id, IVmOpenApiWebPageChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Web page", id);
        }

        /// <summary>
        /// Puts the web page channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutWebPageChannelBySource(string sourceId, IVmOpenApiWebPageChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Web page", sourceId: sourceId);
        }

        /// <summary>
        /// Posts the printable form channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostPrintableFormChannel(IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, organizationService, codeService);
            return PostServiceChannel(vmBase, validator);
        }

        /// <summary>
        /// Puts the printable form channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPrintableFormChannel(string id, IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Printable form", id);
        }

        /// <summary>
        /// Puts the printable form channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutPrintableFormChannelBySource(string sourceId, IVmOpenApiPrintableFormChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Printable form", sourceId: sourceId);
        }

        /// <summary>
        /// Posts the service location channel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PostServiceLocationChannel(IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, organizationService, codeService);
            return PostServiceChannel(vmBase, validator);
        }

        /// <summary>
        /// Puts the service location channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutServiceLocationChannel(string id, IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Service location", id);
        }

        /// <summary>
        /// Puts the service location channel by source.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult PutServiceLocationChannelBySource(string sourceId, IVmOpenApiServiceLocationChannelInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, organizationService, codeService);

            return PutServiceChannel(vmBase, validator, "Service location", sourceId: sourceId);
        }

        private IActionResult GetByOrganizationIdAndType(string organizationId, DateTime? date, string type = null)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!OrganizationService.OrganizationExists(guid))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            ServiceChannelTypeEnum? channelType = null;
            if (!string.IsNullOrEmpty(type)) { channelType = type.Parse<ServiceChannelTypeEnum>(); }
            return Ok(ChannelService.GetServiceChannelsByOrganization(guid, date, versionNumber, channelType));
        }

        private IActionResult PostServiceChannel<TModel, TValidator>(TModel vmBase, TValidator validator) where TModel : class, IVmOpenApiServiceChannelIn where TValidator : ServiceChannelValidator<TModel>
        {
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Get languages user has added into model
            vmBase.AvailableLanguages = GetServiceChannelAvailableLanguages(vmBase).ToList();
            // For post method the required and available languages are the same
            validator.RequiredLanguages = vmBase.AvailableLanguages;
            validator.AvailableLanguages = vmBase.AvailableLanguages;

            // Set publishing status as Draft or Published
            if (vmBase.PublishingStatus != PublishingStatus.Published.ToString())
            {
                vmBase.PublishingStatus = PublishingStatus.Draft.ToString();
            }

            // Validate request data
            validator.Validate(ModelState);
            // Check validation status
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(ChannelService.AddServiceChannel(vmBase, Settings.AllowAnonymous, versionNumber));
        }

        private IActionResult PutServiceChannel<TModel, TValidator>(TModel vmBase, TValidator validator, string channelName, string id = null, string sourceId = null)
            where TModel : class, IVmOpenApiServiceChannelIn where TValidator : ServiceChannelValidator<TModel>
        {
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            IList<string> newLanguages = new List<string>();

            // check that the channel exists
            if (!string.IsNullOrEmpty(id) && !CheckChannelExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"{channelName} channel with id '{id}' not found." });
            }

            // Set entity identifiers
            if (!string.IsNullOrEmpty(id)) { vmBase.Id = id.ParseToGuid(); }
            if (!string.IsNullOrEmpty(sourceId)) { vmBase.SourceId = sourceId; }

            // Get languages user has added into model
            vmBase.AvailableLanguages = GetServiceChannelAvailableLanguages(vmBase).ToList();

            // Get the current version and data related to it
            var currentVersion = vmBase.Id.IsAssigned() ? ChannelService.GetServiceChannelById(vmBase.Id.Value, 0, false) : ChannelService.GetServiceChannelBySource(sourceId, 0, false);
            //request.CurrentPublishingStatus = request.Id.IsAssigned() ? ChannelService.GetChannelStatusByRootId(request.Id.Value) : ChannelService.GetChannelStatusBySourceId(sourceId);
            if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
            {
                if (vmBase.Id.IsAssigned())
                {
                    return NotFound(new VmError() { ErrorMessage = $"Version for service channel with id '{vmBase.Id.Value}' not found." });
                }
                else
                {
                    return NotFound(new VmError() { ErrorMessage = $"Version for service channel with source id '{vmBase.SourceId}' not found." });
                }
            }

            vmBase.ChannelId = currentVersion.ChannelId;

            // Set the publishing status for vm and for validator
            vmBase.CurrentPublishingStatus = currentVersion.PublishingStatus;
            validator.CurrentPublishingStatus = currentVersion.PublishingStatus;

            // Get the available languages from current version and from request.
            // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
            // Required languages are the ones that are just added within request.
            validator.RequiredLanguages = vmBase.AvailableLanguages.Where(i => !currentVersion.AvailableLanguages.Contains(i)).ToList();
            // Available languages is a combination from current (version) available languages and newly set available languages
            var list = new HashSet<string>();
            vmBase.AvailableLanguages.ForEach(i => list.Add(i));
            currentVersion.AvailableLanguages.ForEach(i => list.Add(i));
            validator.AvailableLanguages = list.ToList();

            // Validate request data
            validator.Validate(ModelState);
            // Check validation status
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(ChannelService.SaveServiceChannel(vmBase, Settings.AllowAnonymous, versionNumber));
        }
        
        /// <summary>
        /// Checks if a service channel with given id exists in the database.
        /// </summary>
        /// <param name="channelId">channel guid as string</param>
        /// <returns>true if the channel exists otherwise false</returns>
        protected bool CheckChannelExists(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
            {
                return false;
            }

            Guid? chId = channelId.ParseToGuid();

            if (!chId.HasValue)
            {
                return false;
            }

            return ChannelService.ChannelExists(chId.Value);
        }

        private HashSet<string> GetServiceChannelAvailableLanguages(IVmOpenApiServiceChannelIn vModel)
        {
            var list = new HashSet<string>();
            list.GetAvailableLanguages(vModel.ServiceChannelNames);
            list.GetAvailableLanguages(vModel.ServiceChannelDescriptions);

            return list;
        }
    }
}
