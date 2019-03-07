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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTV.Application.OpenApi.DataValidators;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save service manager
    /// </summary>
    public class SaveServiceManager : SaveEntityManagerBase<IVmOpenApiServiceInVersionBase, IVmOpenApiServiceVersionBase>
    {
        private bool _attachProposedChannels;

        private IServiceService _service;
        private IGeneralDescriptionService _gdService;
        private ICodeService _codeService;
        private IFintoService _fintoService;
        private IOntologyTermDataCache _ontologyTermDataCache;
        private ICommonService _commonService;
        private IChannelService _channelService;
        private IOrganizationService _organizationService;

        private UserRoleEnum _userRole;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="attachProposedChannels"></param>
        /// <param name="serviceService">Service service</param>
        /// <param name="generalDescriptionService"></param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto service</param>
        /// <param name="ontologyTermDataCache">Ontology term data cache</param>
        /// <param name="commonService">Common service</param>
        /// <param name="channelService">Channel service</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="userRole">User role</param>
        public SaveServiceManager(
            string id,
            string sourceId,
            IVmOpenApiServiceInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            bool attachProposedChannels,
            IServiceService serviceService,
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache,
            ICommonService commonService,
            IChannelService channelService,
            IOrganizationService organizationService,
            UserRoleEnum userRole)
            :base(id, sourceId, model, openApiVersion, modelState, logger)
        {
            _attachProposedChannels = attachProposedChannels;
            _service = serviceService;
            _gdService = generalDescriptionService;
            _codeService = codeService;
            _fintoService = fintoService;
            _ontologyTermDataCache = ontologyTermDataCache;
            _commonService = commonService;
            _channelService = channelService;
            _organizationService = organizationService;
            _userRole = userRole;
        }

        /// <summary>
        /// Check the request and parameters
        /// </summary>
        /// <returns></returns>
        protected override IActionResult CheckRequestAndParameters()
        {
            var result = base.CheckRequestAndParameters();
            if (result != null) return result;

            // Has user rights for the service
            var isOwnOrganization = ((VmOpenApiServiceVersionBase)CurrentVersion).Security == null ? false : ((VmOpenApiServiceVersionBase)CurrentVersion).Security.IsOwnOrganization;
            if (_userRole != UserRoleEnum.Eeva && !isOwnOrganization)
            {
                return new NotFoundObjectResult(new VmError() { ErrorMessage = $"User has no rights to update this entity!" });
            }

            // Check the general description data. If current version is attached into general description, service type cannot be updated for service.
            // Except if deleteStatutoryServiceGeneralDescriptionId is true (general description will be removed from the service).
            if (CurrentVersion.GeneralDescriptionId.IsAssigned() && !ViewModel.DeleteGeneralDescriptionId)
            {
                ViewModel.Type = null;
                if (string.IsNullOrEmpty(ViewModel.GeneralDescriptionId))
                {
                    // Let's take the attached general description from current version to be able check and validate the data related to GD. SFIPTV-847
                    ViewModel.GeneralDescriptionId = CurrentVersion.GeneralDescriptionId.Value.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected override string GetErrorMessage()
        {
            if (ViewModel.Id.IsAssigned())
            {
                return $"Service with id '{Id}' not found.";
            }

            return $"Service with source id '{SourceId}' not found.";
        }

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceVersionBase CallCurrentVersionServiceMethod()
        {
            return ViewModel.Id.IsAssigned() ? _service.GetServiceByIdSimple(ViewModel.Id.Value, false) : _service.GetServiceBySource(ViewModel.SourceId);
        }

        /// <summary>
        /// Get the entity related data validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            // Get the available languages from current version.
            // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
            var newLanguages = ViewModel.AvailableLanguages.Where(i => !CurrentVersion.AvailableLanguages.Contains(i)).ToList();
            // Available languages is a combination from current (version) available languages and newly set available languages
            var availableLanguages = new HashSet<string>();
            ViewModel.AvailableLanguages.ForEach(i => availableLanguages.Add(i));
            CurrentVersion.AvailableLanguages.ForEach(i => availableLanguages.Add(i));

            return new ServiceValidator(ViewModel, _gdService, _codeService, _fintoService, _ontologyTermDataCache, _commonService, _channelService,
                _organizationService, newLanguages, availableLanguages.ToList(), _userRole, OpenApiVersion, CurrentVersion);
        }

        /// <summary>
        /// The method for updating the service.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceVersionBase CallServiceMethod()
        {
            return _service.SaveService(ViewModel, OpenApiVersion, _attachProposedChannels, SourceId);
        }
    }
}
