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
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service channel base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class ServiceChannelBaseController : BaseController
    {
        private IChannelService channelService;
        private IOrganizationService organizationService;
        private ICodeService codeService;
        private IServiceService serviceService;
        private ICommonService commonService;
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
        public ServiceChannelBaseController(
            IChannelService channelService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IServiceService serviceService,
            IOptions<AppSettings> settings,
            ILogger logger,
            ICommonService commonService,
            int versionNumber,
            IUserOrganizationService userOrganizationService)
            : base(userOrganizationService, settings, logger)
        {
            this.channelService = channelService;
            this.organizationService = organizationService;
            this.codeService = codeService;
            this.serviceService = serviceService;
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
            this.commonService = commonService;
        }

        /// <summary>
        /// Get service channel base.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected virtual IActionResult GetById(string id, bool getOnlyPublished = true)
        {
            Guid guid = id.ParseToGuidWithExeption();

            var sc = ChannelService.GetServiceChannelById(guid, versionNumber, getOnlyPublished ? VersionStatusEnum.Published : VersionStatusEnum.LatestActive);

            if (sc == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channel with id '{id}' not found." });
            }

            return Ok(sc);
        }

        /// <summary>
        /// Get service channel base.
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByIdList(string guids, bool getOnlyPublished = true)
        {
            if (string.IsNullOrEmpty(guids))
            {
                ModelState.AddModelError("guids", "Property guids is required.");
                return new BadRequestObjectResult(ModelState);
            }
            List<Guid> guidList = new List<Guid>();
            var list = guids.Split(',');
            try
            {
                list.ForEach(id => guidList.Add(id.Trim().ParseToGuidWithExeption()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("guids", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }

            var scs = ChannelService.GetServiceChannels(guidList);

            if (scs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channels not found." });
            }

            return Ok(scs);
        }

        /// <summary>
        /// Get service channel by type base.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByTypeBase(string type, DateTime? date)
        {
            var channelType = type.Parse<ServiceChannelTypeEnum>();

            var scs = ChannelService.GetServiceChannelsWithDetailsByType(channelType, date, versionNumber);

            if (scs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channels not found." });
            }
            return Ok(scs);
        }

        /// <summary>
        /// Get service channel by type base.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        protected virtual IActionResult GetGuidPageByType(string type, DateTime? date, int page, bool getOnlyPublished = true)
        {
            var channelType = type.Parse<ServiceChannelTypeEnum>();
            return Ok(ChannelService.GetServiceChannelsByType(channelType, date, page, pageSize, getOnlyPublished));
        }

        /// <summary>
        /// Get service channel by organization and type base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByOrganizationIdAndTypeBase(string organizationId, string type, DateTime? date)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            ServiceChannelTypeEnum? channelType = null;
            if (!string.IsNullOrEmpty(type)) { channelType = type.Parse<ServiceChannelTypeEnum>(); }

            var scs = ChannelService.GetServiceChannelsByOrganization(guid, date, versionNumber, channelType);

            if (scs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Service channels not found." });
            }
            return Ok(scs);
        }

        /// <summary>
        /// Get service channel by organization and type base.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected virtual IActionResult GetGuidPageByOrganizationIdAndType(string organizationId, string type, DateTime? date, int page)
        {
            Guid guid = organizationId.ParseToGuidWithExeption();

            // check if the organization exists with the given id
            if (!commonService.OrganizationExists(guid, PublishingStatus.Published))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{guid}' not found." });
            }

            ServiceChannelTypeEnum? channelType = null;
            if (!string.IsNullOrEmpty(type)) { channelType = type.Parse<ServiceChannelTypeEnum>(); }
            return Ok(ChannelService.GetServiceChannelsByOrganization(guid, date, page, pageSize, channelType));
        }

        /// <summary>
        /// Gets a list of service channels related to defined municipality.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByMunicipality(string code, [FromQuery]DateTime? date, [FromQuery]int page)
        {
            // check if municipality with given code exists
            var municipality = codeService.GetMunicipalityByCode(code, true);
            if (municipality == null || !municipality.Id.IsAssigned())
            {
                return NotFound(new VmError() { ErrorMessage = $"Municipality with code '{code}' not found." });
            }
            return Ok(ChannelService.GetServiceChannelsByMunicipality(municipality.Id, date, page, pageSize));
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiElectronicChannelInVersionBase;
            var validator = new ElectronicChannelValidator(vmBase, commonService, codeService, serviceService);

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
            
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, commonService, codeService, serviceService);
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPhoneChannelInVersionBase;
            var validator = new PhoneChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, commonService, codeService, serviceService, versionNumber);
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, commonService, codeService, serviceService, versionNumber);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiWebPageChannelInVersionBase;
            var validator = new WebPageChannelValidator(vmBase, commonService, codeService, serviceService, versionNumber);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, commonService, codeService, serviceService);
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiPrintableFormChannelInVersionBase;
            var validator = new PrintableFormChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, commonService, codeService, serviceService);
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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, commonService, codeService, serviceService);

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

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var vmBase = request.VersionBase() as VmOpenApiServiceLocationChannelInVersionBase;
            var validator = new ServiceLocationChannelValidator(vmBase, commonService, codeService, serviceService);

            return PutServiceChannel(vmBase, validator, "Service location", sourceId: sourceId);
        }
        
        private IActionResult PostServiceChannel<TModel, TValidator>(TModel vmBase, TValidator validator) where TModel : class, IVmOpenApiServiceChannelIn where TValidator : ServiceChannelValidator<TModel>
        {
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

            SetAddressProperties(vmBase);

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

            // Set current version
            IVmOpenApiServiceChannel currentVersion = null;
            if (!string.IsNullOrEmpty(id))
            {
                vmBase.Id = id.ParseToGuid();
                if (!vmBase.Id.IsAssigned())
                {
                    return NotFound(new VmError() { ErrorMessage = $"{channelName} channel with id '{id}' not found." });
                }
                currentVersion = channelService.GetServiceChannelByIdSimple(vmBase.Id.Value, false);
            }
            else if (!string.IsNullOrEmpty(sourceId))
            {
                vmBase.SourceId = sourceId;
                currentVersion = ChannelService.GetServiceChannelBySource(sourceId);
            }
 
            // Check current version and data
            if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
            {
                if (vmBase.Id.IsAssigned())
                {
                    return NotFound(new VmError { ErrorMessage = $"{channelName} channel with id '{id}' not found." });
                }
                else
                {
                    return NotFound(new VmError { ErrorMessage = $"{channelName} channel with source id '{vmBase.SourceId}' not found." });
                }
            }

            // check lock (PTV-3391)
            if (currentVersion.Id.IsAssigned())
            { 
                var entityLock = channelService.EntityLockedBy(currentVersion.Id.Value);
                if (entityLock.EntityLockStatus != EntityLockEnum.Unlocked)
                {
                    return NotFound(new VmError {ErrorMessage = $"{channelName} channel is locked by '{entityLock.LockedBy}'."});
                }
            }

            // Has user rights for the channel
            var isOwnOrganization = ((VmOpenApiServiceChannel)currentVersion).Security == null ? false : ((VmOpenApiServiceChannel)currentVersion).Security.IsOwnOrganization;
            if (UserRole() != UserRoleEnum.Eeva && !isOwnOrganization)
            {
                return NotFound(new VmError() { ErrorMessage = $"User has no rights to update or create this entity!" });
            }

            // Get languages user has added into model
            vmBase.AvailableLanguages = GetServiceChannelAvailableLanguages(vmBase).ToList();
           
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

            SetAddressProperties(vmBase);

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

        private void SetAddressProperties(IVmOpenApiServiceChannelIn model)
        {            
            if (model == null && !(model is VmOpenApiServiceLocationChannelInVersionBase)) return;

            var slModel = model as VmOpenApiServiceLocationChannelInVersionBase;

            if (slModel == null || slModel.Addresses == null) return;

            slModel.Addresses.ForEach(a =>
            {
                if (a.Type == AddressConsts.LOCATION)
                {
                    a.Type = AddressCharacterEnum.Visiting.ToString();
                }

                switch (a.SubType)
                {
                    case AddressConsts.ABROAD:
                        a.SubType = AddressTypeEnum.Foreign.ToString();
                        break;
                    case AddressConsts.MULTIPOINT:
                        a.SubType = AddressTypeEnum.Moving.ToString();
                        break;
                    case AddressConsts.SINGLE:
                        a.SubType = AddressTypeEnum.Street.ToString();
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
